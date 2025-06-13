using FluentValidation;
using MediatR;

namespace Application.Mediator
{
    public class MediatorRequest<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public MediatorRequest(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public  Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators.Select(v => v.Validate(context))
                                      .SelectMany(v => v.Errors)
                                      .Where(v => v != null)
                                      .ToList();

            if (failures.Any())
                throw new FluentValidation.ValidationException(failures);

            return next();
            //if (_validators.Any())
            //{
            //    var context = new ValidationContext<TRequest>(request);
            //    var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            //    var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            //    if (failures.Count != 0)
            //    {
            //        var message = failures.Select(x => $"{x.PropertyName}" + ": " + x.ErrorMessage).FirstOrDefault();
            //        throw new FluentValidation.ValidationException(message);
            //    }
            //}
            //return await next();
        }
    
    }
}
