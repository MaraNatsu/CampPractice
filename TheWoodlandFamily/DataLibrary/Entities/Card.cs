using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataAccessLibrary.Entities
{
    public class Card
    {
        public int Id { get; set; }
        public int DeckId { get; set; }
        public byte Order { get; set; }
        public string Type { get; set; }
    }
}
