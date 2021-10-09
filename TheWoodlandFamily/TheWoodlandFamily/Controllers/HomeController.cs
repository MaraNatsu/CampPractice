using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.InputModels;
using TheWoodlandFamily.Models;
using TheWoodlandFamily.OutputModels;

namespace TheWoodlandFamily.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private GameContext db;
        public HomeController(GameContext context)
        {
            db = context;
        }

        [HttpPost("create-room")]
        public async Task<RoomOutputModel> CreateRoom([FromBody] RoomCreationInputModel creatorData)
        {
            var inspector = db.Rooms.FirstOrDefault(room => room.WordKey.Equals(creatorData.WordKey));

            if (inspector != null)
            {
                return null;
            }

            Player firstPlayer = new Player
            {
                Name = creatorData.PlayerName,
                Turn = 1,
                HealthCount = 1
            };
            db.Players.Add(firstPlayer);

            Room room = new Room
            {
                WordKey = creatorData.WordKey,
                PlayerNumber = creatorData.PlayerNumber
            };
            room.Players.Add(firstPlayer);
            db.Rooms.Add(room);

            RoomOutputModel roomViewModel = new RoomOutputModel(room, firstPlayer);
            await db.SaveChangesAsync();

            return roomViewModel;
        }

        [HttpPost("join-room")]
        public async Task<RoomOutputModel> JoinRoom([FromBody] RoomJoiningInputModel playerData)
        {
            Room room = db.Rooms.FirstOrDefault(room => room.WordKey.Equals(playerData.WordKey));

            if (room == null)
            {
                return null;
            }

            Player previousPlayer = room.Players[room.Players.Count() - 1];

            Player player = new Player
            {
                Name = playerData.PlayerName,
                Turn = (byte)(previousPlayer.Turn + 1),
                HealthCount = 1
            };
            db.Players.Add(player);

            RoomOutputModel roomViewModel = new RoomOutputModel(room, player);
            await db.SaveChangesAsync();

            return roomViewModel;
        }
    }
}
