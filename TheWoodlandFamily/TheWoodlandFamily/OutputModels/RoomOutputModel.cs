using EFDataAccessLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.OutputModels
{
    public class RoomOutputModel
    {
        public int RoomId { get; set; }

        public RoomOutputModel(Room room)
        {
            RoomId = room.Id;
        }
    }
}
