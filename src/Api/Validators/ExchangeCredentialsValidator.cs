using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class ExchangeCredentialsValidator : AbstractValidator<ExternalCredentialsRequest>
{
    public ExchangeCredentialsValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
    }
}