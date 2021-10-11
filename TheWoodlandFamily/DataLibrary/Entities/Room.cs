using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EFDataAccessLibrary.Entities
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(8)]
        public string WordKey { get; set; }

        [Required]
        public byte PlayerNumber { get; set; }

        [Required]
        public List<Player> Players { get; set; } = new List<Player>();

        [Required]
        public List<Card> Deck { get; set; } = new List<Card>();
    }
}
