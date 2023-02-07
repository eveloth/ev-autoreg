using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class EvCredentialsValidator : AbstractValidator<ExternalCredentialsRequest>
{
    public EvCredentialsValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
    }
}