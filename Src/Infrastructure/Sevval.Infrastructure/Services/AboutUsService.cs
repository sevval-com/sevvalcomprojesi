using AutoMapper;
using GridBox.Solar.Domain.IRepositories;
using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;
using Sevval.Domain.Entities;
using System.Net;

namespace Sevval.Infrastructure.Services
{
    public class AboutUsService : IAboutUsService
    {
        private readonly IReadRepository<ApplicationUser> _readRepository;
        private readonly IReadRepository<IlanModel> _readAnnouncementRepository;
        private readonly IReadRepository<ConsultantInvitation> _readConsultantInvitationRepository;
        private readonly IMapper _mapper;

        public AboutUsService(IReadRepository<ApplicationUser> readRepository, IMapper mapper, IReadRepository<IlanModel> readAnnouncementRepository, IReadRepository<ConsultantInvitation> readConsultantInvitationRepository)
        {
            _readRepository = readRepository;
            _mapper = mapper;
            _readAnnouncementRepository = readAnnouncementRepository;
            _readConsultantInvitationRepository = readConsultantInvitationRepository;
        }

        public async Task<ApiResponse<GetAboutUsQueryResponse>> GetAboutUsAsync(GetAboutUsQueryRequest request, CancellationToken cancellationToken)
        {
             
            var aboutUs = await _readRepository.GetAsync(u => u.Id == request.UserId);

            if (aboutUs == null)
            {
                return new ApiResponse<GetAboutUsQueryResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kayıt Bulunamadı"
                };
            }


            var mapped = _mapper.Map<GetAboutUsQueryResponse>(aboutUs);
            // Belgeleri aktar
            mapped.Level5CertificatePath = aboutUs.Level5CertificatePath;
            mapped.TaxPlatePath = aboutUs.TaxPlatePath;
            mapped.BannerPicturePath = aboutUs.BannerPicturePath;

            // Count active announcements for company owner + invited consultants
            var consultantEmails = _readConsultantInvitationRepository.Queryable()
                .Where(ci => ci.InvitedBy == aboutUs.Id.ToString())
                .Select(ci => ci.Email)
                .ToList();

            consultantEmails.Add(aboutUs.Email);

            mapped.TotalAnnouncementCount = _readAnnouncementRepository.Queryable()
                .Where(x => consultantEmails.Contains(x.Email) && x.Status == "active")
                .Count();

            return new ApiResponse<GetAboutUsQueryResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = mapped,
                IsSuccessfull = true,
                Message = "Kayıt Getirildi"
            };

        }
    }
}
