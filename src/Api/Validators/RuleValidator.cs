using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class RuleValidator : AbstractValidator<RuleRequest>
{
    public RuleValidator()
    {
        RuleFor(x => x.RuleSubstring).NotNull().NotEmpty().MaximumLength(256);
    }
}