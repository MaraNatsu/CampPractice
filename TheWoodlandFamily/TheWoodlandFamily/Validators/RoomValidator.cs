using EFDataAccessLibrary.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.Validators
{
    public class RoomValidator : AbstractValidator<Room>
    {
        public RoomValidator()
        {
            RuleFor(room => (int)room.PlayerNumber).InclusiveBetween(2, 5);
            RuleFor(room => room.WordKey).NotEmpty().Length(4, 8);
        }
    } 
}
