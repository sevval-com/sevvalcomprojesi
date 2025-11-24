using AutoMapper;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;
using Sevval.Domain.Entities;

namespace Sevval.Mapper.Mapping
{
    public class ConsultantMapping : Profile
    {
        public ConsultantMapping()
        {
            CreateMap<ApplicationUser, GetConsultantsByCompanyQueryResponse>()
      ;


        }
    }
}
