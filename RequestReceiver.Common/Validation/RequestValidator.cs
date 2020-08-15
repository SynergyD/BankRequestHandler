using FluentValidation;
using RequestReceiver.Common.Models;

namespace RequestReceiver.Common.Validation
{
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(a => a.Amount)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(d => d.DepartmentAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(c => c.ClientId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0);

            RuleFor(c => c.Currency)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(20);
        }
    }
}