using EFDataAccessLibrary.DataAccess;
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

        private PlayerJoiner _playerJoiner;
        private PlayerNumberChecker _checker = new PlayerNumberChecker();
        private WebSocketsHolder _holder;

        public GameHub(GameContext context, PlayerJoiner playerJoiner, WebSocketsHolder holder)
        {
            _dbContext = context;
            _playerJoiner = playerJoiner;
            _holder = holder;
        }

        public override Task OnConnectedAsync()
        {
            var playerToken = Context.GetHttpContext();
            Console.WriteLine(playerToken.Request.Query["playerId"]);
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

            Console.WriteLine("Disconnected: " + Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        //public async Task SendMessage(string player, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", player, message);
        //}

        public async Task SendJoinedPlayer(RoomJoiningInputModel playerToJoin)
        {
            PlayerOutputModel joinedPlayer = await _playerJoiner.JoinRoom(playerToJoin, _dbContext); //creates player entity in DB
            await Groups.AddToGroupAsync(Context.ConnectionId, playerToJoin.WordKey);
            await Clients.Client(Context.ConnectionId).SendAsync("JoinedPlayer", joinedPlayer);

            _holder.PlayerConnections.Add(joinedPlayer.PlayerId, Context.ConnectionId);

            if (_checker.CheckIfAllPlayersConnected(playerToJoin.WordKey, _dbContext, _holder.PlayerConnections))
            {
                await Clients.Group(playerToJoin.WordKey).SendAsync("StartGame");
            }
        }
    }
}
