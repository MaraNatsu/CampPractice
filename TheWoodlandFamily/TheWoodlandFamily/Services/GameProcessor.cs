using EFDataAccessLibrary;
using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using System;
using System.Linq;
using TheWoodlandFamily.OutputModels;

namespace TheWoodlandFamily.Services
{
    public class GameProcessor
    {
        public string DefineCardType(GameContext dbContext, Room room)
        {
            Card cardTaken = room.Deck[0];
            string cardType = cardTaken.Type.ToString();
            room.Deck.RemoveAt(0);
            dbContext.SaveChanges();

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
                    if (playerToUpdate.HealthCount > 0)
                    {
                        playerToUpdate.HealthCount -= 1;

                        Random random = new Random();
                        int index = random.Next(0, room.Deck.Count);

                        room.Deck.Insert(index, new Card()
                        {
                            RoomId = room.Id,
                            Order = (byte)(index + 1),
                            Type = CardType.Trap.ToString(),
                            Room = room
                        });
                    }
                    else
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
                if (player.State != PlayerState.Observing.ToString())
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
                .Players
                .FirstOrDefault(player => player.Turn > previousPlayer.Turn && player.State != PlayerState.Observing.ToString());

            if (nextPlayer == null)
            {
                nextPlayer = room
                    .Players
                    .First(player => player.Turn < previousPlayer.Turn && player.State != PlayerState.Observing.ToString());
            }

            nextPlayer.State = PlayerState.Active.ToString();
            dbContext.SaveChanges();

            return nextPlayer;
        }

        public PlayerOutputModel DefineWinner(Room room)
        {
            Player lastPlayer = room.Players.First(player => player.State != PlayerState.Observing.ToString());
            PlayerOutputModel winner = new PlayerOutputModel
            {
                Id = lastPlayer.Id,
                PlayerName = lastPlayer.Name,
                Turn = lastPlayer.Turn,
                HealthCount = lastPlayer.HealthCount
            };

            return winner;
        }
    }
}