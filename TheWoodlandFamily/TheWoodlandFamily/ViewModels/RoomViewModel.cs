using EFDataAccessLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.ViewModels
{
    public class RoomViewModel
    {
        int RoomId { get; set; }
        int PlayerId { get; set; }
        byte HealthCount { get; set; }
        byte PlayerTurn { get; set; }

        public RoomViewModel(Room room, Player player)
        {
            this.RoomId = room.Id;
            this.PlayerId = player.Id;
            this.HealthCount = player.HealthCount;
            this.PlayerTurn = player.Turn;
        }
    }
}
