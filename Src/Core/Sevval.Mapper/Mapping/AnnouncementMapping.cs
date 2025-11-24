using AutoMapper;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser;
using Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement;
using Sevval.Domain.Entities;

namespace Sevval.Mapper.Mapping
{
    public class AnnouncementMapping : Profile
    {
        public AnnouncementMapping()
        {
            CreateMap<RecentlyVisitedAnnouncement, GetRecentlyVisitedAnnouncementQueryResponse>()
      .AfterMap((src, dest) => dest.IlanBasligi = src.Ilan.Title)
      .AfterMap((src, dest) => dest.IlanVitrinImageUrl = src.Ilan.ProfilePicture)
      .AfterMap((src, dest) => dest.Category = src.Ilan.Category)
      .AfterMap((src, dest) => dest.KonutDurumu = src.Ilan.KonutDurumu)
      .AfterMap((src, dest) => dest.MulkTipi = src.Ilan.MulkTipi)
      .AfterMap((src, dest) => dest.sehir = src.Ilan.sehir)
      .AfterMap((src, dest) => dest.semt = src.Ilan.semt)
      .AfterMap((src, dest) => dest.mahalleKoy = src.Ilan.mahalleKoy)
      .AfterMap((src, dest) => dest.GirisTarihi = src.Ilan.GirisTarihi)
      .AfterMap((src, dest) => dest.Area = src.Ilan.Area)
      .AfterMap((src, dest) => dest.AdaNo = src.Ilan.AdaNo)
      .AfterMap((src, dest) => dest.ParselNo = src.Ilan.ParselNo)
      .AfterMap((src, dest) => dest.OdaSayisi = src.Ilan.OdaSayisi)
      .AfterMap((src, dest) => dest.BinaYasi = src.Ilan.BinaYasi)
      .AfterMap((src, dest) => dest.NetMetrekare = src.Ilan.NetMetrekare)
      .AfterMap((src, dest) => dest.Id = src.Id)
      .AfterMap((src, dest) => dest.AnnouncementId = src.IlanId)
      .AfterMap((src, dest) => dest.IlanFiyati = src.Ilan.Price)

      ;

            CreateMap<RecentlyVisitedAnnouncement, GetSuitableAnnouncementsQueryResponse>()
      .AfterMap((src, dest) => dest.IlanBasligi = src.Ilan.Title)
      .AfterMap((src, dest) => dest.IlanVitrinImageUrl = src.Ilan.ProfilePicture)
      .AfterMap((src, dest) => dest.Category = src.Ilan.Category)
      .AfterMap((src, dest) => dest.KonutDurumu = src.Ilan.KonutDurumu)
      .AfterMap((src, dest) => dest.MulkTipi = src.Ilan.MulkTipi)
      .AfterMap((src, dest) => dest.sehir = src.Ilan.sehir)
      .AfterMap((src, dest) => dest.semt = src.Ilan.semt)
      .AfterMap((src, dest) => dest.mahalleKoy = src.Ilan.mahalleKoy)
      .AfterMap((src, dest) => dest.GirisTarihi = src.Ilan.GirisTarihi)
      .AfterMap((src, dest) => dest.Area = src.Ilan.Area)
      .AfterMap((src, dest) => dest.AdaNo = src.Ilan.AdaNo)
      .AfterMap((src, dest) => dest.ParselNo = src.Ilan.ParselNo)
      .AfterMap((src, dest) => dest.OdaSayisi = src.Ilan.OdaSayisi)
      .AfterMap((src, dest) => dest.BinaYasi = src.Ilan.BinaYasi)
      .AfterMap((src, dest) => dest.NetMetrekare = src.Ilan.NetMetrekare)
      .AfterMap((src, dest) => dest.Id = src.Id)
      .AfterMap((src, dest) => dest.AnnouncementId = src.IlanId)
      .AfterMap((src, dest) => dest.IlanFiyati = src.Ilan.Price)

      ;



            CreateMap<IlanModel, GetAnnouncementsByCompanyQueryResponse>();


            CreateMap<IlanModel, GetAnnouncementsByUserQueryResponse>();

            
        }
    }
}
