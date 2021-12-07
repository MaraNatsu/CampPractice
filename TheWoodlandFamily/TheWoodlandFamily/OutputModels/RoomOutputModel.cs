using EFDataAccessLibrary.Entities;

namespace TheWoodlandFamily.OutputModels
{
    public class RoomOutputModel
    {
        public int RoomId { get; set; }
        public byte PlayerNumber { get; set; }
        public string Wordkey { get; set; }
    }
}
