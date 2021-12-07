using EFDataAccessLibrary;
using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.Models;
using TheWoodlandFamily.OutputModels;

namespace TheWoodlandFamily.Services
{
    public class GameProcessor
    {
        public string DefineCardType(GameContext dbContext, Room room)
        {
            Card cardTaken = room.Deck[0];
            string cardType = cardTaken.Type;
            room.Deck.Remove(cardTaken);

            return cardType;
        }

        public PlayerOutputModel UpdatePlayerStatus(GameContext dbContext, Room room, int playerId, string cardType)
        {
            Player playerToUpdate = room.Players.First(player => player.Id == playerId);
            playerToUpdate.State = PlayerState.Waiting.ToString();

            switch (cardType)
            {
                case nameof(CardType.Simple):
                    break;
                case nameof(CardType.Life):
                    playerToUpdate.HealthCount += 1;
                    break;
                case nameof(CardType.Trap):
                    playerToUpdate.HealthCount -= 1;

                    if (playerToUpdate.HealthCount < 0)
                    {
                        playerToUpdate.State = PlayerState.Observing.ToString();
                    }
                    break;
            }

            dbContext.SaveChanges();

            PlayerOutputModel updatedPlayer = new PlayerOutputModel
            {
                Id = playerToUpdate.Id,
                PlayerName = playerToUpdate.Name,
                Turn = playerToUpdate.Turn,
                HealthCount = playerToUpdate.HealthCount
            };

            return updatedPlayer;
        }

        public int CheckActivePlayersNumber(Room room)
        {
            int i = 0;

            foreach (var player in room.Players)
            {
                if (player.State == PlayerState.Active.ToString())
                {
                    i++;
                }
            }

            return i;
        }

        public Player PassMove(GameContext dbContext, Room room, int previousPlayerId)
        {
            Player previousPlayer = room.Players.First(player => player.Id == previousPlayerId);
            Player nextPlayer = room
                .Players.FirstOrDefault(player => player.Turn > previousPlayer.Turn && player.State != PlayerState.Observing.ToString());

            if (nextPlayer.State != null)
            {
                nextPlayer.State = PlayerState.Active.ToString();
            }

            dbContext.SaveChanges();

            return nextPlayer;
        }

        public PlayerOutputModel DefineWinner(Room room)
        {
            Player activePlayer = room.Players.First(player => player.State == PlayerState.Active.ToString());
            PlayerOutputModel winner = new PlayerOutputModel
            {
                Id = activePlayer.Id,
                PlayerName = activePlayer.Name,
                Turn = activePlayer.Turn,
                HealthCount = activePlayer.HealthCount
            };

            return winner;
        }
    }
}
