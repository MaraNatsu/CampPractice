using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TheWoodlandFamily.Models;

namespace TheWoodlandFamily.Services
{
    public class PlayerNumberChecker
    {
        public byte TotalPlayerNumber { get; private set; }
        public byte JoinedPlayerNumber { get; private set; }


        public bool CheckIfAllPlayersConnected(Room room, List<ConnectionDataModel> playerSockets)
        {
            TotalPlayerNumber = room.PlayerNumber;
            JoinedPlayerNumber = DefineConnectedPlayers(room, playerSockets);

            return TotalPlayerNumber == JoinedPlayerNumber;
        }

        private byte DefineConnectedPlayers(Room room, List<ConnectionDataModel> playerSockets)
        {
            byte connectedPlayers = 0;

            foreach (var player in room.Players)
            {
                if (playerSockets.Select(socket => socket.PlayerId).ToList().Contains(player.Id))
                {
                    connectedPlayers++;
                }
            }
            return connectedPlayers;
        }
    }
}
