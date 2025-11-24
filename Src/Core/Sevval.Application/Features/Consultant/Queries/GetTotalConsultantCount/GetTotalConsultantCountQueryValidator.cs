using FluentValidation;

namespace Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;

public class GetTotalConsultantCountQueryValidator : AbstractValidator<GetTotalConsultantCountQueryRequest>
{
    public GetTotalConsultantCountQueryValidator()
    {
        RuleFor(x => x.Status)
            .MaximumLength(50)
            .WithMessage("Durum en fazla 50 karakter olabilir.");

        RuleFor(x => x.CompanyName)
            .MaximumLength(200)
            .WithMessage("Şirket adı en fazla 200 karakter olabilir.");
    }
}
