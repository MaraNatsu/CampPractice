using EFDataAccessLibrary.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ValidationLibrary.Validators
{
    public class PlayerValidator : AbstractValidator<Player>
    {
        public PlayerValidator()
        {
            RuleFor(player => player.Name)
                .NotEmpty()
                .WithMessage("The nickname is required")
                .Length(1, 10)
                .WithMessage("Your nickname is too long");
        }
    }
}
