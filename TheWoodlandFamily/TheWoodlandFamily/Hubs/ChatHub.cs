using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.InputModels;
using TheWoodlandFamily.Models;
using TheWoodlandFamily.OutputModels;
using TheWoodlandFamily.Services;

namespace TheWoodlandFamily.Hubs
{
    public class ChatHub : Hub
    {
        public GameContext _dbContext { get; }

        private PlayerJoiner _playerJoiner;
        private Dictionary<int, string> _playerSockets = new Dictionary<int, string>();

        public ChatHub(GameContext context, PlayerJoiner playerJoiner)
        {
            _dbContext = context;
            _playerJoiner = playerJoiner;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string player, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", player, message);
        }

        public async Task SendJoinedPlayer(RoomJoiningInputModel playerToJoin)
        {
            PlayerOutputModel joinedPlayer = await _playerJoiner.JoinRoom(playerToJoin); //creates player entity in DB
            await Groups.AddToGroupAsync(Context.ConnectionId, playerToJoin.WordKey);
            _playerSockets.Add(joinedPlayer.PlayerId, Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).SendAsync("OnPlayerJoined", joinedPlayer);

            PlayerNumberChecker checker = new PlayerNumberChecker(playerToJoin.WordKey, _dbContext, _playerSockets);

            if (checker.IsAllPlayersConnected)
            {
                await Clients.Group(playerToJoin.WordKey).SendAsync("StartGame");
            }
        }
    }
}
