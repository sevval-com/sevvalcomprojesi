using MediatR;
using Microsoft.AspNetCore.Identity;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Application.Features.Admin.Commands.DeleteUser;

public class DeleteUserAdminCommandHandler : IRequestHandler<DeleteUserAdminCommandRequest, ApiResponse<DeleteUserAdminCommandResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserAdminCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<DeleteUserAdminCommandResponse>> Handle(
        DeleteUserAdminCommandRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            
            if (user == null)
            {
                return new ApiResponse<DeleteUserAdminCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = "Kullanıcı bulunamadı",
                    Code = 404
                };
            }

            // Soft delete - sadece IsActive alanını "deleted" yap
            user.IsActive = "deleted";

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ApiResponse<DeleteUserAdminCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = $"Kullanıcı silinemedi: {errors}",
                    Code = 500
                };
            }

            var response = new DeleteUserAdminCommandResponse
            {
                UserId = user.Id,
                IsActive = user.IsActive
            };

            return new ApiResponse<DeleteUserAdminCommandResponse>
            {
                IsSuccessfull = true,
                Message = "Kullanıcı başarıyla silindi",
                Data = response,
                Code = 200
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<DeleteUserAdminCommandResponse>
            {
                IsSuccessfull = false,
                Message = $"İşlem başarısız: {ex.Message}",
                Code = 500
            };
        }
    }
}
