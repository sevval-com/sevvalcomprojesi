using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Admin.Queries.GetCorporateUsers;
using Sevval.Application.Features.Admin.Queries.GetUserStats;
using Sevval.Application.Features.Admin.Commands.ApproveUser;
using Sevval.Application.Features.Admin.Commands.DeleteUser;
using Sevval.Application.Features.Admin.Commands.BulkAction;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : BaseController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Kurumsal kullanıcı listesini getirir (Admin only)
    /// </summary>
    [HttpGet("corporate-users")]
    [SwaggerOperation(
        Summary = "Kurumsal kullanıcı listesi", 
        Description = "Filtreli ve sayfalandırılmış kurumsal kullanıcı listesini getirir. Sadece Admin rolü erişebilir."
    )]
    [SwaggerResponse(200, "Başarılı", typeof(GetCorporateUsersQueryResponse))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(403, "Admin rolü gerekli")]
    public async Task<IActionResult> GetCorporateUsers(
        [FromQuery] string? userType,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var request = new GetCorporateUsersQueryRequest
        {
            UserType = userType,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Kullanıcı detay ve istatistiklerini getirir (Admin only)
    /// </summary>
    [HttpGet("corporate-users/{userId}/stats")]
    [SwaggerOperation(
        Summary = "Kullanıcı detay ve istatistikleri",
        Description = "Belirtilen kullanıcının detaylı bilgilerini ve ilan istatistiklerini getirir. Sadece Admin rolü erişebilir."
    )]
    [SwaggerResponse(200, "Başarılı", typeof(GetUserStatsQueryResponse))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(403, "Admin rolü gerekli")]
    [SwaggerResponse(404, "Kullanıcı bulunamadı")]
    public async Task<IActionResult> GetUserStats(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var request = new GetUserStatsQueryRequest { UserId = userId };
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Kullanıcıyı onayla veya reddet (Admin only)
    /// </summary>
    [HttpPut("corporate-users/{userId}/approve")]
    [SwaggerOperation(
        Summary = "Kullanıcı onaylama/reddetme",
        Description = "Kullanıcıyı onayla (aktif) veya reddet (reddedildi). Sadece Admin rolü erişebilir."
    )]
    [SwaggerResponse(200, "Başarılı")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(403, "Admin rolü gerekli")]
    [SwaggerResponse(404, "Kullanıcı bulunamadı")]
    public async Task<IActionResult> ApproveUser(
        string userId,
        [FromBody] ApproveUserCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        request.UserId = userId;
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Kullanıcıyı sil (Soft Delete - Admin only)
    /// </summary>
    [HttpDelete("corporate-users/{userId}")]
    [SwaggerOperation(
        Summary = "Kullanıcı silme (Soft Delete)",
        Description = "Kullanıcıyı soft delete yapar (IsActive='deleted'). Sadece Admin rolü erişebilir."
    )]
    [SwaggerResponse(200, "Başarılı")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(403, "Admin rolü gerekli")]
    [SwaggerResponse(404, "Kullanıcı bulunamadı")]
    public async Task<IActionResult> DeleteUser(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteUserAdminCommandRequest { UserId = userId };
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Toplu işlem (Approve veya Delete - Admin only)
    /// </summary>
    [HttpPost("corporate-users/bulk-action")]
    [SwaggerOperation(
        Summary = "Toplu kullanıcı işlemi",
        Description = "Birden fazla kullanıcı için toplu onaylama veya silme işlemi. Sadece Admin rolü erişebilir."
    )]
    [SwaggerResponse(200, "Başarılı")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(403, "Admin rolü gerekli")]
    [SwaggerResponse(400, "Geçersiz action")]
    public async Task<IActionResult> BulkAction(
        [FromBody] BulkActionCommandRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}
