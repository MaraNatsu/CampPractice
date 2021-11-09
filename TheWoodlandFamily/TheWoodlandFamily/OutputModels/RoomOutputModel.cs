using EFDataAccessLibrary.Entities;

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
