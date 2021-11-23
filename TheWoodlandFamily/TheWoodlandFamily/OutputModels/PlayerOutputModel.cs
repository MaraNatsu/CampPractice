using EFDataAccessLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWoodlandFamily.OutputModels
{
    public class PlayerOutputModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public PlayerOutputModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
