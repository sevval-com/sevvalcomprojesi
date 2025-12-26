using FluentValidation;

namespace Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;

public class GetConsultantsByCompanyQueryValidator : AbstractValidator<GetConsultantsByCompanyQueryRequest>
{
    public GetConsultantsByCompanyQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Sayfa numarası 1'den büyük olmalıdır.");

        RuleFor(x => x.Size)
            .InclusiveBetween(1, 300)
            .WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

    
    }
}
