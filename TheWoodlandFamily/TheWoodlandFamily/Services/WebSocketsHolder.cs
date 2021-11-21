using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.Services
{
    public class WebSocketsHolder
    {
        public Dictionary<int, string> PlayerConnections { get; set; } = new Dictionary<int, string>();
    }
}
