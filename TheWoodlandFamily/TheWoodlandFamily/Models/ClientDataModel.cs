using EFDataAccessLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.Models
{
    public class ClientDataModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public byte HealthCount { get; set; }
        public byte PlayerTurn { get; set; }

        public int RoomId { get; set; }
        public string Wordkey { get; set; }
        public byte PlayerNumber { get; set; }

        public ClientDataModel(Room room, Player player)
        {
            PlayerId = player.Id;
            PlayerName = player.Name;
            HealthCount = player.HealthCount;
            PlayerTurn = player.Turn;

            RoomId = room.Id;
            Wordkey = room.WordKey;
            PlayerNumber = room.PlayerNumber;
        }
    }
}
