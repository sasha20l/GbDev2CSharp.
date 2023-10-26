﻿using CloneHabr.Dto.Requests;
using FluentValidation;

namespace CloneHabrService.Models.Validators
{
    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
    {
        public AuthenticationRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotNull()
                .Length(6, 255);

            RuleFor(x => x.Password)
                .NotNull()
                .Length(5, 30);

        }
    }
}
