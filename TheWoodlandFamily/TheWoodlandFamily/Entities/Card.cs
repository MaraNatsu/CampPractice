using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.Entities
{
    public class Card
    {
        public int Id { get; set; }
        public int DeckId { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
    }
}
