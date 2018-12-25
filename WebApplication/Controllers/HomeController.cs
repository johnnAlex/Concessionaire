using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private ConnectionDB connection;
        private IConfiguration configuration;

        public HomeController(ConnectionDB connection, IConfiguration configuration)
        {
            this.connection = connection;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index(string sortOrder, int? page, string showDispatch)
        {
            ViewData["CurrentSort"] = sortOrder;
            try
            {
                var cars = from s in connection.Car select s;
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    int val = Convert.ToInt32(HttpContext.Session.GetInt32(sortOrder));
                    if (page == null)
                    {
                        int count = Convert.ToInt32(HttpContext.Session.GetInt32(sortOrder + "Count")) + 1;
                        HttpContext.Session.SetInt32(sortOrder + "Count", count);
                        if (count > 1)
                        {
                            HttpContext.Session.SetInt32(sortOrder, val == 0 ? 1 : 0);
                        }
                    }
                    switch (sortOrder)
                    {
                        case "brand":
                            cars = val == 1 ? cars.OrderByDescending(s => s.Brand) : cars.OrderBy(s => s.Brand);
                            break;
                        case "model":
                            cars = val == 1 ? cars.OrderByDescending(s => s.Model) : cars.OrderBy(s => s.Model);
                            break;
                        case "color":
                            cars = val == 1 ? cars.OrderByDescending(s => s.Color) : cars.OrderBy(s => s.Color);
                            break;
                        case "plate":
                            cars = val == 1 ? cars.OrderByDescending(s => s.Plate) : cars.OrderBy(s => s.Plate);
                            break;
                        case "city":
                            cars = val == 1 ? cars.OrderByDescending(s => s.City) : cars.OrderBy(s => s.City);
                            break;
                        default:
                            // cars = cars.OrderBy(s => s.Brand);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(showDispatch))
                    TempData["CarsSelected"] = connection.Dispatch.ToList();
                ViewBag.Active = false;
                return View(await PaginatedList<Car>.CreateAsync(cars.AsNoTracking(), page ?? 1, configuration.GetValue<int>("PageSize")));
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
            return RedirectToAction("Index", new { showDispatch = "show" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, RequestMessage = message });
        }
    }
}
