using AutoMapper;
using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;
using Sevval.Application.Features.Company.Queries.GetCompanyByName;
using Sevval.Application.Features.User.Commands.AddUser;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Features.User.Commands.CorporateUpdate;
using Sevval.Application.Features.User.Commands.DeleteUser;
using Sevval.Application.Features.User.Commands.IndividualRegister;
using Sevval.Application.Features.User.Commands.IndividualUpdate;
using Sevval.Application.Features.User.Commands.LoginWithSocialMedia;
using Sevval.Application.Features.User.Commands.UpdateUser;
using Sevval.Application.Features.User.Commands.UpdateUserProfile;
using Sevval.Application.Features.User.Queries.GetAllUsers;
using Sevval.Application.Features.User.Queries.GetUserById;
using Sevval.Domain.Entities;

namespace Sevval.Mapper.Mapping;

public class UserMapping:Profile
{
    public UserMapping()
    {
        CreateMap<ApplicationUser, GetAllUsersQueryResponse>();
        CreateMap<ApplicationUser, GetAboutUsQueryResponse>();


        CreateMap<ApplicationUser, GetCompaniesQueryResponse>()
        //.ForMember(dest => dest.UserOrder, opt => opt.MapFrom(src => src.SomeProperty ?? ""))
 .ForMember(dest => dest.TotalAnnouncement, opt => opt.Ignore())
 .ForMember(dest => dest.CompanyMembershipDuration, opt => opt.Ignore())
            ;


        CreateMap<CorporateRegisterCommandRequest, ApplicationUser>()
            .AfterMap((src, dest) => dest.UserName = src.Email)
            .AfterMap((src, dest) => dest.AcikAdres = src.Address)
            .AfterMap((src, dest) => dest.Referans = src.Reference)
            ;

        CreateMap<IndividualRegisterCommandRequest, ApplicationUser>()
            .AfterMap((src, dest) => dest.UserName = src.Email);

        CreateMap<ApplicationUser, CorporateRegisterCommandResponse>();

        CreateMap<ApplicationUser, IndividualRegisterCommandResponse>();

        CreateMap<ApplicationUser, LoginWithSocialMediaCommandResponse>();

        CreateMap<AddUserCommandRequest, ApplicationUser>()
             .AfterMap((src, dest) => dest.UserName = src.Email); 

        CreateMap<ApplicationUser, AddUserCommandResponse>();

        CreateMap<DeleteUserCommandRequest, DeleteUserCommandResponse>();

        CreateMap<LoginWithSocialMediaCommandRequest, ApplicationUser>()
                        .AfterMap((src, dest) => dest.UserName = src.Email)
        ;


        CreateMap<ApplicationUser, GetUserByIdQueryResponse>()
                        ;



        CreateMap<ApplicationUser, DeleteUserCommandResponse>();

        CreateMap<UpdateUserCommandRequest, ApplicationUser>();

        CreateMap<ApplicationUser, UpdateUserCommandResponse>();

        CreateMap<UpdateUserProfileCommandRequest, ApplicationUser>()
            .AfterMap((src, dest) => dest.AcikAdres = src.Address)
            .AfterMap((src, dest) => dest.VergiNo = src.TaxNumber)
            ;

        CreateMap<ApplicationUser, UpdateUserProfileCommandResponse>();


        CreateMap<CorporateUpdateCommandRequest, ApplicationUser>()
              .AfterMap((src, dest) => dest.UserName = src.Email)
              .AfterMap((src, dest) => dest.AcikAdres = src.Address)
              .AfterMap((src, dest) => dest.VergiNo = src.TaxNumber)
              ;


        CreateMap<ApplicationUser, CorporateUpdateCommandResponse>();


        CreateMap<IndividualUpdateCommandRequest, ApplicationUser>();

        CreateMap<ApplicationUser, IndividualUpdateCommandRequest>();



    }
}
