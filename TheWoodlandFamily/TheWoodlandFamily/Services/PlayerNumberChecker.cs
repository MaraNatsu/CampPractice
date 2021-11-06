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
        public byte TotalPlayerNumber { get; }
        public byte JoinedPlayerNumber { get; }
        public bool IsAllPlayersConnected { get; }

        private string _wordKey;
        private GameContext _dbContext;
        private Dictionary<int, string> _playerSockets;

        public PlayerNumberChecker(string wordKey, GameContext dbContext, Dictionary<int, string> playerSockets)
        {
            _wordKey = wordKey;
            _dbContext = dbContext;
            _playerSockets = playerSockets;

            TotalPlayerNumber = DefineTotalPlayerNumber();
            JoinedPlayerNumber = DefineConnectedPlayers();
            IsAllPlayersConnected = CheckIfAllPlayersConnected();
        }

        private byte DefineTotalPlayerNumber()
        {
            Room room = _dbContext.Rooms.FirstOrDefault(room => room.WordKey.Equals(_wordKey));
            return room.PlayerNumber;
        }

        private byte DefineConnectedPlayers()
        {
            Room room = _dbContext.Rooms.FirstOrDefault(room => room.WordKey.Equals(_wordKey));

            int[] playerIds = new int[TotalPlayerNumber];

            for (int i = 0; i < TotalPlayerNumber; i++)
            {
                playerIds[i] = room.Players[i].Id;
            }

            byte connectedPlayers = 0;

            foreach (var player in _playerSockets)
            {
                if (player.Key.Equals(playerIds))
                {
                    connectedPlayers++;
                }
            }

            return connectedPlayers;
        }

        private bool CheckIfAllPlayersConnected()
        {
            if (TotalPlayerNumber == JoinedPlayerNumber)
            {
                return true;
            }

            return false;
        }
    }
}
