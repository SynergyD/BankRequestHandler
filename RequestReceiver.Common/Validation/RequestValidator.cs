using FluentValidation;
using RequestReceiver.Common.Models;

namespace RequestReceiver.Common.Validation
{
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(a => a.Amount)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}