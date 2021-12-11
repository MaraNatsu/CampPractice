using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.Models;

namespace TheWoodlandFamily.Services
{
    public class WebSocketsHolder
    {
        public Dictionary<int, List<ConnectionDataModel>> PlayerConnections { get; set; } = new Dictionary<int, List<ConnectionDataModel>>();

        public void AddConnection(int roomId, int playerId, string connectionId)
        {
            List<ConnectionDataModel> connections;
            int activeRoomId = PlayerConnections.Keys.FirstOrDefault(id => id == roomId);

            if (activeRoomId != 0)
            {
                connections = PlayerConnections[roomId];
                connections.Add(new ConnectionDataModel(playerId, connectionId));
                return;
            }

            connections = new List<ConnectionDataModel>();
            connections.Add(new ConnectionDataModel(playerId, connectionId));
            PlayerConnections.Add(roomId, connections);
        }

        public void RemoveConnection(int roomId, int playerId)
        {
            int activeRoomId = PlayerConnections.Keys.FirstOrDefault(id => id == roomId);

            if (activeRoomId == 0)
            {
                return;
            }

            List<ConnectionDataModel> connections = PlayerConnections[roomId];
            ConnectionDataModel connectionToRemove = connections.FirstOrDefault(connection => connection.PlayerId == playerId);
            connections.Remove(connectionToRemove);

            if (connections.Count == 0)
            {
                PlayerConnections.Remove(roomId);
            }
        }
    }
}
