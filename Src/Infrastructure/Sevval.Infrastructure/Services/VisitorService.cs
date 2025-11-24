using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount;
using Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities;
using System.Net;

namespace Sevval.Infrastructure.Services;

public class VisitorService : IVisitorService
{
    private readonly IWriteRepository<Visitor> _writeRepository;
    private readonly IReadRepository<Visitor> _readRepository;
    private readonly IWriteRepository<VisitorCount> _writeVisitorCountRepository;
    private readonly IReadRepository<VisitorCount> _readVisitorCountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VisitorService(IWriteRepository<Visitor> writeRepository, IReadRepository<Visitor> readRepository, IWriteRepository<VisitorCount> writeVisitorCountRepository, IReadRepository<VisitorCount> readVisitorCountRepository, IUnitOfWork unitOfWork)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _writeVisitorCountRepository = writeVisitorCountRepository;
        _readVisitorCountRepository = readVisitorCountRepository;
        _unitOfWork = unitOfWork;
    }



    public async Task<ApiResponse<GetActiveVisitorCountQueryResponse>> GetActiveVisitorCountAsync(GetActiveVisitorCountQueryRequest request)
    {

        //var threeDaysAgo = DateTime.Now.AddDays(-1);
        var threeDaysAgo = DateTime.Now.AddDays(-3); //TODO şimdilik 3 gün olarak ayarlandı.Front-end canlıya alınınca değişecek
        var activeVisitorCount = await _readRepository.Queryable()
            .Where(v => v.VisitTime > threeDaysAgo)


            .CountAsync();

        return new ApiResponse<GetActiveVisitorCountQueryResponse>
        {
            Data = new GetActiveVisitorCountQueryResponse
            {
                ActiveVisitorCount = activeVisitorCount
            },
            IsSuccessfull = true,
            Message = "Aktif ziyaretçi sayısı başarıyla getirildi."
        };
    }

    public async Task<ApiResponse<GetTotalVisitorCountQueryResponse>> GetTotalVisitorCountAsync(GetTotalVisitorCountQueryRequest request)
    {
        var visitorCount = await _readVisitorCountRepository.Queryable().Select(a => a.TotalVisitors)
            .FirstOrDefaultAsync();

        return new ApiResponse<GetTotalVisitorCountQueryResponse>
        {
            Code = (int)HttpStatusCode.OK,
            Data = new GetTotalVisitorCountQueryResponse
            {
                TotalVisitorCount = visitorCount
            },
            IsSuccessfull = true,
            Message = "Toplam ziyaretçi sayısı başarıyla getirildi."
        };
    }

    /* public async Task AddVisitorAsync(string ipAddress)
     {
         var currentTime = DateTime.UtcNow;

         // IP adresi kontrolü
         var existingVisitor = await DbContext.Visitors
             .FirstOrDefaultAsync(v => v.IpAddress == ipAddress);

         if (existingVisitor == null)
         {
             // Yeni ziyaretçi kaydı oluştur
             var visitor = new Visitor
             {
                 IpAddress = ipAddress,
                 VisitTime = currentTime
             };
             DbContext.Visitors.Add(visitor);

             // Toplam ziyaretçi sayısını artır
             var visitorCount = await DbContext.VisitorCounts.FirstOrDefaultAsync();
             if (visitorCount == null)
             {
                 visitorCount = new VisitorCount { TotalVisitors = 1, ActiveVisitors = 1 };
                 DbContext.VisitorCounts.Add(visitorCount);
             }
             else
             {
                 visitorCount.TotalVisitors += 1;
                 visitorCount.ActiveVisitors += 1;
             }

             await DbContext.SaveChangesAsync();
         }
     }

     public async Task RemoveVisitorAsync(string ipAddress)
     {
         var visitor = await DbContext.Visitors
             .FirstOrDefaultAsync(v => v.IpAddress == ipAddress);

         if (visitor != null)
         {
             DbContext.Visitors.Remove(visitor);

             // Aktif ziyaretçi sayısını azalt
             var visitorCount = await DbContext.VisitorCounts.FirstOrDefaultAsync();
             if (visitorCount != null)
             {
                 visitorCount.ActiveVisitors -= 1;
             }

             await DbContext.SaveChangesAsync();
         }
     }

     */

    public async Task<ApiResponse<IncreaseVisitorCountCommandResponse>> IncreaseVisitorCountAsync(IncreaseVisitorCountCommandRequest request, CancellationToken cancellationToken)
    {

        var currentTime = DateTime.UtcNow;

        Visitor visitor = _readRepository.Queryable().FirstOrDefault(v => v.IpAddress == request.IpAddress);

        if (visitor is null)
        {
            visitor = new Visitor
            {
                IpAddress = request.IpAddress ?? "Unknown",
                VisitTime = currentTime
            };
            await _writeRepository.AddAsync(visitor);

        }








        // Toplam ziyaretçi sayısını artır
        var visitorCount = await _readVisitorCountRepository.Queryable().FirstOrDefaultAsync(cancellationToken);

        if (visitorCount == null)
        {
            visitorCount = new VisitorCount { TotalVisitors = 1 };
            await _writeVisitorCountRepository.AddAsync(visitorCount);
        }
        else
        {
            visitorCount.TotalVisitors += 1;
            await _writeVisitorCountRepository.UpdateAsync(visitorCount);
        }

        if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
        {
            return new ApiResponse<IncreaseVisitorCountCommandResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = new IncreaseVisitorCountCommandResponse
                {
                    IsSuccessful = true,
                    Message = "Ziyaretçi sayısı başarıyla artırıldı.",
                    NewVisitorCount = visitorCount.TotalVisitors
                },
                IsSuccessfull = true,
                Message = "Ziyaretçi sayısı başarıyla artırıldı."
            };
        }



        return new ApiResponse<IncreaseVisitorCountCommandResponse>
        {
            Code = (int)HttpStatusCode.InternalServerError,
            Data = new IncreaseVisitorCountCommandResponse
            {
                IsSuccessful = false,
                Message = "Ziyaretçi sayısı artırılırken hata oluştu.",
                NewVisitorCount = 0
            },
            IsSuccessfull = false,
            Message = $"Hata"
        };

    }

    public async Task<ApiResponse<DecreaseVisitorCountCommandResponse>> DecreaseVisitorCountAsync(DecreaseVisitorCountCommandRequest request, CancellationToken cancellationToken)
    {

        // IP adresine göre en son ziyaretçi kaydını bul ve sil
        if (!string.IsNullOrEmpty(request.IpAddress))
        {
            var visitor = await _readRepository.Queryable()
                .FirstOrDefaultAsync(v => v.IpAddress == request.IpAddress);


            if (visitor != null)
            {
                await _writeRepository.DeleteAsync(visitor);
            }
        }

        // Toplam ziyaretçi sayısını azalt
        var visitorCount = await _readVisitorCountRepository.Queryable().FirstOrDefaultAsync(cancellationToken);
        if (visitorCount != null && visitorCount.TotalVisitors > 0)
        {
            visitorCount.TotalVisitors -= 1;
            await _writeVisitorCountRepository.UpdateAsync(visitorCount);
        }



        if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
        {
            return new ApiResponse<DecreaseVisitorCountCommandResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = new DecreaseVisitorCountCommandResponse
                {
                    IsSuccessful = true,
                    Message = "Ziyaretçi sayısı başarıyla azaltıldı.",
                    NewVisitorCount = visitorCount?.TotalVisitors ?? 0
                },
                IsSuccessfull = true,
                Message = "Ziyaretçi sayısı başarıyla azaltıldı."
            };
        }

        return new ApiResponse<DecreaseVisitorCountCommandResponse>
        {
            Code = (int)HttpStatusCode.InternalServerError,
            Data = new DecreaseVisitorCountCommandResponse
            {
                IsSuccessful = false,
                Message = "Ziyaretçi sayısı azaltılırken hata oluştu.",
                NewVisitorCount = 0
            },
            IsSuccessfull = false,
            Message = $"Hata"
        };

    }
}