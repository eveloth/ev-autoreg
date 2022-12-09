﻿using System.Text.RegularExpressions;
using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class UserProfileValidator : AbstractValidator<UserProfileRequest>
{
    public UserProfileValidator()
    {
        RuleFor(profile => profile.FisrtName)
            .NotNull()
            .NotEmpty()
            .Length(min: 2, max: 64)
            .Matches("^[a-z ,.'-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        RuleFor(profile => profile.LastName)
            .NotNull()
            .NotEmpty()
            .Length(min: 2, max: 64)
            .Matches("^[a-z ,.'-]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}