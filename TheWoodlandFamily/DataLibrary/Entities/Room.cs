using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataAccessLibrary.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string WordKey { get; set; }
        public int PlayerNumber { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Card> Deck { get; set; } = new List<Card>();
    }
}
