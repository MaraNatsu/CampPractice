using EFDataAccessLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.OutputModels
{
    public class PlayerOutputModel
    {
        public int RoomId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public byte HealthCount { get; set; }
        public byte PlayerTurn { get; set; }

        public PlayerOutputModel(Room room, Player player)
        {
            RoomId = room.Id;
            PlayerId = player.Id;
            PlayerName = player.Name;
            HealthCount = player.HealthCount;
            PlayerTurn = player.Turn;
        }
    }
}
