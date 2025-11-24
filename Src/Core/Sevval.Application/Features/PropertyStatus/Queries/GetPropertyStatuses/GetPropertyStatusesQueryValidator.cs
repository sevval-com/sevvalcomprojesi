using FluentValidation;

namespace Sevval.Application.Features.PropertyStatus.Queries.GetPropertyStatuses;

public class GetPropertyStatusesQueryValidator : AbstractValidator<GetPropertyStatusesQueryRequest>
{
    public GetPropertyStatusesQueryValidator()
    {
        // Bu query için özel validasyon kuralı gerekmiyor
        // Çünkü parametre almıyor
    }
}
