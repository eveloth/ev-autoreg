using System.Text.RegularExpressions;
using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class UserCredentialsValidator : AbstractValidator<UserCredentialsRequest>
{
    public UserCredentialsValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();

        RuleFor(x => x.Password)
            .Matches(
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\(\)])[A-Za-z\d@$!%*?&\(\)]{8,}$",
                RegexOptions.Compiled
            );
    }
}