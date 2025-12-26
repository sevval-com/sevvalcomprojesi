using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Application.Features.Admin.Queries.GetCorporateUsers;

public class GetCorporateUsersQueryHandler : IRequestHandler<GetCorporateUsersQueryRequest, ApiResponse<GetCorporateUsersQueryResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public GetCorporateUsersQueryHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<ApiResponse<GetCorporateUsersQueryResponse>> Handle(
        GetCorporateUsersQueryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Temel sorgu - tüm kullanıcılar
            var query = _userManager.Users.AsQueryable();

            // UserType filtresi
            if (!string.IsNullOrEmpty(request.UserType) && request.UserType != "Tümü")
            {
                query = query.Where(u => u.UserTypes == request.UserType);
            }

            // Status filtresi
            if (!string.IsNullOrEmpty(request.Status) && request.Status != "All")
            {
                query = query.Where(u => u.IsActive.ToLower() == request.Status.ToLower());
            }

            // Toplam kayıt sayısı
            var totalCount = await query.CountAsync(cancellationToken);

            // Sayfalama ve sıralama
            var users = await query
                .OrderByDescending(u => u.RegistrationDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // DTO'ya dönüştürme
            var userDtos = new List<CorporateUserDto>();

            foreach (var user in users)
            {
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

                userDtos.Add(new CorporateUserDto
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
                    LastLoginDate = user.LockoutEnd?.DateTime, // DateTimeOffset'ten DateTime'a çevir
                    Stats = new UserStatsDto
                    {
                        TotalAnnouncements = announcementsList.Count(),
                        PhotoAnnouncements = photoAnnouncements,
                        VideoAnnouncements = videoAnnouncements,
                        NoPhotoAnnouncements = announcementsList.Count() - photoAnnouncements,
                        NoVideoAnnouncements = announcementsList.Count() - videoAnnouncements,
                        LastAnnouncementDate = announcementsList.Any() ? announcementsList.Max(a => a.GirisTarihi) : null,
                        FirstAnnouncementDate = announcementsList.Any() ? announcementsList.Min(a => a.GirisTarihi) : null
                    },
                    Documents = new UserDocumentsDto
                    {
                        Level5CertificatePath = user.Document1Path,
                        TaxPlatePath = user.Document2Path,
                        ProfilePicturePath = user.ProfilePicturePath
                    }
                });
            }

            var response = new GetCorporateUsersQueryResponse
            {
                Users = userDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };

            return new ApiResponse<GetCorporateUsersQueryResponse>
            {
                IsSuccessfull = true,
                Message = "Kullanıcı listesi başarıyla getirildi",
                Data = response,
                Code = 200
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetCorporateUsersQueryResponse>
            {
                IsSuccessfull = false,
                Message = $"Kullanıcı listesi getirilemedi: {ex.Message}",
                Code = 500
            };
        }
    }
}
