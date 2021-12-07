using EFDataAccessLibrary;
using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.InputModels;
using TheWoodlandFamily.OutputModels;

namespace TheWoodlandFamily.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        public GameContext _dbContext { get; }

        public HomeController(GameContext context)
        {
            _dbContext = context;
        }

        [HttpPost("create-room")]
        public async Task<RoomOutputModel> CreateRoom([FromBody] RoomCreationInputModel roomData)
        {
            Room inspector = _dbContext.Rooms.FirstOrDefault(room => room.WordKey.Equals(roomData.Wordkey));

            if (inspector != null)
            {
                return null;
            }

            Room room = new Room
            {
                WordKey = roomData.Wordkey,
                PlayerNumber = roomData.PlayerNumber
            };
           
            _dbContext.Rooms.Add(room);

            GenerateDeck(room);         

            await _dbContext.SaveChangesAsync();
            RoomOutputModel roomViewModel = new RoomOutputModel
            {
                RoomId = room.Id,
                PlayerNumber = room.PlayerNumber,
                Wordkey = room.WordKey
            };

            return roomViewModel;
        }

        [HttpPost("create-player")]
        public async Task<PlayerOutputModel> CreatePlayer([FromBody] RoomJoiningInputModel playerData)
        {
            Room room = _dbContext
                .Rooms
                .Include(room => room.Players)
                .FirstOrDefault(room => room.WordKey.Equals(playerData.Wordkey));

            if (room == null)
            {
                return null;
            }

            byte previousPlayerTurn;

            if (room.Players.Count() == 0)
            {
                previousPlayerTurn = 0;
            }
            else
            {
                previousPlayerTurn = room.Players.Max(player => player.Turn);
            }

            Player player = new Player
            {
                RoomId = room.Id,
                State = PlayerState.Waiting.ToString(),
                Name = playerData.Name,
                HealthCount = 1,
                Turn = (byte)(previousPlayerTurn + 1),
                Room = room
            };

            room.Players.Add(player);

            await _dbContext.SaveChangesAsync();
            PlayerOutputModel playerViewModel = new PlayerOutputModel
            {
                Id = player.Id,
                PlayerName = player.Name,
                Turn = player.Turn,
                HealthCount = player.HealthCount
            };
            //ClientDataModel clientData = new ClientDataModel(room, player);

            return playerViewModel;
        }

        private void GenerateDeck(Room room)
        {
            byte lifeCardNumber = 4;
            byte simpleCardNumber = 20;
            byte trapCardNumber = (byte)(room.PlayerNumber - 1);
            byte cardNumber = (byte)(lifeCardNumber + simpleCardNumber + trapCardNumber);

            List<Card> deck = new List<Card>();
            Random random = new Random();

            FillDeckWIthType(deck, lifeCardNumber, CardType.Life);
            FillDeckWIthType(deck, simpleCardNumber, CardType.Simple);
            FillDeckWIthType(deck, trapCardNumber, CardType.Trap);

            for (int i = deck.Count - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);

                var temp = deck[j];
                deck[j] = deck[i];
                deck[i] = temp;
            }

            byte turn = 1;

            foreach (var card in deck)
            {
                card.RoomId = room.Id;
                card.Order = turn;
                card.Room = room;

                _dbContext.Cards.Add(card);
                room.Deck.Add(card);
                turn++;
            }
        }

        private void FillDeckWIthType(List<Card> deck, int cardNumber, CardType type)
        {
            int i = 0;

            while (i < cardNumber)
            {
                deck.Add(new Card
                {
                    Type = type.ToString()
                });

                i++;
            }
        }
    }
}
