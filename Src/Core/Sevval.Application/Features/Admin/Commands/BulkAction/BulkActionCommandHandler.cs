using MediatR;
using Microsoft.AspNetCore.Identity;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Application.Features.Admin.Commands.BulkAction;

public class BulkActionCommandHandler : IRequestHandler<BulkActionCommandRequest, ApiResponse<BulkActionCommandResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public BulkActionCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<BulkActionCommandResponse>> Handle(
        BulkActionCommandRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.Action != "approve" && request.Action != "delete")
            {
                return new ApiResponse<BulkActionCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = "Geçersiz action. 'approve' veya 'delete' olmalıdır.",
                    Code = 400
                };
            }

            var results = new List<BulkActionResult>();
            int successCount = 0;
            int failedCount = 0;

            foreach (var userId in request.UserIds)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    
                    if (user == null)
                    {
                        results.Add(new BulkActionResult
                        {
                            UserId = userId,
                            Success = false,
                            ErrorMessage = "Kullanıcı bulunamadı"
                        });
                        failedCount++;
                        continue;
                    }

                    // Action'a göre işlem yap
                    if (request.Action == "approve")
                    {
                        user.IsActive = "active";
                    }
                    else if (request.Action == "delete")
                    {
                        user.IsActive = "deleted";
                    }

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        results.Add(new BulkActionResult
                        {
                            UserId = userId,
                            Success = true
                        });
                        successCount++;
                    }
                    else
                    {
                        results.Add(new BulkActionResult
                        {
                            UserId = userId,
                            Success = false,
                            ErrorMessage = "Güncelleme başarısız"
                        });
                        failedCount++;
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new BulkActionResult
                    {
                        UserId = userId,
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                    failedCount++;
                }
            }

            var response = new BulkActionCommandResponse
            {
                SuccessCount = successCount,
                FailedCount = failedCount,
                Results = results
            };

            return new ApiResponse<BulkActionCommandResponse>
            {
                IsSuccessfull = true,
                Message = "Toplu işlem tamamlandı",
                Data = response,
                Code = 200
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<BulkActionCommandResponse>
            {
                IsSuccessfull = false,
                Message = $"Toplu işlem başarısız: {ex.Message}",
                Code = 500
            };
        }
    }
}
