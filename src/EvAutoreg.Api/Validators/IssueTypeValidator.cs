using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class IssueTypeValidator : AbstractValidator<IssueTypeRequest>
{
    public IssueTypeValidator()
    {
        RuleFor(x => x.IssueTypeName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(32)
            .Must(x => x.All(char.IsLetterOrDigit));
    }
}