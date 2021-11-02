using EFDataAccessLibrary.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.InputModels;
using TheWoodlandFamily.Models;
using TheWoodlandFamily.OutputModels;

namespace TheWoodlandFamily.Hubs
{
    public class ChatHub : Hub
    {
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
            RoomJoiningInputModel newPlayer = new RoomJoiningInputModel
            {
                PlayerName = playerToJoin.PlayerName,
                WordKey = playerToJoin.WordKey
            };

            PlayerCreator playerCreator = new PlayerCreator();
            PlayerOutputModel joinedPlayer = await playerCreator.JoinRoom(newPlayer);
            await Groups.AddToGroupAsync(Context.ConnectionId, playerToJoin.WordKey);
            await Clients.Group(playerToJoin.WordKey).SendAsync("OnPlayerJoined", joinedPlayer);
            //await Clients.OthersInGroup(playerToJoin.WordKey).SendAsync("OnPlayerJoined", joinedPlayer);
        }
    }
}
