using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class AutoregistrarSettingsValidator : AbstractValidator<AutoregistrarSettingsRequest>
{
    public AutoregistrarSettingsValidator()
    {
        RuleFor(x => x.ExchangeServerUri).NotNull().NotEmpty().MaximumLength(256);
        RuleFor(x => x.ExtraViewUri).NotNull().NotEmpty().MaximumLength(256);
        RuleFor(x => x.NewIssueRegex).NotNull().NotEmpty().MaximumLength(256);
        RuleFor(x => x.IssueNoRegex).NotNull().NotEmpty().MaximumLength(256);
    }
}