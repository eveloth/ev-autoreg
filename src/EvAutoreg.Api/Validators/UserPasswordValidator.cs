﻿using System.Text.RegularExpressions;
using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class UserPasswordValidator : AbstractValidator<UserPasswordRequest>
{
    public UserPasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .Matches(
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\(\)])[A-Za-z\d@$!%*?&\(\)]{8,}$",
                RegexOptions.Compiled
            );
    }
}