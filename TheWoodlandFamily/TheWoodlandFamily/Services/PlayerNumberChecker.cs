using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.Controllers;

namespace TheWoodlandFamily.Services
{
    public class PlayerNumberChecker
    {
        public byte TotalPlayerNumber { get; private set; }
        public byte JoinedPlayerNumber { get; private set; }


        public bool CheckIfAllPlayersConnected(string wordKey, GameContext dbContext, Dictionary<int, string> playerSockets)
        {
            TotalPlayerNumber = DefineTotalPlayerNumber(dbContext, wordKey);
            JoinedPlayerNumber = DefineConnectedPlayers(dbContext, wordKey, playerSockets);

            return TotalPlayerNumber == JoinedPlayerNumber;
        }

        private byte DefineTotalPlayerNumber(GameContext dbContext, string wordKey)
        {
            Room room = dbContext.Rooms.FirstOrDefault(room => room.WordKey.Equals(wordKey));
            return room.PlayerNumber;
        }

        private byte DefineConnectedPlayers(GameContext dbContext, string wordKey, Dictionary<int, string> playerSockets)
        {
            Room room = dbContext.Rooms.FirstOrDefault(room => room.WordKey.Equals(wordKey));

            int[] playerIds = new int[TotalPlayerNumber];

            for (int i = 0; i < TotalPlayerNumber; i++)
            {
                playerIds[i] = room.Players[i].Id;
            }

            byte connectedPlayers = 0;

            foreach (var player in playerSockets)
            {
                if (player.Key.Equals(playerIds))
                {
                    connectedPlayers++;
                }
            }

            return connectedPlayers;
        }
    }
}
