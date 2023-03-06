using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class NewRuleValidator : AbstractValidator<RuleRequest>
{
    public NewRuleValidator()
    {
        RuleFor(x => x.RuleSubstring).NotNull().NotEmpty().MaximumLength(256);
    }
}