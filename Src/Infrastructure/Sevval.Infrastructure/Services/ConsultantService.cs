using GridBox.Solar.Domain.IRepositories;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;
using Sevval.Application.Interfaces.Services;
using Sevval.Domain.Entities;
using Sevval.Application.Utilities;
using AutoMapper;

namespace Sevval.Infrastructure.Services;

public class ConsultantService :  IConsultantService
{
    private readonly IReadRepository<ApplicationUser> _readRepository;
    private readonly IReadRepository<ConsultantInvitation> _readConsultantInvitationRepository;
    private readonly IReadRepository<IlanModel> _readAnnouncementRepository;
    private readonly IMapper _mapper;

    public ConsultantService(IReadRepository<ApplicationUser> readRepository, IReadRepository<ConsultantInvitation> readConsultantInvitationRepository, IMapper mapper, IReadRepository<IlanModel> readAnnouncementRepository = null)
    {
        _readRepository = readRepository;
        _readConsultantInvitationRepository = readConsultantInvitationRepository;
        _mapper = mapper;
        _readAnnouncementRepository = readAnnouncementRepository;
    }

    public async Task<ApiResponse<GetTotalConsultantCountQueryResponse>> GetTotalConsultantCountAsync(GetTotalConsultantCountQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var consultantsQuery = _readRepository.Queryable().Where(u => 
                           u.UserTypes == "Kurumsal" || u.UserTypes == "Vakıf"
                           || u.UserTypes == "İnşaat" || u.UserTypes == "Banka");

            if (!string.IsNullOrEmpty(request.Status))
            {
                consultantsQuery = consultantsQuery.Where(u => u.IsActive == request.Status);
            }

            if (!string.IsNullOrEmpty(request.CompanyName))
            {
                consultantsQuery = consultantsQuery.Where(u => u.CompanyName.Contains(request.CompanyName));
            }

            var totalCount = consultantsQuery.Count();

            var response = new GetTotalConsultantCountQueryResponse
            {
                TotalCount = totalCount,
                Status = request.Status,
                CompanyName = request.CompanyName,
                Message = $"Toplam {totalCount} danışman bulundu."
            };

            return new ApiResponse<GetTotalConsultantCountQueryResponse>
            {
                Data = response,
                IsSuccessfull = true,
                Message = "Danışman sayısı başarıyla getirildi.",
                Code = 200
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<GetTotalConsultantCountQueryResponse>
            {
                Data = null,
                IsSuccessfull = false,
                Message = $"Danışman sayısı getirilirken hata oluştu: {ex.Message}",
                Code = 500
            };
        }
    }

    public async Task<ApiResponse<List<GetConsultantsByCompanyQueryResponse>>> GetConsultantsByCompanyAsync(GetConsultantsByCompanyQueryRequest request, CancellationToken cancellationToken)
    {


        var consultants = _readConsultantInvitationRepository.Queryable()
            .Where(ci => ci.InvitedBy == request.UserId);

        var consultantsQuery = _readRepository.Queryable()
            .Where(u => u.Id == request.UserId || consultants.Any(ci => ci.Email == u.Email));

        // Apply optional status filter
        if (!string.IsNullOrEmpty(request.Status))
        {
            consultantsQuery = consultantsQuery.Where(u => u.IsActive == request.Status);
        }

        var result = await PaginatedList<ApplicationUser>.
           CreateAsync(consultantsQuery.OrderByDescending(a => a.Id),
           request.Page, request.Size);


        if (result.TotalItems == 0)
        {
            return new ApiResponse<List<GetConsultantsByCompanyQueryResponse>>
            {
                Code = 404,
                IsSuccessfull = false,
                Message = "Şirket bulunamadı.",
                Data = new List<GetConsultantsByCompanyQueryResponse>()
            };
        }

        var mapped = _mapper.Map<List<GetConsultantsByCompanyQueryResponse>>(result);

        foreach (var consultant in mapped)
        {
            consultant.TotalAnnouncementCount =
               _readAnnouncementRepository.Queryable().Where(a => a.Email == consultant.Email
               && a.Status == "active").Count();
        }

        return new ApiResponse<List<GetConsultantsByCompanyQueryResponse>>
        {
            Data = mapped,
            IsSuccessfull = true,
            Message = "Şirket danışmanları başarıyla getirildi.",
            Code = 200
        };

    }
}
