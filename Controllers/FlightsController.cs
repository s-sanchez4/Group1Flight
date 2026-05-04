using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Group1Flight.Models.DomainModels;
using Group1Flight.Models.DataLayer;
using Group1Flight.Models.DataLayer.Repositories; 

namespace Group1Flight.Controllers
{
    public class FlightsController : Controller
    {
        private IRepository<Flight> flightData { get; set; }

        public FlightsController(IRepository<Flight> flightRep)
        {
            flightData = flightRep;
        }

        // 1. Removed 'async' and 'Task' - changed to standard IActionResult
        public IActionResult Index(string fromCity, string toCity, DateTime? departureDate, string cabinType)
        {
            // 2. Changed ListAsync() to List()
            var allFlights = flightData.List(new QueryOptions<Flight>());
            
            ViewBag.FromCities = new SelectList(allFlights.Select(f => f.From).Distinct().OrderBy(c => c).ToList());
            ViewBag.ToCities = new SelectList(allFlights.Select(f => f.To).Distinct().OrderBy(c => c).ToList());

            var options = new QueryOptions<Flight> {
                Includes = new[] { "Airline" }
            };

            // Filtering logic remains the same
            if (!string.IsNullOrEmpty(fromCity))
                options.Where = f => f.From == fromCity;

            if (!string.IsNullOrEmpty(toCity))
                options.Where = f => f.To == toCity;

            if (departureDate.HasValue)
                options.Where = f => f.Date.Date == departureDate.Value.Date;

            if (!string.IsNullOrEmpty(cabinType) && cabinType != "All")
                options.Where = f => f.CabinType == cabinType;

            // 3. Removed 'await' and used List()
            var filteredFlights = flightData.List(options);
            return View(filteredFlights);
        }

        public IActionResult Details(int id, string fromCity, string toCity, DateTime? departureDate, string cabinType)
        {
            var options = new QueryOptions<Flight> {
                Includes = new[] { "Airline" },
                Where = f => f.FlightId == id
            };

            // 4. Changed GetAsync() to Get()
            var flight = flightData.Get(options);

            if (flight == null) return NotFound();

            ViewBag.FromCity = fromCity;
            ViewBag.ToCity = toCity;
            ViewBag.DepartureDate = departureDate?.ToString("yyyy-MM-dd");
            ViewBag.CabinType = cabinType;

            return View(flight);
        }

        public IActionResult Selections()
        {
            string? existingCookie = Request.Cookies["SelectedFlights"];
            if (string.IsNullOrEmpty(existingCookie))
            {
                return View(new List<Flight>());
            }

            var selectedIds = existingCookie.Split(',')
                                            .Where(s => !string.IsNullOrEmpty(s)) 
                                            .Select(int.Parse)
                                            .ToList();

            var options = new QueryOptions<Flight> {
                Includes = new[] { "Airline" },
                Where = f => selectedIds.Contains(f.FlightId)
            };

            // 5. Changed ListAsync() to List()
            var flights = flightData.List(options);
            return View(flights); 
        }

        [HttpPost]
        public IActionResult Select(int id)
        {
            string cookieName = "SelectedFlights";
            string? existingCookie = Request.Cookies[cookieName];
            
            List<string> selectedIds = string.IsNullOrEmpty(existingCookie) 
                ? new List<string>() 
                : existingCookie.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (!selectedIds.Contains(id.ToString()))
            {
                selectedIds.Add(id.ToString());
            }

            CookieOptions options = new CookieOptions { 
                Expires = DateTime.Now.AddDays(14),
                HttpOnly = false, 
                IsEssential = true, 
            };
            
            Response.Cookies.Append(cookieName, string.Join(",", selectedIds), options);
            return RedirectToAction("Details", "Home", new { id = id, status = "success" });
        }

        public IActionResult ClearSelections()
        {
            Response.Cookies.Delete("SelectedFlights");
            return RedirectToAction("Selections");
        }

        [HttpPost]
        public IActionResult RemoveSelection(int id)
        {
            string cookieName = "SelectedFlights";
            string? existingCookie = Request.Cookies[cookieName];
            
            if (!string.IsNullOrEmpty(existingCookie))
            {
                var selectedIds = existingCookie.Split(',').ToList();
                selectedIds.Remove(id.ToString()); 

                CookieOptions options = new CookieOptions { Expires = DateTime.Now.AddDays(14) };
                
                if (selectedIds.Any())
                    Response.Cookies.Append(cookieName, string.Join(",", selectedIds), options);
                else
                    Response.Cookies.Delete(cookieName);
            }
            return RedirectToAction("Selections");
        }
        [HttpPost]
                public IActionResult Reserve(int id)
                {
                    var flight = flightData.Get(id);
                    if (flight != null)
                    {
                        flight.IsReserved = true; 
                        flightData.Update(flight);
                        flightData.Save();
                        TempData["Message"] = "Flight successfully reserved.";
                    }
                    return RedirectToAction("Selections", "Flights"); 
}
    }
}