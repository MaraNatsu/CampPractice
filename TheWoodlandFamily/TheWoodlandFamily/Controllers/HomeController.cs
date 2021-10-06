using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheWoodlandFamily.Models;
using TheWoodlandFamily.ViewModels;

namespace TheWoodlandFamily.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private GameContext db;
        public HomeController(GameContext context)
        {
            db = context;
        }

        public RoomViewModel CreateRoom(string playerName, string wordKey, byte playerNumber)
        {
            Player firstPlayer = new Player
            {
                Name = playerName,
                Turn = 1,
                HealthCount = 1
            };
            db.Players.Add(firstPlayer);

            Room room = new Room
            {
                WordKey = wordKey,
                PlayerNumber = playerNumber
            };
            room.Players.Add(firstPlayer);
            db.Rooms.Add(room);

            RoomViewModel roomViewModel = new RoomViewModel(room, firstPlayer);

            return roomViewModel;
        }

        //[HttpGet]   // GET /api/test2
        //public IActionResult ListProducts()
        //{
        //    return Json(1);
        //}

        //[HttpGet("{id}")]   // GET /api/test2/xyz
        //public IActionResult GetProduct(string id)
        //{
        //    return Json(1);
        //}

        //[HttpGet("int/{id:int}")] // GET /api/test2/int/3
        //public IActionResult GetIntProduct(int id)
        //{
        //    return Json(1);
        //}

        //[HttpGet("int2/{id}")]  // GET /api/test2/int2/3
        //public IActionResult GetInt2Product(int id)
        //{
        //    return Json(1);
        //}
    }
}
