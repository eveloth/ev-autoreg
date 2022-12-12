using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class EvCredentialsValidator : AbstractValidator<ExternalCredentialsRequest>
{
    public EvCredentialsValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
    }
}