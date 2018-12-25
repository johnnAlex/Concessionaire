using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
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

        public async Task<IActionResult> Index(string sortOrder, int? page, string showDispatch, string sort)
        {
            ViewData["CurrentSort"] = sortOrder;
            HttpContext.Session.SetInt32("CurrentPage", page??1);
            try
            {
                var cars = from s in connection.Car select s;
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    HttpContext.Session.SetString("CurrentSort", sortOrder);                    
                    int val = Convert.ToInt32(HttpContext.Session.GetInt32(sortOrder));
                    if (!string.IsNullOrEmpty(sort))
                    {
                        int count = Convert.ToInt32(HttpContext.Session.GetInt32(sortOrder + "Count")) + 1;
                        HttpContext.Session.SetInt32(sortOrder + "Count", count);
                        if (count > 1)
                        {
                            HttpContext.Session.SetInt32(sortOrder, val == 0 ? 1 : 0);
                            HttpContext.Session.SetInt32(sortOrder + "Count", 1);
                            val = val == 0 ? 1 : 0;
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
                if (cars.Where(x => x.Selected).Count() > 0)
                {
                    ViewBag.dispatch = GetDispatches().ToList();
                }
                if (!string.IsNullOrEmpty(showDispatch))
                    ViewBag.dispatch = connection.Dispatch.ToList();
                return View(await PaginatedList<Car>.CreateAsync(cars.AsNoTracking(), page ?? 1, configuration.GetValue<int>("PageSize")));
            }
            catch (MySqlException e)
            {
                return RedirectToAction("Error", new { e.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(PaginatedList<Car> cars)
        {
            try
            {
                connection.UpdateRange(cars);
                await connection.SaveChangesAsync();
                return RedirectToAction("Index", new {
                    sortOrder = HttpContext.Session.GetString("CurrentSort"),
                    page = HttpContext.Session.GetInt32("CurrentPage"),
                    countDispatch = "count" });
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SaveDispatches()
        {
            var dispatches = GetDispatches();
            foreach (var d in dispatches)
            {
                connection.Dispatch.AddAsync(d);
            }
            IQueryable<Car> c = connection.Car.Where(x => x.Selected);
            c.ForEachAsync(x => x.Selected = false);
            connection.SaveChanges();
            return RedirectToAction("Index", new { showDispatch = "show" });
        }

        private IQueryable<Dispatch> GetDispatches()
        {
            return from car in connection.Car
                        where car.Selected
                        group car by car.City into dispatch
                        select new Dispatch()
                        {
                            City = dispatch.Key,
                            Quantity = dispatch.Count()
                        };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, RequestMessage = message });
        }
    }
}
