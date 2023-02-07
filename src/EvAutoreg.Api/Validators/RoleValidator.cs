using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class RoleValidator : AbstractValidator<RoleRequest>
{
    public RoleValidator()
    {
        RuleFor(x => x.RoleName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(64)
            .Must(x => x.All(char.IsLetterOrDigit));
    }
}