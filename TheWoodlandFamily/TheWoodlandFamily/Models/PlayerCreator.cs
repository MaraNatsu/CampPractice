using EFDataAccessLibrary;
using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.Controllers;
using TheWoodlandFamily.InputModels;
using TheWoodlandFamily.OutputModels;

namespace TheWoodlandFamily.Models
{
    public class PlayerCreator
    {
        private HomeController _controller;

        public async Task<PlayerOutputModel> JoinRoom(RoomJoiningInputModel playerData)
        {

            Room room = _controller
                ._dbContext
                .Rooms
                .Include(room => room.Players)
                .FirstOrDefault(room => room.WordKey.Equals(playerData.WordKey));

            if (room == null)
            {
                return null;
            }

            byte previousPlayerTurn;

            if (_controller._dbContext.Players.Count() == 0)
            {
                previousPlayerTurn = 0;
            }
            else
            {
                previousPlayerTurn = _controller._dbContext.Players.Max(player => player.Turn);
            }

            Player player = new Player
            {
                RoomId = room.Id,
                Name = playerData.PlayerName,
                State = PlayerState.Waiting.ToString(),
                Turn = (byte)(previousPlayerTurn + 1),
                HealthCount = 1
            };
            _controller._dbContext.Players.Add(player);

            await _controller._dbContext.SaveChangesAsync();
            PlayerOutputModel playerViewModel = new PlayerOutputModel(room, player);

            return playerViewModel;
        }
    }
}
