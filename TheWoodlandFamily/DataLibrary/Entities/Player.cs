using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFDataAccessLibrary.Entities
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
