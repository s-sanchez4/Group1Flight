using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Group1Flight.Models.DomainModels;
using Group1Flight.Models.ViewModels;
using Group1Flight.Models.DataLayer.Repositories; 
// Update this if your session extensions are in a subfolder
using Group1Flight.Models.ExtensionMethods; 
using Group1Flight.Models;

namespace Group1Flight.Controllers
{
    public class HomeController : Controller
    {
        // Use the Repository interface instead of FlightContext
        private IRepository<Flight> flightData { get; set; }

        public HomeController(IRepository<Flight> flightRep)
        {
            flightData = flightRep;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            // Use the updated namespace for Session Extensions
            var sessionModel = HttpContext.Session.GetObject<FlightViewModel>("UserFilter") ?? new FlightViewModel();

            // Set up QueryOptions for Eager Loading the Airline
            var options = new QueryOptions<Flight> {
                Includes = new[] { "Airline" }
            };

            // Retrieve flights through the repository
            var flights = flightData.List(options);

            // Apply filtering logic if session data exists
            if (sessionModel.Flight != null)
            {
                if (!string.IsNullOrEmpty(sessionModel.Flight.From))
                    flights = flights.Where(f => f.From == sessionModel.Flight.From);
                    
                if (!string.IsNullOrEmpty(sessionModel.Flight.To))
                    flights = flights.Where(f => f.To == sessionModel.Flight.To);
            }

            ViewBag.Flights = flights.ToList();

            // Populate dropdowns using the Repository
            ViewBag.FromCities = new SelectList(flightData.List(new QueryOptions<Flight>())
                .Select(f => f.From).Distinct());
            ViewBag.ToCities = new SelectList(flightData.List(new QueryOptions<Flight>())
                .Select(f => f.To).Distinct());

            return View("~/Views/Home/Index.cshtml", sessionModel);
        }

        [HttpPost]
        public IActionResult Index(FlightViewModel model)
        {
            HttpContext.Session.SetObject("UserFilter", model);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() => View("Privacy");
        public IActionResult Admin() => View("Admin");
        public IActionResult Airlines() => View("Airlines");

        public IActionResult Details(int id)
        {
            // Use QueryOptions to find a specific flight with its Airline
            var options = new QueryOptions<Flight> {
                Includes = new[] { "Airline" },
                Where = f => f.FlightId == id
            };

            var flight = flightData.Get(options);

            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Note: Ensure ErrorViewModel is correctly namespaced in Models
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    } 
}