using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.User.Commands.ConfirmEstate;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Features.User.Commands.CorporateUpdate;
using Sevval.Application.Features.User.Commands.DeleteUser;
using Sevval.Application.Features.User.Commands.ForgottenPassword;
using Sevval.Application.Features.User.Commands.IndividualRegister;
using Sevval.Application.Features.User.Commands.IndividualUpdate;
using Sevval.Application.Features.User.Commands.LoginWithSocialMedia;
using Sevval.Application.Features.User.Commands.UpdatePassword;
using Sevval.Application.Features.User.Commands.RejectEstate;
using Sevval.Application.Features.User.Commands.SendNewCode;
using Sevval.Application.Features.User.Queries.CheckUserExists;
using Sevval.Application.Features.User.Queries.GetUserById;
using Sevval.Application.Interfaces.IService;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;



namespace Sevval.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public UserController(IMediator mediator, IUserService userService)
    {
        _mediator = mediator;
        _userService = userService;
    }

    [HttpPost(CorporateRegisterCommandRequest.Route)]
    [SwaggerOperation(Summary = "Kurumsal kayıt - form")]
    public async Task<IActionResult> CorporateRegister([FromForm] CorporateRegisterCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost(IndividualRegisterCommandRequest.Route)]
    [SwaggerOperation(Summary = "Bireysel kayıt - form")]
    public async Task<IActionResult> IndividualRegister([FromForm] IndividualRegisterCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost(CheckUserExistsQueryRequest.Route)]
    [SwaggerOperation(Summary = "Kullanıcının sistemde kayıtlı olup olmadığını kontrol eder.(Telefon Numarası ve E-posta)")]
    public async Task<IActionResult> CheckUserExists([FromBody] CheckUserExistsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }


    [HttpPost(ForgottenPasswordCommandRequest.Route)]
    [SwaggerOperation(Summary = "Şifre yenilemek için kullanıcı bilgilerini giriş yapar.")]
    public async Task<IActionResult> ForgottenPassword([FromBody] ForgottenPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }


    [HttpPost(ConfirmEstateCommandRequest.Route)]
    [SwaggerOperation(Summary = "Emlakcı hesabı onaylama işlemini gerçekleştirir.")]
    public async Task<IActionResult> ConfirmEstate([FromBody] ConfirmEstateCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    [HttpPost(RejectEstateCommandRequest.Route)]
    [SwaggerOperation(Summary = "Emlakcı hesabı reddetme işlemini gerçekleştirir.")]
    public async Task<IActionResult> RejectEstate([FromBody] RejectEstateCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    [HttpPost(SendNewCodeCommandRequest.Route)]
    [SwaggerOperation(Summary = "Şifre yenilemek için kod gönderir.")]
    public async Task<IActionResult> SendNewCode([FromBody] SendNewCodeCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }


    [HttpPost(LoginWithSocialMediaCommandRequest.Route)]
    [SwaggerOperation(Summary = "Sosyal medyada oturum açma işlemini gerçekleştirir.")]
    public async Task<IActionResult> LoginWithSocialMedia([FromBody] LoginWithSocialMediaCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }


    [HttpPut(CorporateUpdateCommandRequest.Route)]
    [SwaggerOperation(Summary = "Kurumsal kullanıcı bilgilerini gunceller.")]
    public async Task<IActionResult> CorporateUpdate([FromForm] CorporateUpdateCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }


    [HttpPut(IndividualUpdateCommandRequest.Route)]
    [SwaggerOperation(Summary = "Bireysel kullanıcı bilgilerini gunceller.")]
    public async Task<IActionResult> IndividualUpdate([FromForm] IndividualUpdateCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    [Authorize]
    [HttpPut(UpdatePasswordCommandRequest.Route)]
    [SwaggerOperation(Summary = "Kullanıcı şifresini günceller. Kullanıcı giriş yapmış olmalıdır.")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        // Set the UserId from the authenticated user
        request.UserId = GetCurrentUserId();
        
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }


    [Authorize]
    [HttpGet(GetUserByIdQueryRequest.Route)]
    [SwaggerOperation(Summary = "Oturum açmış kullanıcı bilgilerini getirir.")]

    public async Task<IActionResult> GetUser(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetUserByIdQueryRequest { Id = GetCurrentUserId() }, cancellationToken);
        return Ok(response);
    }

    
    //[Authorize(Roles = "admin")]
    [HttpDelete(DeleteUserCommandRequest.Route + "/{id}")]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new DeleteUserCommandRequest { Id = id }, cancellationToken);
        return Ok(response);
    }


    /* // [Authorize(Roles = "admin")]
     [HttpPost(AddUserCommandRequest.Route)]
     public async Task<IActionResult> AddUser(AddUserCommandRequest request, CancellationToken cancellationToken)
     {
         var response = await _mediator.Send(request, cancellationToken);
         return Ok(response);
     }

     //[Authorize(Roles = "admin")]
     [HttpGet(GetAllUsersQueryRequest.Route)]
     public async Task<IActionResult> GetUsers([FromQuery] GetAllUsersQueryRequest request, CancellationToken cancellationToken)
     {
         var response = await _mediator.Send(request);
         return Ok(response);
     }


     
     //[Authorize(Roles = "admin")]
     [HttpDelete(DeleteUserCommandRequest.Route + "/{id}")]
     public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
     {
         var response = await _mediator.Send(new DeleteUserCommandRequest { Id = id }, cancellationToken);
         return Ok(response);
     }

     //[Authorize(Roles = "admin")]
     [HttpPut(UpdateUserCommandRequest.Route)]
     public async Task<IActionResult> UpdateUser(UpdateUserCommandRequest request, CancellationToken cancellationToken)
     {
         var response = await _mediator.Send(request, cancellationToken);
         return Ok(response);
     }



     //[Authorize]
     [HttpPut(UpdateUserProfileCommandRequest.Route)]
     public async Task<IActionResult> UpdateUserProfile(UpdateUserProfileCommandRequest request, CancellationToken cancellationToken)
     {
         var response = await _mediator.Send(request, cancellationToken);
         return Ok(response);
     }
     */



    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Sign out the user
        await _userService.Logout();

        return Ok("Logged out successfully");
    }




}
