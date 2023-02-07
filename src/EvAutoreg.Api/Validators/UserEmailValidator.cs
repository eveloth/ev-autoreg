using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class UserEmailValidator : AbstractValidator<UserEmailRequest>
{
    public UserEmailValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
    }
}