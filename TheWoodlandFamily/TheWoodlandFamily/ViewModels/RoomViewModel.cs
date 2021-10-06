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
        byte FirstPlayerTurn { get; set; }

        public RoomViewModel(Room room, Player firstPlayer)
        {
            this.RoomId = room.Id;
            this.PlayerId = firstPlayer.Id;
            this.HealthCount = firstPlayer.HealthCount;
            this.FirstPlayerTurn = firstPlayer.Turn;
        }
    }
}
