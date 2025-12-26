using AutoMapper;
using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement;
using Sevval.Application.Interfaces.Services;
using Sevval.Application.Utilities;
using Sevval.Domain.Entities;

namespace Sevval.Infrastructure.Services
{
    public class RecentlyVisitedAnnouncementService : IRecentlyVisitedAnnouncementService
    {
        private readonly IReadRepository<RecentlyVisitedAnnouncement> _readRepository;
        private readonly IReadRepository<PhotoModel> _readPhotoModelRepository;
        private readonly IWriteRepository<RecentlyVisitedAnnouncement> _writeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RecentlyVisitedAnnouncementService(IReadRepository<RecentlyVisitedAnnouncement> readRepository,
            IWriteRepository<RecentlyVisitedAnnouncement> writeRepository, IUnitOfWork unitOfWork, IMapper mapper, IReadRepository<PhotoModel> readPhotoModelRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _readPhotoModelRepository = readPhotoModelRepository;
        }


        public async Task<ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>> GetRecentlyVisitedAnnouncementAsync(GetRecentlyVisitedAnnouncementQueryRequest request, CancellationToken cancellationToken)
        {

            var recentlyVisiteds = _readRepository.Queryable().Where(x => x.UserId == request.UserId);



            var result = await PaginatedList<RecentlyVisitedAnnouncement>
                .CreateAsync(recentlyVisiteds.OrderByDescending(a => a.Id), request.Page, request.Size,
           a => a.Include(a => a.Ilan));

            

            if (result.TotalItems == 0)
            {
                return new ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>
                {
                    Code = 404,
                    IsSuccessfull = false,
                    Message = "Son gezilen ilan bulunamadı.",
                    Data = new List<GetRecentlyVisitedAnnouncementQueryResponse>()
                };
            }



            var mapped = _mapper.Map<List<GetRecentlyVisitedAnnouncementQueryResponse>>(result);

            foreach (var item in mapped)
            {
                if (string.IsNullOrEmpty(item.IlanVitrinImageUrl))
                {
                    item.IlanVitrinImageUrl = _readPhotoModelRepository.Queryable().FirstOrDefault(a=>a.IlanId==item.AnnouncementId)?.Url;
                }
                
            }

            return new ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>
            {
                Code = 200,
                IsSuccessfull = true,
                Message = "Son gezilen ilanlar başarıyla getirildi.",
                Data = mapped
            };


        }

        public async Task<ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>> AddRecentlyVisitedAnnouncementAsync(AddRecentlyVisitedAnnouncementCommandRequest request, CancellationToken cancellationToken)
        {

            var recentlyVisitedAnnouncementExist = _readRepository.Any(x => x.UserId == request.UserId && x.IlanId == request.AnnouncementId);

            if (!recentlyVisitedAnnouncementExist)
            {

                var recentlyVisitedAnnouncement = new RecentlyVisitedAnnouncement
                {
                    UserId = request.UserId,
                    IlanId = request.AnnouncementId,
                    Property = request.Property,
                    Province = request.Province,
                    VisitedAt = DateTime.Now,

                };


                await _writeRepository.AddAsync(recentlyVisitedAnnouncement);

                if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
                {
                    return new ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>
                    {
                        Code = 200,
                        IsSuccessfull = true,
                        Message = "Son gezilen ilan eklendi.",
                        Data = new AddRecentlyVisitedAnnouncementCommandResponse
                        {
                            IsSuccessful = true,
                            Message = "Son gezilen ilan eklendi.",
                            AnnouncementId = request.AnnouncementId,
                            VisitedAt = DateTime.Now
                        }
                    };
                }


                return new ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>
                {
                    Code = 500,
                    IsSuccessfull = false,
                    Message = "Son gezilen ilan eklenirken bir hata oluştu",
                    Data = null
                };



            }

            return new ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>
            {
                Code = 200,
                IsSuccessfull = true,
                Message = "Son gezilen ilan eklendi.",
                Data = new AddRecentlyVisitedAnnouncementCommandResponse
                {
                    IsSuccessful = true,
                    Message = "Son gezilen ilan eklendi.",
                    AnnouncementId = request.AnnouncementId,
                    VisitedAt = DateTime.Now
                }
            };

        }

        public async Task<ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>> GetSuitableAnnouncementsAsync(GetSuitableAnnouncementsQueryRequest request, CancellationToken cancellationToken)
        {
            var recentlyVisiteds = _readRepository.Queryable()
    .Include(a => a.Ilan)
    .Where(x => x.UserId == request.UserId && x.Province != null && x.Property != null)
    .AsQueryable() // Switch to client evaluation
    .GroupBy(x => new { x.Property, x.Province })
    .Select(x => x.First());


           
          
            var result = await PaginatedList<RecentlyVisitedAnnouncement>.CreateAsync(
                recentlyVisiteds, request.Page, request.Size);


            if (result.TotalItems == 0)
            {
                return new ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>
                {
                    Code = 404,
                    IsSuccessfull = false,
                    Message = "Son gezilen ilan bulunamadı.",
                    Data = new List<GetSuitableAnnouncementsQueryResponse>()
                };
            }


            if (result.TotalItems < request.Size)
            {
                recentlyVisiteds = _readRepository.Queryable().Where(x => x.UserId == request.UserId);

                result = await PaginatedList<RecentlyVisitedAnnouncement>
                    .CreateAsync(recentlyVisiteds.OrderByDescending(a => a.Id), request.Page, request.Size,
                        a => a.Include(a => a.Ilan));
            }


            var mapped = _mapper.Map<List<GetSuitableAnnouncementsQueryResponse>>(result);

            foreach (var item in mapped)
            {
                if (string.IsNullOrEmpty(item.IlanVitrinImageUrl))
                {
                    item.IlanVitrinImageUrl = _readPhotoModelRepository.Queryable().FirstOrDefault(a => a.IlanId == item.AnnouncementId)?.Url;
                }

            }

            return new ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>
            {
                Code = 200,
                IsSuccessfull = true,
                Message = "Son gezilen ilanlar başarıyla getirildi.",
                Data = mapped
            };
        }
    }
}
