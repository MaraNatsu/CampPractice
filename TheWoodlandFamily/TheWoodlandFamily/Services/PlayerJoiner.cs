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

namespace TheWoodlandFamily.Services
{
    public class PlayerJoiner
    {
        public async Task<PlayerOutputModel> JoinRoom(RoomJoiningInputModel playerData, GameContext dbContext)
        {
            Room room = dbContext
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
                Name = playerData.Name,
                State = PlayerState.Waiting.ToString(),
                Turn = (byte)(previousPlayerTurn + 1),
                HealthCount = 1,
                Room = room
            };
            room.Players.Add(player);

            await dbContext.SaveChangesAsync();
            PlayerOutputModel playerViewModel = new PlayerOutputModel
            {
                Id = player.Id,
                PlayerName = player.Name
            };

            return playerViewModel;
        }
    }
}
