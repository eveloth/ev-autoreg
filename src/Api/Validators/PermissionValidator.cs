using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class PermissionValidator : AbstractValidator<PermissionRequest>
{
    public PermissionValidator()
    {
        RuleFor(x => x.PermissionName).NotNull().NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(256);
    }
}