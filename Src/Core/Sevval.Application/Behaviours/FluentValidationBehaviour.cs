using FluentValidation;
using MediatR;


namespace Sevval.Application.Behaviours;

public class FluentValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private IEnumerable<IValidator<TRequest>> Validator;

    public FluentValidationBehaviour(IEnumerable<IValidator<TRequest>> validator)
    {
        Validator = validator;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failtures = Validator.Select(a => a.Validate(context))
            .SelectMany(a => a.Errors)
            .GroupBy(a => a.ErrorMessage)
            .Select(a => a.FirstOrDefault()).Where(s => s != null).ToList();

        if (failtures.Any())
        {
            throw new ValidationException(failtures);
        }

        return next();
    }
}
