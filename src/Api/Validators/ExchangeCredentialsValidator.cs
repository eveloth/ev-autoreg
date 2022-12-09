using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class ExchangeCredentialsValidator : AbstractValidator<ExchangeCredentialsRequest>
{
    public ExchangeCredentialsValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
    }
}