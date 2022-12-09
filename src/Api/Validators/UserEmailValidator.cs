using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class UserEmailValidator : AbstractValidator<UserEmailRequest>
{
    public UserEmailValidator()
    {
        RuleFor(x => x.NewEmail).NotNull().NotEmpty().EmailAddress();
    }
}