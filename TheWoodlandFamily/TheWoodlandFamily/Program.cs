using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using EFDataAccessLibrary.DataAccess;

namespace TheWoodlandFamily
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            var builder = new ConfigurationBuilder();
            // path to current catalogue
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // get confuguration from appsettings.json
            builder.AddJsonFile("appsettings.json");
            // create configuration
            var config = builder.Build();
            // get connection line
            string connectionString = config.GetConnectionString("Default");

            var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
