using EFDataAccessLibrary;
using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            if (_checker.CheckIfAllPlayersConnected(room, _holder.PlayerConnections[room.Id]))
            {
                Player firstPlayer = room.Players.FirstOrDefault(player => player.Turn == 1);
                string connectionId = _holder
                    .PlayerConnections[room.Id]
                    .First(player => player.PlayerId == firstPlayer.Id)
                    .ConnectionId;

                firstPlayer.State = PlayerState.Active.ToString();
                _dbContext.SaveChanges();

                await Clients.Group(room.WordKey).SendAsync("StartGame", firstPlayer.Id);
                await AllowMove(connectionId);
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

        public async Task AllowMove(string connectionId)
        {
            await Clients.Client(connectionId).SendAsync("SetPlayerToMove");
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

            PlayerOutputModel movedPlayer = _processor.UpdatePlayerStatus(_dbContext, room, playerId, cardType);
            await Clients.Client(Context.ConnectionId).SendAsync("UpdatePlayerData", movedPlayer);

            int activePlayersNumber = _processor.CheckActivePlayersNumber(room);

            if (activePlayersNumber > 1)
            {

                Player nextPlayer = _processor.PassMove(_dbContext, room, playerId);

                string connectionId = _holder
                    .PlayerConnections[room.Id]
                    .FirstOrDefault(player => player.PlayerId == nextPlayer.Id)
                    .ConnectionId;

                await AllowMove(connectionId);
            }
            else
            {
                PlayerOutputModel winner = _processor.DefineWinner(room);
                await Clients.Group(wordkey).SendAsync("FinishGame", winner.Id);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            HttpContext playerToken = Context.GetHttpContext();
            int playerId = Convert.ToInt32(playerToken.Request.Query["playerId"]);

            Console.WriteLine("Disconnected: " + playerId + " - " + Context.ConnectionId);

            Room room = _dbContext
                .Players
                .Include(player => player.Room)
                .FirstOrDefault(player => player.Id == playerId)
                .Room;

            _holder.RemoveConnection(room.Id, playerId);

            Player playerToRemove = room.Players.FirstOrDefault(player => player.Id == playerId);

            if (playerToRemove != null)
            {
                _dbContext.Players.Remove(playerToRemove);
                _dbContext.SaveChanges();
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.WordKey);
            await Clients.Group(room.WordKey).SendAsync("RemoveDisconnectedPlayer", playerId);
        }
    }
}