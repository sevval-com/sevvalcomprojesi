using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Abstractions.Services;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Favorite.Commands.AddFavorite;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;

namespace Sevval.Infrastructure.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IReadRepository<IlanModel> _readIlanModelRepository;
        private readonly IWriteRepository<IlanModel> _writeIlanModelRepository; // <IlanModel>
        private readonly IWriteRepository<HaftalikBegeniler> _writeHaftalikBegenilerRepository;
        private readonly IReadRepository<HaftalikBegeniler> _readHaftalikBegenilerRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FavoriteService(IReadRepository<IlanModel> readIlanModelRepository, IWriteRepository<HaftalikBegeniler> writeHaftalikBegenilerRepository, IReadRepository<HaftalikBegeniler> readHaftalikBegenilerRepository, IUnitOfWork unitOfWork, IWriteRepository<IlanModel> writeIlanModelRepository)
        {
            _readIlanModelRepository = readIlanModelRepository;
            _writeHaftalikBegenilerRepository = writeHaftalikBegenilerRepository;
            _readHaftalikBegenilerRepository = readHaftalikBegenilerRepository;
            _unitOfWork = unitOfWork;
            _writeIlanModelRepository = writeIlanModelRepository;
        }



        public async Task<ApiResponse<AddFavoriteCommandResponse>> AddFavoriteAsync(AddFavoriteCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // İlanı bul
                var ilan = await _readIlanModelRepository.Queryable().FirstOrDefaultAsync(i => i.Id == request.AnnouncementId, cancellationToken);

                if (ilan == null)
                {
                    return new ApiResponse<AddFavoriteCommandResponse>
                    {
                        IsSuccessfull = false,
                        Message = "İlan bulunamadı.",
                        Data = new AddFavoriteCommandResponse
                        {
                            IsSuccessful = false,
                            Message = "İlan bulunamadı.",
                            AnnouncementId = request.AnnouncementId
                        }
                    };
                }

                // HaftalikBegeniler tablosundan kullanıcının mevcut haftalık favori verisini al
                var haftalikBegeniler = await _readHaftalikBegenilerRepository.Queryable()
                    .FirstOrDefaultAsync(h => h.UserEmail == request.UserEmail, cancellationToken);

                if (haftalikBegeniler == null)
                {
                    // Kullanıcının haftalık favori kaydı yoksa yeni bir tane oluştur
                    haftalikBegeniler = new HaftalikBegeniler
                    {
                        UserEmail = request.UserEmail,
                        Pazar = 0,
                        Pazartesi = 0,
                        Sali = 0,
                        Carsamba = 0,
                        Persembe = 0,
                        Cuma = 0,
                        Cumartesi = 0,
                        Toplam = 0
                    };
                    await _writeHaftalikBegenilerRepository.AddAsync(haftalikBegeniler);
                }

                // Haftanın mevcut gününü al
                var today = DateTime.Now.DayOfWeek;
                var dayName = GetTurkishDayName(today);

                // Bugünün gününe göre haftalık begeni sayısını artır
                switch (today)
                {
                    case DayOfWeek.Monday:
                        haftalikBegeniler.Pazartesi += 1;
                        break;
                    case DayOfWeek.Tuesday:
                        haftalikBegeniler.Sali += 1;
                        break;
                    case DayOfWeek.Wednesday:
                        haftalikBegeniler.Carsamba += 1;
                        break;
                    case DayOfWeek.Thursday:
                        haftalikBegeniler.Persembe += 1;
                        break;
                    case DayOfWeek.Friday:
                        haftalikBegeniler.Cuma += 1;
                        break;
                    case DayOfWeek.Saturday:
                        haftalikBegeniler.Cumartesi += 1;
                        break;
                    case DayOfWeek.Sunday:
                        haftalikBegeniler.Pazar += 1;
                        break;
                }

                // Toplam favoriyi güncelle
                haftalikBegeniler.Toplam += 1;

                // Favori sayısını artır
                ilan.FavoriSayisi += 1;



                await _writeHaftalikBegenilerRepository.UpdateAsync(haftalikBegeniler);

                await _writeIlanModelRepository.UpdateAsync(ilan);

                // Değişiklikleri kaydet
                await _unitOfWork.CommitAsync(cancellationToken);



                return new ApiResponse<AddFavoriteCommandResponse>
                {
                    IsSuccessfull = true,
                    Message = "İlan favorilere başarıyla eklendi.",
                    Data = new AddFavoriteCommandResponse
                    {
                        IsSuccessful = true,
                        Message = "İlan favorilere başarıyla eklendi.",
                        AnnouncementId = request.AnnouncementId,
                        NewFavoriteCount = ilan.FavoriSayisi,
                        WeeklyFavoriteCount = haftalikBegeniler.Toplam,
                        DayOfWeek = dayName
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AddFavoriteCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = $"Favori eklenirken bir hata oluştu: {ex.Message}",
                    Data = new AddFavoriteCommandResponse
                    {
                        IsSuccessful = false,
                        Message = ex.Message,
                        AnnouncementId = request.AnnouncementId
                    }
                };
            }
        }

        private string GetTurkishDayName(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Pazartesi",
                DayOfWeek.Tuesday => "Salı",
                DayOfWeek.Wednesday => "Çarşamba",
                DayOfWeek.Thursday => "Perşembe",
                DayOfWeek.Friday => "Cuma",
                DayOfWeek.Saturday => "Cumartesi",
                DayOfWeek.Sunday => "Pazar",
                _ => "Bilinmeyen"
            };
        }
    }
}
