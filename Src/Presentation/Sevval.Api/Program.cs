using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Sevval.Api;
using Sevval.Application;
using Sevval.Application.Exceptions;
using Sevval.Application.Features.Common;
using Sevval.Application.Utilities;
using Sevval.Domain.Entities;
using Sevval.Infrastructure;
using Sevval.Mapper.Mapper;
using Sevval.Persistence;
using Sevval.Persistence.Context;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sevval api", Version = "v1" });
    c.EnableAnnotations();
});

builder.Services.AddHttpClient();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence();
builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddIdentity<ApplicationUser, Role>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 5;
    opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";


}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddRoles<Role>()
.AddDefaultTokenProviders().AddSignInManager<SignInManager<ApplicationUser>>();


builder.Services.AddHttpClient();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
    var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();

    opt.TokenValidationParameters
    = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer,
        IssuerSigningKey = SignHelper.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
        ValidAudience = tokenOptions.Audiences[0],
        ValidateIssuerSigningKey = true, //imzas�n� do�rula
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,//token �mr� tolere etme s�resi

    };

});

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));


builder.Services.AddCors(options =>
     options.AddPolicy("SevvalClients", builder =>
     {
         builder.WithOrigins(
             "http://185.48.183.238",
             "https://www.Sevval.com",
             "https://Sevval.com",
             "https://api.Sevval.com",
             "http://185.48.183.238:80",
             "https://185.48.183.238:443",
             "http://localhost:5173",
             "http://localhost:7078",
             "http://localhost:5174",
             "https://localhost:7078",
             "http://localhost:5096",
             "http://localhost:5235",
             "http://localhost:5400",
             "http://localhost:5300"
             )
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
     }
     ));
builder.Services.AddDataProtection()
   .PersistKeysToDbContext<ApplicationDbContext>().SetApplicationName("SevvalApp");

builder.Services.AddMapper();
builder.Services.AddApplication();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("SevvalClients");

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();
app.ConfigureExceptionHandlingMiddleware();

app.Run();
