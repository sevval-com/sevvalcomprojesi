using AutoMapper;
using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementDetails;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser;
using Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.GetTodaysAnnouncements;
using Sevval.Application.Features.Announcement.Queries.SearchAnnouncements;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Utilities;
using Sevval.Domain.Entities;

namespace Sevval.Infrastructure.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly IReadRepository<IlanModel> _readRepository;
    private readonly IReadRepository<PhotoModel> _photoRepository;
    private readonly IReadRepository<VideoModel> _videoRepository;
    private readonly IReadRepository<ApplicationUser> _readApplicationUserRepository;
    private readonly IReadRepository<ConsultantInvitation> _readConsultantInvitationRepository;
    private readonly IReadRepository<YorumModel> _readCommentRepository;
    private readonly IReadRepository<GununIlanModel> _readGununIlanRepository;
    private readonly IWriteRepository<IlanModel> _writeRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AnnouncementService(
        IReadRepository<IlanModel> readRepository,
        IReadRepository<PhotoModel> photoRepository,
        IReadRepository<VideoModel> videoRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IReadRepository<ApplicationUser> readApplicationUserRepository,
        IReadRepository<ConsultantInvitation> readConsultantInvitationRepository,
        IReadRepository<YorumModel> readCommentRepository,
        IReadRepository<GununIlanModel> readGununIlanRepository,
        IWriteRepository<IlanModel> writeRepository,
        IUnitOfWork unitOfWork)
    {
        _readRepository = readRepository;
        _photoRepository = photoRepository;
        _videoRepository = videoRepository;
        _mapper = mapper;
        _readApplicationUserRepository = readApplicationUserRepository;
        _readConsultantInvitationRepository = readConsultantInvitationRepository;
        _readCommentRepository = readCommentRepository;
        _readGununIlanRepository = readGununIlanRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<GetAnnouncementCountQueryResponse>> GetAnnouncementCountAsync(GetAnnouncementCountQueryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _readRepository.Queryable();

            // Filter by status if provided
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(i => i.Status == request.Status);
            }


            var count = await query.CountAsync(cancellationToken);

            var message = "İlan sayısı başarıyla getirildi.";

            return new ApiResponse<GetAnnouncementCountQueryResponse>
            {
                Data = new GetAnnouncementCountQueryResponse
                {
                    TotalCount = count,
                    Status = request.Status ?? "all",
                    Message = message
                },
                IsSuccessfull = true,
                Message = message
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetAnnouncementCountQueryResponse>
            {
                Data = new GetAnnouncementCountQueryResponse
                {
                    TotalCount = 0,
                    Status = request.Status ?? "all",
                    Message = "İlan sayısı getirilirken bir hata oluştu."
                },
                IsSuccessfull = false,
                Message = $"İlan sayısı getirilirken bir hata oluştu: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>> GetAnnouncementCountByTypeAsync(GetAnnouncementCountByTypeQueryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _readRepository.Queryable();

            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(i => i.Status == request.Status);
            }



            var announcementCountsByType = await query
                .GroupBy(i => i.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() }).OrderByDescending(a => a.Count)
                .ToListAsync();

            List<GetAnnouncementCountByTypeQueryResponse> list = new List<GetAnnouncementCountByTypeQueryResponse>();

            foreach (var item in announcementCountsByType)
            {
                //GetImagePath
                list.Add(new GetAnnouncementCountByTypeQueryResponse
                {
                    Count = item.Count,
                    Type = item.Category,
                    ImagePath = GeneralHelper.GetImagePath(item.Category),

                });
            }

            var message = "ilanların tip bazında sayıları başarıyla getirildi.";

            return new ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>
            {
                Data = list,
                IsSuccessfull = true,
                Message = message
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>
            {
                Data = new List<GetAnnouncementCountByTypeQueryResponse>(),
                IsSuccessfull = false,
                Message = $"İlan tip sayıları getirilirken bir hata oluştu: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>> GetAnnouncementCountByProvinceAsync(GetAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _readRepository.Queryable();

            // Filter by status if provided
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(i => i.Status == request.Status);
            }


            // Group by sehir (province) and count
            var announcementCountsByProvince = await query
                .GroupBy(i => i.sehir ?? "Belirtilmemiş")
                .Select(g => new { Province = g.Key, Count = g.Count() }).OrderByDescending(a => a.Count)
                .ToListAsync();

            List<GetAnnouncementCountByProvinceQueryResponse> list = new List<GetAnnouncementCountByProvinceQueryResponse>();

            foreach (var item in announcementCountsByProvince)
            {
                list.Add(new GetAnnouncementCountByProvinceQueryResponse
                {
                    Province = item.Province,
                    Comment = GeneralHelper.GetLandmark(item.Province),
                    ImagePath = $"/sehirler/{item.Province}.jpeg",
                    Count = item.Count

                });
            }


            var message = "ilanların il bazında sayıları başarıyla getirildi.";

            return new ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>
            {
                Data = list,
                IsSuccessfull = true,
                Message = message
            };
        }
        catch (Exception ex)
        {

        }

        return new ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>
        {
            Data = new List<GetAnnouncementCountByProvinceQueryResponse>(),
            IsSuccessfull = false,
            Message = $"İlan il sayıları getirilirken bir hata oluştu"
        };
    }

    public async Task<ApiResponse<GetTodaysAnnouncementsQueryResponse>> GetTodaysAnnouncementsAsync(GetTodaysAnnouncementsQueryRequest request, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;

        // 1) Try to get today's featured listing from GununIlanlari
        var featuredToday = await _readGununIlanRepository.Queryable()
            .Where(g => g.YayinlanmaTarihi.Date == today)
            .OrderByDescending(g => g.YayinlanmaTarihi)
            .FirstOrDefaultAsync(cancellationToken);

        // 2) If none for today, fallback to most recent featured
        if (featuredToday == null)
        {
            featuredToday = await _readGununIlanRepository.Queryable()
                .OrderByDescending(g => g.YayinlanmaTarihi)
                .FirstOrDefaultAsync(cancellationToken);
        }

        IlanModel selectedAnnouncement = null;

        if (featuredToday != null)
        {
            // Find corresponding Ilan by Id from featured record
            var ilanQuery = _readRepository.Queryable().Where(i => i.Id == featuredToday.Id);
            if (!string.IsNullOrEmpty(request.Status))
            {
                ilanQuery = ilanQuery.Where(i => i.Status == request.Status);
            }
            selectedAnnouncement = await ilanQuery.FirstOrDefaultAsync(cancellationToken);
        }

        // 3) If still not found, fallback to latest created today by GirisTarihi
        if (selectedAnnouncement == null)
        {
            var ilanQuery = _readRepository.Queryable()
                .Where(i => i.GirisTarihi.Date == today);
            if (!string.IsNullOrEmpty(request.Status))
            {
                ilanQuery = ilanQuery.Where(i => i.Status == request.Status);
            }
            selectedAnnouncement = await ilanQuery
                .OrderByDescending(i => i.GirisTarihi)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (selectedAnnouncement == null)
        {
            return new ApiResponse<GetTodaysAnnouncementsQueryResponse>
            {
                Code = 404,
                Data = new GetTodaysAnnouncementsQueryResponse(),
                IsSuccessfull = false,
                Message = "Bugünün ilanları bulunamadı"
            };
        }

        var response = new GetTodaysAnnouncementsQueryResponse();

        // Safe image load (avoid NRE when selectedAnnouncement is null)
        var images = await _photoRepository.Queryable()
            .Where(photo => photo.IlanId == selectedAnnouncement.Id)
            .Select(a => a.Url)
            .ToListAsync(cancellationToken);

        response.Id = selectedAnnouncement.Id;
        response.Category = selectedAnnouncement.Category;
        response.Title = selectedAnnouncement.Title;
        response.Description = selectedAnnouncement.Description;
        response.Price = $"{selectedAnnouncement.Price:N0} TL";
        response.Area = selectedAnnouncement.Area;
        response.Sehir = selectedAnnouncement.sehir;
        response.Semt = selectedAnnouncement.semt;
        response.MahalleKoy = selectedAnnouncement.mahalleKoy;
        response.GirisTarihi = selectedAnnouncement.GirisTarihi;
        response.FirstName = selectedAnnouncement.FirstName;
        response.LastName = selectedAnnouncement.LastName;
        response.PhoneNumber = selectedAnnouncement.PhoneNumber;
        response.Email = selectedAnnouncement.Email;
        response.GoruntulenmeSayisi = selectedAnnouncement.GoruntulenmeSayisi;
        response.Status = selectedAnnouncement.Status;
        response.ProfilePicture = selectedAnnouncement.ProfilePicture;
        response.IlanNo = selectedAnnouncement.IlanNo;
        response.MulkTipi = selectedAnnouncement.MulkTipi;
        response.OdaSayisi = selectedAnnouncement.OdaSayisi;
        response.BinaYasi = selectedAnnouncement.BinaYasi;
        response.KatSayisi = selectedAnnouncement.KatSayisi;
        response.BulunduguKat = selectedAnnouncement.BulunduguKat;
        response.TapuDurumu = selectedAnnouncement.TapuDurumu;
        response.Kimden = selectedAnnouncement.Kimden;
        response.KonutDurumu = selectedAnnouncement.KonutDurumu;
        response.AdaNo = selectedAnnouncement.AdaNo;
        response.ParselNo = selectedAnnouncement.ParselNo;
        response.NetMetrekare = selectedAnnouncement.NetMetrekare;

        // If this ilan is featured today, add daily featured extra view count
        try
        {
            var today2 = DateTime.Today;
            var featuredCounter = await _readGununIlanRepository.Queryable()
                .Where(g => g.YayinlanmaTarihi.Date == today2 && g.Id == selectedAnnouncement.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (featuredCounter != null)
            {
                var baseViews = selectedAnnouncement.GoruntulenmeSayisi;
                var extraViews = featuredCounter.GoruntulenmeSayisi;
                response.GoruntulenmeSayisi = (baseViews) + (extraViews);
            }
        }
        catch { }

        response.ImagePaths = images != null && images.Any() ? images : new List<string>() {
            "/images/Common/default.webp",
            "/images/Common/sevval.jpg",
            "/images/Common/default.webp"
        };

        return new ApiResponse<GetTodaysAnnouncementsQueryResponse>
        {
            Data = response,
            IsSuccessfull = true,
            Message = $"Bugünün en son ilanı başarıyla getirildi."
        };

    }

    public async Task<ApiResponse<SearchAnnouncementsQueryResponse>> SearchAnnouncementsAsync(SearchAnnouncementsQueryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _readRepository.Queryable().AsNoTracking();

            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(x => x.Status == request.Status);
            }
            else
            {
                query = query.Where(x => x.Status == "active");
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                if (int.TryParse(request.Keyword, out int announcementId))
                {
                    query = query.Where(x => x.Id == announcementId ||
                                            (x.sehir ?? "").Contains(request.Keyword) ||
                                            (x.semt ?? "").Contains(request.Keyword) ||
                                            (x.mahalleKoy ?? "").Contains(request.Keyword) ||
                                            (x.Title ?? "").Contains(request.Keyword) ||
                                            (x.Description ?? "").Contains(request.Keyword));
                }
                else
                {
                    query = query.Where(x => (x.sehir ?? "").ToLower().Contains(request.Keyword.ToLower()) ||
                                            (x.semt ?? "").ToLower().Contains(request.Keyword.ToLower()) ||
                                            (x.mahalleKoy ?? "").ToLower().Contains(request.Keyword.ToLower()) ||
                                            (x.Title ?? "").ToLower().Contains(request.Keyword.ToLower()) ||
                                            (x.Description ?? "").ToLower().Contains(request.Keyword.ToLower()));
                }
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                query = query.Where(x => (x.Category ?? "") == request.Category);
            }

            if (!string.IsNullOrEmpty(request.PropertyType))
            {
                query = query.Where(x => (x.MulkTipi ?? "") == request.PropertyType);
            }

            if (!string.IsNullOrEmpty(request.PropertyStatus))
            {
                query = query.Where(x => (x.KonutDurumu ?? "") == request.PropertyStatus);
            }

            if (request.MinPrice.HasValue && request.MinPrice > 0)
            {
                query = query.Where(x => x.Price >= request.MinPrice);
            }
            if (request.MaxPrice.HasValue && request.MaxPrice > 0)
            {
                query = query.Where(x => x.Price <= request.MaxPrice);
            }

            if (!string.IsNullOrEmpty(request.Province))
            {
                query = query.Where(x => (x.sehir ?? "") == request.Province);
            }
            if (!string.IsNullOrEmpty(request.District))
            {
                query = query.Where(x => (x.semt ?? "") == request.District);
            }
            if (!string.IsNullOrEmpty(request.Neighborhood))
            {
                query = query.Where(x => (x.mahalleKoy ?? "") == request.Neighborhood);
            }

            if (request.MinArea.HasValue && request.MinArea > 0)
            {
                query = query.Where(x => x.Area >= request.MinArea);
            }
            if (request.MaxArea.HasValue && request.MaxArea > 0)
            {
                query = query.Where(x => x.Area <= request.MaxArea);
            }

            if (!string.IsNullOrEmpty(request.RoomCount))
            {
                query = query.Where(x => (x.OdaSayisi ?? "") == request.RoomCount);
            }
            if (!string.IsNullOrEmpty(request.BedroomCount))
            {
                query = query.Where(x => (x.YatakSayisi ?? "") == request.BedroomCount);
            }
            if (!string.IsNullOrEmpty(request.BuildingAge))
            {
                query = query.Where(x => (x.BinaYasi ?? "") == request.BuildingAge);
            }
            if (!string.IsNullOrEmpty(request.HeatingType))
            {
                query = query.Where(x => (x.Isitma ?? "") == request.HeatingType);
            }
            if (!string.IsNullOrEmpty(request.BalconyCount))
            {
                query = query.Where(x => (x.Balkon ?? "") == request.BalconyCount);
            }
            if (!string.IsNullOrEmpty(request.ElevatorStatus))
            {
                query = query.Where(x => (x.Asansor ?? "") == request.ElevatorStatus);
            }
            if (!string.IsNullOrEmpty(request.ParkingStatus))
            {
                query = query.Where(x => (x.Otopark ?? "") == request.ParkingStatus);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.GirisTarihi >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(x => x.GirisTarihi <= request.EndDate.Value);
            }

            //if (!string.IsNullOrEmpty(request.UserEmail))
            //{
            //    query = query.Where(x => (x.UserEmail ?? "") == request.UserEmail);
            //}

            if (request.HasPhotos.HasValue)
            {
                var photoAnnouncementIds = await _photoRepository.Queryable()
                    .Select(p => p.IlanId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (request.HasPhotos.Value)
                {
                    query = query.Where(x => photoAnnouncementIds.Contains(x.Id));
                }
                else
                {
                    query = query.Where(x => !photoAnnouncementIds.Contains(x.Id));
                }
            }

            if (request.HasVideos.HasValue)
            {
                var videoAnnouncementIds = await _videoRepository.Queryable()
                    .Select(v => v.IlanId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (request.HasVideos.Value)
                {
                    query = query.Where(x => videoAnnouncementIds.Contains(x.Id));
                }
                else
                {
                    query = query.Where(x => !videoAnnouncementIds.Contains(x.Id));
                }
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = request.SortBy?.ToLower() switch
            {
                "price" => request.SortOrder?.ToUpper() == "ASC"
                    ? query.OrderBy(x => x.Price)
                    : query.OrderByDescending(x => x.Price),
                "area" => request.SortOrder?.ToUpper() == "ASC"
                    ? query.OrderBy(x => x.Area)
                    : query.OrderByDescending(x => x.Area),
                "title" => request.SortOrder?.ToUpper() == "ASC"
                    ? query.OrderBy(x => x.Title)
                    : query.OrderByDescending(x => x.Title),
                _ => request.SortOrder?.ToUpper() == "ASC"
                    ? query.OrderBy(x => x.GirisTarihi)
                    : query.OrderByDescending(x => x.GirisTarihi)
            };

            // Apply pagination
            var skip = (request.Page - 1) * request.PageSize;
            var pagedQuery = query.Skip(skip).Take(request.PageSize);

            // Execute query and get results
            var announcements = await pagedQuery.Select(x => new AnnouncementSearchResultDto
            {
                Id = x.Id,
                Title = x.Title ?? "",
                Description = x.Description ?? "",
                Price = x.Price,
                Category = x.Category ?? "",
                PropertyType = x.MulkTipi ?? "",
                PropertyStatus = x.KonutDurumu ?? "",
                Area = x.Area,
                Province = x.sehir ?? "",
                District = x.semt ?? "",
                Neighborhood = x.mahalleKoy ?? "",
                RoomCount = x.OdaSayisi,
                BedroomCount = x.YatakSayisi,
                BuildingAge = x.BinaYasi,
                NetArea = x.NetMetrekare,
                AdaNo = x.AdaNo,
                ParselNo = x.ParselNo,
                CreatedDate = x.GirisTarihi,
                ImageUrl = null,
                //UserName = x.UserName ?? "",
                PhoneNumber = x.PhoneNumber ?? "",
                //CompanyName = x.CompanyName ?? "",
                HasPhotos = false, // Will be set below
                HasVideos = false  // Will be set below
            }).ToListAsync(cancellationToken);

            // Set photo and video status for each announcement
            if (announcements.Any())
            {
                var announcementIds = announcements.Select(a => a.Id).ToList();

                var photoAnnouncementIds = await _photoRepository.Queryable()
                    .Where(p => announcementIds.Contains(p.IlanId))
                    .Select(p => new { p.IlanId, p.Url })
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var videoAnnouncementIds = await _videoRepository.Queryable()
                    .Where(v => announcementIds.Contains(v.IlanId))
                    .Select(v => v.IlanId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                foreach (var announcement in announcements)
                {
                    announcement.HasPhotos = photoAnnouncementIds.Select(a => a.IlanId).Contains(announcement.Id);
                    announcement.HasVideos = videoAnnouncementIds.Contains(announcement.Id);
                    announcement.ImageUrl = photoAnnouncementIds.FirstOrDefault(a => a.IlanId == announcement.Id)?.Url
                        ?? "/images/Common/default.webp";
                }
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var hasNextPage = request.Page < totalPages;
            var hasPreviousPage = request.Page > 1;

            var appliedFilters = new SearchFiltersDto
            {
                Keyword = request.Keyword,
                Category = request.Category,
                PropertyType = request.PropertyType,
                PropertyStatus = request.PropertyStatus,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Province = request.Province,
                District = request.District,
                Neighborhood = request.Neighborhood,
                MinArea = request.MinArea,
                MaxArea = request.MaxArea,
                RoomCount = request.RoomCount,
                BuildingAge = request.BuildingAge,
                HasPhotos = request.HasPhotos,
                HasVideos = request.HasVideos,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SortBy = request.SortBy ?? "Date",
                SortOrder = request.SortOrder ?? "DESC",
                Page = request.Page,
                PageSize = request.PageSize
            };

            var message = totalCount > 0
                ? $"{totalCount} ilan bulundu."
                : "Arama kriterlerinize uygun ilan bulunamadı.";

            var response = new SearchAnnouncementsQueryResponse
            {
                Announcements = announcements,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage,
                Message = message,
                AppliedFilters = appliedFilters
            };

            return new ApiResponse<SearchAnnouncementsQueryResponse>
            {
                Data = response,
                IsSuccessfull = true,
                Message = message
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<SearchAnnouncementsQueryResponse>
            {
                Data = null,
                IsSuccessfull = false,
                Message = $"İlan arama işlemi sırasında hata oluştu: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>> GetAnnouncementsByCompanyAsync(GetAnnouncementsByCompanyQueryRequest request, CancellationToken cancellationToken)
    {


        var user = await _readApplicationUserRepository.GetAsync(a => a.Id == request.UserId);

        if (user is null)
        {
            return new ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>
            {
                Code = 404,
                Data = new List<GetAnnouncementsByCompanyQueryResponse>(),
                IsSuccessfull = false,
                Message = "Firma bulunamadı"
            };
        }


        // Include company owner's email and all consultant emails invited by this company
        var consultantEmails = _readConsultantInvitationRepository.Queryable()
            .Where(ci => ci.InvitedBy == user.Id.ToString())
            .Select(ci => ci.Email)
            .ToList();

        consultantEmails.Add(user.Email);

        var announcements = _readRepository.Queryable().Where(a => consultantEmails.Contains(a.Email));


        if (!string.IsNullOrEmpty(request.Status))
            announcements = announcements.Where(a => a.Status == request.Status);

        if (!announcements.Any())
        {
            return new ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>
            {
                Code = 404,
                Data = new List<GetAnnouncementsByCompanyQueryResponse>(),
                IsSuccessfull = false,
                Message = "Firmaya ait ilan bulunamadı"
            };
        }



        var result = await PaginatedList<IlanModel>.
      CreateAsync(announcements.OrderByDescending(a => a.Id),
      request.Page, request.Size);



        if (result.TotalItems == 0)
        {
            return new ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>
            {
                Code = 404,
                IsSuccessfull = false,
                Message = "İlan bulunamadı.",
                Data = new List<GetAnnouncementsByCompanyQueryResponse>()
            };
        }

        var mapped = _mapper.Map<List<GetAnnouncementsByCompanyQueryResponse>>(result);


        foreach (var item in mapped)
        {
            item.Images = await _photoRepository.Queryable()
                .Where(photo => photo.IlanId == item.Id)
                .Select(photo => photo.Url)
                .ToListAsync(cancellationToken);
        }


        return new ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>
        {
            Data = mapped,
            IsSuccessfull = true,
            Message = "Firma ilanları başarıyla getirildi.",
            Code = 200,
            Meta = new MetaData
            {
                Pagination = new Pagination
                {
                    PageNumber = request.Page,
                    PageSize = request.Size,
                    TotalItem = result.TotalItems,
                    TotalPage = (int)Math.Ceiling((double)result.TotalItems / request.Size)
                }
            }
        };

    }

    public async Task<ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>> GetCompanyAnnouncementCountByProvinceAsync(GetCompanyAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken)
    {

        var user = await _readApplicationUserRepository.GetAsync(a => a.Id == request.UserId);


        if (user is null)
        {
            return new ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>
            {
                IsSuccessfull = false,
                Message = "Belirtilen şirket adına ait kullanıcı bulunamadı.",
                Data = new List<GetCompanyAnnouncementCountByProvinceQueryResponse>()
            };
        }


        // Get consultant emails for this company
        var consultantEmails = _readConsultantInvitationRepository.Queryable()
            .Where(ci => ci.InvitedBy == user.Id.ToString())
            .Select(ci => ci.Email)
            .ToList();

        var query = _readRepository.Queryable()
            .Where(i => i.Email == user.Email || consultantEmails.Contains(i.Email));

        // Filter by status if provided
        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(i => i.Status == request.Status);
        }

        // Group by province and count announcements
        var provinceCountsQuery = query
            .Where(i => !string.IsNullOrEmpty(i.sehir))
            .GroupBy(i => i.sehir)
            .Select(g => new GetCompanyAnnouncementCountByProvinceQueryResponse
            {
                Province = g.Key,
                Count = g.Count(),

            });

        var provinceCounts = await provinceCountsQuery
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Province)
            .ToListAsync(cancellationToken);

        return new ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>
        {
            IsSuccessfull = true,
            Message = "İl bazında ilan sayıları getirildi.",
            Data = provinceCounts,

        };

    }

    public async Task<ApiResponse<List<GetAnnouncementsByUserQueryResponse>>> GetAnnouncementsByUserAsync(GetAnnouncementsByUserQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user by email
            var user = await _readApplicationUserRepository.GetAsync(a => a.Email == request.Email);
            if (user == null)
            {
                return new ApiResponse<List<GetAnnouncementsByUserQueryResponse>>
                {
                    IsSuccessfull = false,
                    Message = "Belirtilen e-posta adresine ait kullanıcı bulunamadı.",
                    Data = new List<GetAnnouncementsByUserQueryResponse>()
                };
            }

            // Build query for user's announcements
            var query = _readRepository.Queryable()
                .Where(a => a.Email == user.Email);

            // Filter by status if provided
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(a => a.Status == request.Status);
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "price" => request.SortOrder == "ASC" ? query.OrderBy(a => a.Price) : query.OrderByDescending(a => a.Price),
                    "area" => request.SortOrder == "ASC" ? query.OrderBy(a => a.Area) : query.OrderByDescending(a => a.Area),
                    "title" => request.SortOrder == "ASC" ? query.OrderBy(a => a.Title) : query.OrderByDescending(a => a.Title),
                    "viewcount" => request.SortOrder == "ASC" ? query.OrderBy(a => a.GoruntulenmeSayisi) : query.OrderByDescending(a => a.GoruntulenmeSayisi),
                    _ => request.SortOrder == "ASC" ? query.OrderBy(a => a.GirisTarihi) : query.OrderByDescending(a => a.GirisTarihi)
                };

            }

            var result = await PaginatedList<IlanModel>.CreateAsync(query, request.Page, request.Size);

            var mapped = _mapper.Map<List<GetAnnouncementsByUserQueryResponse>>(result);



            return new ApiResponse<List<GetAnnouncementsByUserQueryResponse>>
            {
                IsSuccessfull = true,
                Message = "Kullanıcı ilanları başarıyla getirildi.",
                Data = mapped,
                Meta = new MetaData
                {
                    RequestId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    Pagination = new Pagination
                    {
                        PageNumber = request.Page,
                        PageSize = request.Size,
                        TotalItem = result.TotalItems,
                        TotalPage = (int)Math.Ceiling((double)result.TotalItems / request.Size)
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<GetAnnouncementsByUserQueryResponse>>
            {
                IsSuccessfull = false,
                Message = "Kullanıcı ilanları getirilirken bir hata oluştu: " + ex.Message,
                Data = new List<GetAnnouncementsByUserQueryResponse>()
            };
        }
    }

    private string GetCategoryDisplayName(string category)
    {
        return category?.ToLower() switch
        {
            "konut" => "Konut",
            "isyeri" => "İş Yeri",
            "arsa" => "Arsa",
            "turistiktesis" => "Turistik Tesis",
            "bahce" => "Bahçe",
            "tarla" => "Tarla",
            _ => category ?? "Belirtilmemiş"
        };
    }

    public async Task<ApiResponse<GetAnnouncementDetailsQueryResponse>> GetAnnouncementDetailsAsync(GetAnnouncementDetailsQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Get announcement with related data
            var announcement = await _readRepository.Queryable()
                .Where(i => i.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (announcement == null)
            {
                return new ApiResponse<GetAnnouncementDetailsQueryResponse>
                {
                    IsSuccessfull = false,
                    Message = "İlan bulunamadı.",
                    Data = new GetAnnouncementDetailsQueryResponse
                    {
                        Message = "İlan bulunamadı."
                    }
                };
            }


            var allowedEmails = new List<string>
            {
                "sftumen41@gmail.com", "exdel.txt@gmail.com",
                "burak.tumen@hotmail.com", "fahritumen01@gmail.com",
                "ilkaykoyun4167@gmail.com", "eren.acar.08@gmail.com"
            };

            if (announcement.Status != "active" &&
                !allowedEmails.Contains(request.UserEmail) &&
                request.UserEmail != announcement.Email)
            {
                return new ApiResponse<GetAnnouncementDetailsQueryResponse>
                {
                    IsSuccessfull = false,
                    Message = "Bu ilana erişim yetkiniz bulunmamaktadır.",
                    Data = new GetAnnouncementDetailsQueryResponse
                    {
                        Message = "Bu ilana erişim yetkiniz bulunmamaktadır."
                    }
                };
            }

            var announcementUser = await _readApplicationUserRepository.Queryable()
                .Where(u => u.Email == announcement.Email)
                .FirstOrDefaultAsync(cancellationToken);

            var companyUser = await _readConsultantInvitationRepository.Queryable()
               .Where(u => u.Email == announcement.Email)
               .FirstOrDefaultAsync(cancellationToken);



            #region CompanyInfo


            CompanyDetailsDto companyDetails = null;


            if (announcementUser != null && !announcementUser.IsConsultant) //şirket sahibi
            {

                var consultantEmails = await _readConsultantInvitationRepository.Queryable().
                    Where(i => i.InvitedBy == announcementUser.Id).
                    Select(i => i.Email).ToListAsync(cancellationToken);

                consultantEmails.Add(announcementUser.Email);

                var companyAnnouncementCount = await _readRepository.Queryable()
                    .CountAsync(i => consultantEmails.Contains(i.Email) && i.Status == "active",
                    cancellationToken);

                companyDetails = new CompanyDetailsDto
                {
                    CompanyName = announcementUser.CompanyName,
                    CompanyOwnerProfilePicturePath = announcementUser.ProfilePicturePath,
                    TotalAnnouncementCount = companyAnnouncementCount,
                    Email = announcementUser.Email,
                    RegistrationDate = announcementUser.RegistrationDate,
                    CompanyOwnerId = announcementUser.Id,
                };
            }
            else if (companyUser != null) //danışman
            {

                var companyOwner = await _readApplicationUserRepository.Queryable()
                     .Where(u => u.Id == companyUser.InvitedBy)
                     .FirstOrDefaultAsync(cancellationToken);


                var consultantEmails = await _readConsultantInvitationRepository.Queryable().
                    Where(i => i.InvitedBy == companyOwner.Id).
                    Select(i => i.Email).ToListAsync(cancellationToken);

                consultantEmails.Add(companyOwner.Email);

                var companyAnnouncementCount = await _readRepository.Queryable()
                    .CountAsync(i => consultantEmails.Contains(i.Email) && i.Status == "active",
                    cancellationToken);

                companyDetails = new CompanyDetailsDto
                {
                    CompanyName = companyOwner.CompanyName,
                    CompanyOwnerProfilePicturePath = companyOwner.ProfilePicturePath,
                    TotalAnnouncementCount = companyAnnouncementCount,
                    Email = companyOwner.Email,
                    RegistrationDate = companyOwner.RegistrationDate,
                    CompanyOwnerId = companyOwner.Id,
                };
            }

            #endregion

            #region ConsultantInfo
            ConsultantDto consultant = null;

            if (announcementUser != null)
            {
                consultant = new ConsultantDto
                {
                    FirstName = announcementUser.FirstName,
                    LastName = announcementUser.LastName,
                    ProfilePicturePath = announcementUser.ProfilePicturePath,
                    Email = announcementUser.Email,
                    CompanyName = announcementUser.CompanyName,
                    PhoneNumber = announcementUser.PhoneNumber,
                    IsConsultant = announcementUser.IsConsultant,
                    Id = announcementUser.Id
                };
            }
            #endregion

            #region Comments

            var comments = await _readCommentRepository.Queryable()
               .Where(c => c.IlanId == announcement.Id).Include(a => a.Kullanici)
               .OrderByDescending(c => c.YorumTarihi).Take(5)
               .Select(c => new CommentDto
               {
                   Id = c.Id,
                   Content = c.Yorum,
                   AuthorName = c.Kullanici != null ? c.Kullanici.FirstName + " " + c.Kullanici.LastName : "Anonim Kullanıcı",
                   CommentDate = c.YorumTarihi,
                   IlanId = c.IlanId
               })
               .ToListAsync(cancellationToken);

            #endregion

            #region OtherAnnouncements
            // kullanıcnını diğer ilanları
            var relatedAnnouncements = await _readRepository.Queryable()
                .Where(i => i.Email == announcement.Email && i.Status == "active" && i.Id != request.Id)
                .OrderByDescending(x => x.GirisTarihi)
                .Take(5)
                .Select(i => new RelatedAnnouncementDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Price = i.Price,
                    Area = (decimal)(i.Area > 0 ? i.Area : 0),
                    District = i.semt,
                    City = i.sehir,
                    ImagePath = _photoRepository.Queryable()
                        .Where(p => p.IlanId == i.Id)
                        .Select(p => p.Url)
                        .FirstOrDefault() ?? "/images/Common/default.webp",
                })
                .ToListAsync(cancellationToken);

            #endregion

            var photos = await _photoRepository.Queryable()
               .Where(p => p.IlanId == request.Id)
               .Select(p => p.Url)
               .ToListAsync(cancellationToken);


            var videos = await _videoRepository.Queryable()
                .Where(v => v.IlanId == request.Id)
                .Select(v => v.VideoUrl)
                .ToListAsync(cancellationToken);


            // Increment view count
            announcement.GoruntulenmeSayisi++;

            announcement.GoruntulenmeTarihi = DateTime.Now;

            await _writeRepository.UpdateAsync(announcement);

            await _unitOfWork.CommitAsync(cancellationToken);

            var response = new GetAnnouncementDetailsQueryResponse
            {
                CompanyInfo = companyDetails,
                Announcement = announcement,
                Photos = photos,
                Videos = videos,
                ConsultantInfo = consultant,
                Comments = comments ?? new List<CommentDto>(),
                RelatedAnnouncements = relatedAnnouncements,

                Message = "İlan detayları başarıyla getirildi."
            };

            // Günün ilanı görüntülenmelerini toplam görüntülenmeye ekle
            try
            {
                var today = DateTime.Today;
                var featuredToday = await _readGununIlanRepository.Queryable()
                    .Where(g => g.YayinlanmaTarihi.Date == today && g.Id == announcement.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (featuredToday != null)
                {
                    response.Announcement.GoruntulenmeSayisi = response.Announcement.GoruntulenmeSayisi + featuredToday.GoruntulenmeSayisi;
                }
            }
            catch { }




            return new ApiResponse<GetAnnouncementDetailsQueryResponse>
            {
                IsSuccessfull = true,
                Message = "İlan detayları başarıyla getirildi.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetAnnouncementDetailsQueryResponse>
            {
                IsSuccessfull = false,
                Message = $"İlan detayları getirilirken hata oluştu: {ex.Message}",
                Data = new GetAnnouncementDetailsQueryResponse
                {
                    Message = "İlan detayları getirilirken hata oluştu."
                }
            };
        }
    }

    private string GetStatusDisplayName(string status)
    {
        return status?.ToLower() switch
        {
            "active" => "Aktif",
            "inactive" => "Pasif",
            "deleted" => "Silinmiş",
            "archived" => "Arşivlenmiş",
            "rejected" => "Reddedilmiş",
            _ => status ?? "Belirtilmemiş"
        };
    }
}
