using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Fixes 'SelectList' error
using Microsoft.EntityFrameworkCore;      // Fixes '.Include' error
using Group1Flight.Models;
using Group1Flight.Extensions;           // Fixes 'SetObject/GetObject' error

namespace Group1Flight.Controllers
{
    public class HomeController : Controller
    {
        // 1. ADD THIS: This allows the controller to talk to your database
        private readonly FlightContext _context;

        public HomeController(FlightContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            // READ from Session
            var sessionModel = HttpContext.Session.GetObject<FlightViewModel>("UserFilter") ?? new FlightViewModel();

            // 2. FIX: Use _context (the private variable) instead of the Class name
            var flights = _context.Flights.Include(f => f.Airline).AsQueryable();

            if (sessionModel.Flight != null)
            {
                if (!string.IsNullOrEmpty(sessionModel.Flight.From))
                    flights = flights.Where(f => f.From == sessionModel.Flight.From);
                    
                if (!string.IsNullOrEmpty(sessionModel.Flight.To))
                    flights = flights.Where(f => f.To == sessionModel.Flight.To);
            }

            ViewBag.Flights = flights.ToList();

            // 3. FIX: _context is now recognized here
            ViewBag.FromCities = new SelectList(_context.Flights.Select(f => f.From).Distinct());
            ViewBag.ToCities = new SelectList(_context.Flights.Select(f => f.To).Distinct());

            // The "~" starts at the root of your project
            return View("~/Views/Home/Index.cshtml", sessionModel);
        }

        [HttpPost]
        public IActionResult Index(FlightViewModel model)
        {
            HttpContext.Session.SetObject("UserFilter", model);
            
            // Note: If you want them to stay on the home page to see results:
            // return RedirectToAction("Index");
            
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() => View("Privacy");
        public IActionResult Admin() => View("Admin");
        public IActionResult Airlines() => View("Airlines");
        public IActionResult Details(int id)
        {
            // Find the specific flight and include the Airline info for the logo
            var flight = _context.Flights
                .Include(f => f.Airline)
                .FirstOrDefault(f => f.FlightId == id);

            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    } 
}