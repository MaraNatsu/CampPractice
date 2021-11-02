using EFDataAccessLibrary;
using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public GameContext _dbContext { get; }

        public HomeController(GameContext context)
        {
            _dbContext = context;
        }

        [HttpPost("create-room")]
        public async Task<RoomOutputModel> CreateRoom([FromBody] RoomCreationInputModel roomData)
        {
            Room inspector = _dbContext.Rooms.FirstOrDefault(room => room.WordKey.Equals(roomData.WordKey));

            if (inspector != null)
            {
                return null;
            }

            Room room = new Room
            {
                WordKey = roomData.WordKey,
                PlayerNumber = roomData.PlayerNumber
            };
            _dbContext.Rooms.Add(room);

            //Player firstPlayer = new Player
            //{
            //    RoomId = room.Id,
            //    Name = creatorData.PlayerName,
            //    State = PlayerState.Waiting.ToString(),
            //    Turn = 1,
            //    HealthCount = 1,
            //    Room = room
            //};
            //dbContext.Players.Add(firstPlayer);

            await _dbContext.SaveChangesAsync();
            RoomOutputModel roomViewModel = new RoomOutputModel(room);

            return roomViewModel;
        }

        //[HttpPost("join-room")]
        //public async Task<RoomOutputModel> JoinRoom([FromBody] RoomJoiningInputModel playerData)
        //{
        //    Room room = dbContext.Rooms.Include(room => room.Players).FirstOrDefault(room => room.WordKey.Equals(playerData.WordKey));

        //    if (room == null)
        //    {
        //        return null;
        //    }

        //    byte previousPlayerTurn = dbContext.Players.Max(player => player.Turn);

        //    Player player = new Player
        //    {
        //        RoomId = room.Id,
        //        Name = playerData.PlayerName,
        //        State = PlayerState.Waiting.ToString(),
        //        Turn = (byte)(previousPlayerTurn + 1),
        //        HealthCount = 1
        //    };
        //    dbContext.Players.Add(player);

        //    await dbContext.SaveChangesAsync();
        //    RoomOutputModel roomViewModel = new RoomOutputModel(room, player);

        //    return roomViewModel;
        //}
    }
}
