using MediatR;
using Microsoft.AspNetCore.Identity;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Application.Features.Admin.Commands.ApproveUser;

public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommandRequest, ApiResponse<ApproveUserCommandResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApproveUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<ApproveUserCommandResponse>> Handle(
        ApproveUserCommandRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            
            if (user == null)
            {
                return new ApiResponse<ApproveUserCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = "Kullanıcı bulunamadı",
                    Code = 404
                };
            }

            // Onaylama veya reddetme
            user.IsActive = request.Approved ? "active" : "rejected";

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ApiResponse<ApproveUserCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = $"Kullanıcı güncellenemedi: {errors}",
                    Code = 500
                };
            }

            var response = new ApproveUserCommandResponse
            {
                UserId = user.Id,
                IsActive = user.IsActive
            };

            var message = request.Approved ? "Kullanıcı başarıyla onaylandı" : "Kullanıcı reddedildi";
            return new ApiResponse<ApproveUserCommandResponse>
            {
                IsSuccessfull = true,
                Message = message,
                Data = response,
                Code = 200
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ApproveUserCommandResponse>
            {
                IsSuccessfull = false,
                Message = $"İşlem başarısız: {ex.Message}",
                Code = 500
            };
        }
    }
}
