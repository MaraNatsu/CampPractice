using EFDataAccessLibrary.Entities;
using FluentValidation;

namespace ValidationLibrary.Validators
{
    public class RoomValidator : AbstractValidator<Room>
    {
        public RoomValidator()
        {
            RuleFor(room => room.WordKey)
                .NotEmpty()
                .WithMessage("The word key is required")
                .MinimumLength(4)
                .WithMessage("Your word key is too short")
                .MaximumLength(8)
                .WithMessage("Your word key is too long");

            RuleFor(room => (int)room.PlayerNumber)
                .InclusiveBetween(2, 5);
        }
    }
}
