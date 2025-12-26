using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Base;
using Sevval.Application.Features.Admin.Queries.GetCorporateUsers;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Application.Features.Admin.Queries.GetUserStats;

public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQueryRequest, ApiResponse<GetUserStatsQueryResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public GetUserStatsQueryHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ApiResponse<GetUserStatsQueryResponse>> Handle(
        GetUserStatsQueryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            
            if (user == null)
            {
                return new ApiResponse<GetUserStatsQueryResponse>
                {
                    IsSuccessfull = false,
                    Message = "Kullanıcı bulunamadı",
                    Code = 404
                };
            }

            // İlan istatistikleri
            var announcementsList = await _context.IlanBilgileri
                .Where(a => a.Email == user.Email)
                .ToListAsync(cancellationToken);

            var photoAnnouncements = 0; // Navigation property yok, ayrı tablo
            var videoAnnouncements = announcementsList.Count(a => !string.IsNullOrEmpty(a.VideoLink));

            // Parent company bilgisi
            string? parentCompanyName = null;
            if (!string.IsNullOrEmpty(user.ConsultantCompanyId))
            {
                var parentUser = await _userManager.FindByIdAsync(user.ConsultantCompanyId);
                parentCompanyName = parentUser?.CompanyName;
            }

            var response = new GetUserStatsQueryResponse
            {
                UserInfo = new UserInfoDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserTypes = user.UserTypes,
                    CompanyName = user.CompanyName,
                    IsConsultant = user.IsConsultant,
                    InvitedBy = user.ConsultantCompanyId,
                    ParentCompanyName = parentCompanyName,
                    IsActive = user.IsActive,
                    CreatedDate = user.RegistrationDate,
                    LastLoginDate = user.LockoutEnd?.DateTime
                },
                Statistics = new UserStatsDto
                {
                    TotalAnnouncements = announcementsList.Count(),
                    PhotoAnnouncements = photoAnnouncements,
                    VideoAnnouncements = videoAnnouncements,
                    NoPhotoAnnouncements = announcementsList.Count() - photoAnnouncements,
                    NoVideoAnnouncements = announcementsList.Count() - videoAnnouncements,
                    LastAnnouncementDate = announcementsList.Any() ? announcementsList.Max(a => a.GirisTarihi) : null,
                    FirstAnnouncementDate = announcementsList.Any() ? announcementsList.Min(a => a.GirisTarihi) : null
                }
            };

            return new ApiResponse<GetUserStatsQueryResponse>
            {
                IsSuccessfull = true,
                Message = "Kullanıcı istatistikleri başarıyla getirildi",
                Data = response,
                Code = 200
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetUserStatsQueryResponse>
            {
                IsSuccessfull = false,
                Message = $"Kullanıcı istatistikleri getirilemedi: {ex.Message}",
                Code = 500
            };
        }
    }
}
