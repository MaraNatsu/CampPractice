using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TheWoodlandFamily.InputModels;
using TheWoodlandFamily.Models;
using TheWoodlandFamily.OutputModels;
using TheWoodlandFamily.Services;

namespace TheWoodlandFamily.Hubs
{
    public class GameHub : Hub
    {
        public GameContext _dbContext { get; }

        private PlayerNumberChecker _checker = new PlayerNumberChecker();
        private WebSocketsHolder _holder;

        public GameHub(GameContext context, WebSocketsHolder holder)
        {
            _dbContext = context;
            _holder = holder;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            HttpContext playerToken = Context.GetHttpContext();
            int playerId = Convert.ToInt32(playerToken.Request.Query["playerId"]);
            Console.WriteLine("Connected: " + playerId + " - " + Context.ConnectionId);

            int roomId = _dbContext
                .Players
                .FirstOrDefault(player => player.Id == playerId)
                .RoomId;
            Room room = _dbContext
                .Rooms
                .Include(room => room.Players)
                .FirstOrDefault(room => room.Id == roomId);

            _holder.AddConnection(roomId, playerId, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, room.WordKey);

            await SendAllActiveConnectionsAsync(room);

            if (_checker.CheckIfAllPlayersConnected(room, _holder.PlayerConnections[roomId]))
            {
                await Clients.Group(room.WordKey).SendAsync("StartGame");
            }
        }

        private async Task SendAllActiveConnectionsAsync(Room room)
        {
            List<PlayerOutputModel> connectedPlayers = new List<PlayerOutputModel>();
            List<ConnectionDataModel> connections = _holder.PlayerConnections[room.Id];

            foreach (var connection in connections)
            {
                string playerName = room
                    .Players
                    .FirstOrDefault(player => player.Id == connection.PlayerId)
                    .Name;

                connectedPlayers.Add(new PlayerOutputModel
                {
                    Id = connection.PlayerId,
                    PlayerName = playerName
                });
            }

            await Clients.Group(room.WordKey).SendAsync("GetConnectedPlayers", connectedPlayers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            HttpContext playerToken = Context.GetHttpContext();
            int playerId = Convert.ToInt32(playerToken.Request.Query["playerId"]);

            Console.WriteLine("Disconnected: " + playerId + " - " + Context.ConnectionId);

            int roomId = _dbContext
                .Players
                .Where(player => player.Id == playerId)
                .FirstOrDefault()
                .RoomId;
            string roomWordKey = _dbContext
                .Rooms
                .Where(room => room.Id == roomId)
                .FirstOrDefault()
                .WordKey;

            _holder.RemoveConnection(roomId, playerId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomWordKey);
            await Clients.Group(roomWordKey).SendAsync("RemoveDisconnectedPlayer", playerId);
        }

        //public async Task SendConnectedPlayer(PlayerConnectingInputModel playerToConnect)
        //{
        //    PlayerOutputModel connectedPayer = new PlayerOutputModel
        //    {
        //        Id = playerToConnect.Id,
        //        PlayerName = playerToConnect.Name
        //    };

        //    await Groups.AddToGroupAsync(Context.ConnectionId, playerToConnect.Wordkey);
        //    await Clients.Group(playerToConnect.Wordkey).SendAsync("AddConnectedPlayer", connectedPayer);

        //    if (_checker.CheckIfAllPlayersConnected(playerToConnect.Wordkey, _dbContext, _holder.PlayerConnections))
        //    {
        //        await Clients.Group(playerToConnect.Wordkey).SendAsync("StartGame");
        //    }
        //}
    }
}
