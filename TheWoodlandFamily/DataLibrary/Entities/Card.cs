using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFDataAccessLibrary.Entities
{
    public class Card
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        [Required]
        public byte Order { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public Room Room { get; set; }
    }
}
