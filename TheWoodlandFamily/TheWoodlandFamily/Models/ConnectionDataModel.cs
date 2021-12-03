using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.Models
{
    public class ConnectionDataModel
    {
        public int PlayerId { get; private set; }
        public string ConnectionId { get; private set; }

        public ConnectionDataModel(int playerId, string connectionId)
        {
            PlayerId = playerId;
            ConnectionId = connectionId;
        }
    }
}
