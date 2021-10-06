using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataAccessLibrary.Entities
{
    public class Card
    {
        public int Id { get; set; }
        public int DeckId { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
    }
}
