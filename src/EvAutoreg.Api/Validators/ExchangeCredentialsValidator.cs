﻿using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class ExchangeCredentialsValidator : AbstractValidator<ExternalCredentialsRequest>
{
    public ExchangeCredentialsValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
    }
}