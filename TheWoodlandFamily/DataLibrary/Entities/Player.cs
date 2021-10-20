using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFDataAccessLibrary.Entities
{
    public class Player
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Name { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public byte Turn { get; set; }

        [Required]
        public byte HealthCount { get; set; }

        public Room Room { get; set; }
    }
}
