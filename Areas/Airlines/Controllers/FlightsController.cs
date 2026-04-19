using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group1Flight.Models;
using Group1Flight.Extensions;

namespace Group1Flight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    [Route("Airlines/[controller]/[action]")]
    public class FlightsController : Controller
    {
        private readonly FlightContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FlightsController(FlightContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["BodyClass"] = "airlines-bg";
            var sessionFilter = HttpContext.Session.GetObject<FlightViewModel>("UserFilter");
            var flightsQuery = _context.Flights.Include(f => f.Airline).AsQueryable();

            if (sessionFilter?.Flight != null)
            {
                if (!string.IsNullOrEmpty(sessionFilter.Flight.From))
                    flightsQuery = flightsQuery.Where(f => f.From == sessionFilter.Flight.From);

                if (!string.IsNullOrEmpty(sessionFilter.Flight.To))
                    flightsQuery = flightsQuery.Where(f => f.To == sessionFilter.Flight.To);

                if (sessionFilter.Flight.AirlineId != 0)
                    flightsQuery = flightsQuery.Where(f => f.AirlineId == sessionFilter.Flight.AirlineId);
            }

            var flights = await flightsQuery.ToListAsync();
            return View(flights);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["BodyClass"] = "airlines-bg";
            var viewModel = new FlightViewModel
            {
                AirlineList = _context.Airlines.Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.AirlineId.ToString()
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlightViewModel viewModel)
        {
            // 1. Prepare clean data for comparison
            var newCode = viewModel.Flight.FlightCode?.Trim().ToUpper() ?? "";
            var newDate = viewModel.Flight.Date.Date;

            // 2. THE STRENGTHENED CHECK (Explicit comparison for Year/Month/Day)
            bool isDuplicate = await _context.Flights.AnyAsync(f => 
                f.FlightCode.ToUpper() == newCode && 
                f.Date.Year == newDate.Year &&
                f.Date.Month == newDate.Month &&
                f.Date.Day == newDate.Day);

            if (isDuplicate)
            {
                // Adding this error forces ModelState.IsValid to be false
                ModelState.AddModelError("Flight.FlightCode", "STOP: This flight code is already scheduled for this date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(viewModel.Flight);
                await _context.SaveChangesAsync();
                
                TempData["Message"] = $"Success: Flight {viewModel.Flight.FlightCode} added.";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            // 3. RE-POPULATE list and return view if validation failed
            viewModel.AirlineList = _context.Airlines.Select(a => new SelectListItem 
            { 
                Text = a.Name, 
                Value = a.AirlineId.ToString() 
            });

            ViewData["BodyClass"] = "airlines-bg";
            return View(viewModel);
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound();

            ViewData["BodyClass"] = "airlines-bg";
            var viewModel = new FlightViewModel
            {
                Flight = flight,
                AirlineList = _context.Airlines.Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.AirlineId.ToString()
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        [Route("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlightViewModel viewModel)
        {
            if (id != viewModel.Flight.FlightId) return NotFound();

            // Optional: You can also add the duplicate check here for Edits!
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Flight);
                    await _context.SaveChangesAsync();
                    
                    TempData["Message"] = "Flight updated successfully!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Flights.Any(e => e.FlightId == id)) return NotFound();
                    else throw;
                }
            }

            viewModel.AirlineList = _context.Airlines.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.AirlineId.ToString()
            });

            ViewData["BodyClass"] = "airlines-bg";
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var flight = await _context.Flights.Include(f => f.Airline).FirstOrDefaultAsync(m => m.FlightId == id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Flight deleted.";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}