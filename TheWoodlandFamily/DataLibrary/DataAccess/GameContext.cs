using System;
using System.Collections.Generic;
using System.Text;
using EFDataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFDataAccessLibrary.DataAccess
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions options) : base(options) { }
        public DbSet<Player> Players { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Card> Deck { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=NOTEBOOK-HOME-A;Initial Catalog=EFDemoDB;Integrated Security=True;");
        }
    }
}

