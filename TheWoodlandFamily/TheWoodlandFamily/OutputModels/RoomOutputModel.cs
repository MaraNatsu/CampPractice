using EFDataAccessLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.OutputModels
{
    public class RoomOutputModel
    {
        public int RoomId { get; set; }
        public int PlayerId { get; set; }
        public byte HealthCount { get; set; }
        public byte PlayerTurn { get; set; }

        public RoomOutputModel(Room room, Player player)
        {
            this.RoomId = room.Id;
            this.PlayerId = player.Id;
            this.HealthCount = player.HealthCount;
            this.PlayerTurn = player.Turn;
        }
    }
}
