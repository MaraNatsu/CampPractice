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

            await _dbContext.SaveChangesAsync();
            RoomOutputModel roomViewModel = new RoomOutputModel(room);

            return roomViewModel;
        }

        [HttpPost("create-player")]
        public async Task<ClientDataModel> CreatePlayer([FromBody] RoomJoiningInputModel playerData)
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
            ClientDataModel clientData = new ClientDataModel(room, player);

            return clientData;
        }
    }
}
