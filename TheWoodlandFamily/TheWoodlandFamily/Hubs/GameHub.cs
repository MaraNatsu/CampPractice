using EFDataAccessLibrary.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TheWoodlandFamily.InputModels;
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

        public override Task OnConnectedAsync()
        {
            HttpContext playerToken = Context.GetHttpContext();
            int playerId = Convert.ToInt32(playerToken.Request.Query["playerId"]);
            _holder.PlayerConnections.Add(playerId, Context.ConnectionId);

            Console.WriteLine("Connected: " + Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _holder
                .PlayerConnections
                .Remove(_holder
                .PlayerConnections
                .Where(connectionId => connectionId.Value == Context.ConnectionId)
                .FirstOrDefault()
                .Key);

            HttpContext playerToken = Context.GetHttpContext();
            int playerId = Convert.ToInt32(playerToken.Request.Query["playerId"]);

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

            Clients.Group(roomWordKey).SendAsync("DisconnectPlayer", playerId);

            Console.WriteLine("Disconnected: " + Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendConnectedPlayer(PlayerConnectingInputModel playerToConnect)
        {
            PlayerOutputModel connectedPlayer = new PlayerOutputModel
            {
                Id = playerToConnect.Id,
                PlayerName = playerToConnect.Name
            };

            await Groups.AddToGroupAsync(Context.ConnectionId, playerToConnect.Wordkey);
            await Clients.Group(playerToConnect.Wordkey).SendAsync("ShowConnectedPlayer", connectedPlayer);

            if (_checker.CheckIfAllPlayersConnected(playerToConnect.Wordkey, _dbContext, _holder.PlayerConnections))
            {
                await Clients.Group(playerToConnect.Wordkey).SendAsync("StartGame");
            }
        }
    }
}
