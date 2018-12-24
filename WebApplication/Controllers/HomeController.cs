using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private ConnectionDB connection;

        public HomeController(ConnectionDB connection)
        {
            this.connection = connection;
        }

        public IActionResult Index(string sortOrder)
        {
            try
            {
                var cars = from s in connection.Car select s;
                int val = 0;
                switch (sortOrder)
                {
                    case "brand":
                        cars = (val = Convert.ToInt32(HttpContext.Session.GetInt32("brand"))) == 1 ? cars.OrderByDescending(s => s.Brand) : cars.OrderBy(s => s.Brand);
                        HttpContext.Session.SetInt32("brand", val == 0 ? 1 : 0);
                        break;
                    case "model":
                        cars = (val = Convert.ToInt32(HttpContext.Session.GetInt32("model"))) == 1 ? cars.OrderByDescending(s => s.Model) : cars.OrderBy(s => s.Model);
                        HttpContext.Session.SetInt32("model", val == 0 ? 1 : 0);
                        break;
                    case "color":
                        cars = (val = Convert.ToInt32(HttpContext.Session.GetInt32("color"))) == 1 ? cars.OrderByDescending(s => s.Color) : cars.OrderBy(s => s.Color);
                        HttpContext.Session.SetInt32("color", val == 0 ? 1 : 0);
                        break;
                    case "plate":
                        cars = (val = Convert.ToInt32(HttpContext.Session.GetInt32("plate"))) == 1 ? cars.OrderByDescending(s => s.Plate) : cars.OrderBy(s => s.Plate);
                        HttpContext.Session.SetInt32("plate", val == 0 ? 1 : 0);
                        break;
                    case "city":
                        cars = (val = Convert.ToInt32(HttpContext.Session.GetInt32("city"))) == 1 ? cars.OrderByDescending(s => s.City) : cars.OrderBy(s => s.City);
                        HttpContext.Session.SetInt32("city ", val == 0 ? 1 : 0);
                        break;
                    default:
                        cars = cars.OrderBy(s => s.Brand);
                        break;
                }
                ViewBag.Active = false;
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    TempData["CarsSelected"] = connection.Dispatch.ToList();
                }
                return View(cars.ToList());
            }
            catch (MySqlException e)
            {
                return RedirectToAction("Error", new { e.Message });
            }
        }


        [HttpPost]
        public IActionResult Index(IList<Car> list)
        {
            IDictionary<string, Dispatch> selected = new Dictionary<string, Dispatch>();
            bool active = true;
            for (var c = 0; c < list.Count; c++)
            {
                if (list[c].Selected)
                {
                    if (!selected.ContainsKey(list[c].City))
                    {
                        selected.Add(list[c].City, new Dispatch { City = list[c].City, Quantity = 1 });
                    }
                    else
                    {
                        selected[list[c].City].Quantity++;
                    }
                }
                else if (active)
                {
                    active = false;
                }
            }
            ViewBag.Active = active;
            IList<Dispatch> dispatches = selected.Values.ToList();
            HttpContext.Session.SetString("CarsSelected", JsonConvert.SerializeObject(dispatches));
            TempData["CarsSelected"] = dispatches;
            return View(list);
        }

        [HttpPost]
        public IActionResult SaveDispatches()
        {
            IList<Dispatch> dispatches = JsonConvert.DeserializeObject<List<Dispatch>>(HttpContext.Session.GetString("CarsSelected"));
            foreach (var d in dispatches)
            {
                connection.Dispatch.AddAsync(d);
            }
            connection.SaveChanges();
            return RedirectToAction("Index", new { sortOrder = "brand" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, RequestMessage = message });
        }
    }
}
