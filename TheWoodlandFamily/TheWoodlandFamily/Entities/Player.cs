using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public byte Turn { get; set; }
        public byte HealthCount { get; set; }
    }
}
