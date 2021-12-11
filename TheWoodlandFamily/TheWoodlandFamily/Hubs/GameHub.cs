using EFDataAccessLibrary;
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
        private GameProcessor _processor;

        public GameHub(GameContext context, WebSocketsHolder holder, GameProcessor processor)
        {
            _dbContext = context;
            _holder = holder;
            _processor = processor;
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
                Player firstPlayer = room.Players.FirstOrDefault(player => player.Turn == 1);
                firstPlayer.State = PlayerState.Active.ToString();
                _dbContext.SaveChanges();

                await Clients.Group(room.WordKey).SendAsync("StartGame", firstPlayer.Id);
            }
        }

        private async Task SendAllActiveConnectionsAsync(Room room)
        {
            List<PlayerOutputModel> connectedPlayers = new List<PlayerOutputModel>();
            List<ConnectionDataModel> connections = _holder.PlayerConnections[room.Id];

            foreach (var connection in connections)
            {
                Player player = room
                    .Players
                    .FirstOrDefault(player => player.Id == connection.PlayerId);

                connectedPlayers.Add(new PlayerOutputModel
                {
                    Id = connection.PlayerId,
                    PlayerName = player.Name,
                    Turn = player.Turn,
                    HealthCount = player.HealthCount
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
                .FirstOrDefault(player => player.Id == playerId)
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

        public async Task ProcessMove(int playerId, string wordkey)
        {
            Room room = _dbContext
                .Rooms
                .Include(room => room.Players)
                .Include(room => room.Deck)
                .FirstOrDefault(room => room.WordKey == wordkey);

            string cardType = _processor.DefineCardType(_dbContext, room);
            await Clients.Client(Context.ConnectionId).SendAsync("ShowCardTaken", cardType);

            PlayerOutputModel updatedPlayer = _processor.UpdatePlayerStatus(_dbContext, room, playerId, cardType);
            await Clients.Group(wordkey).SendAsync("UpdatePlayerData", updatedPlayer);

            int activePlayersNumber = _processor.CheckActivePlayersNumber(room);

            if (activePlayersNumber > 1)
            {

                Player nextPlayer = _processor.PassMove(_dbContext, room, playerId);

                string connectionId = _holder
                    .PlayerConnections[room.Id]
                    .FirstOrDefault(player => player.PlayerId == nextPlayer.Id)
                    .ConnectionId;

                await Clients.Client(connectionId).SendAsync("MakeMove", nextPlayer.Id);
            }
            else
            {
                PlayerOutputModel winner = _processor.DefineWinner(room);
                await Clients.Group(wordkey).SendAsync("FinishGame", winner.Id);
            }
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
