using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group1Flight.Models;

namespace Group1Flight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    public class Flights : Controller
    {
        private readonly FlightContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Flights(FlightContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["BodyClass"] = "airlines-bg";
            var flights = await _context.Flights.Include(f => f.Airline).ToListAsync();
            return View(flights);
        }

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
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.Flight);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"Success: Flight {viewModel.Flight.FlightCode} added.";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            viewModel.AirlineList = _context.Airlines.Select(a => new SelectListItem { Text = a.Name, Value = a.AirlineId.ToString() });
            return View(viewModel);
        }

        // --- 1. THE MISSING GET METHOD (To load the Edit page) ---
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

        // --- 2. THE CORRECTED POST METHOD (To save and redirect) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlightViewModel viewModel)
        {
            if (id != viewModel.Flight.FlightId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Flight);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = $"Success: Flight {viewModel.Flight.FlightCode} updated.";
                    TempData["MessageType"] = "success";
                    
                    // THIS IS WHAT TAKES YOU BACK TO DASHBOARD
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Flights.Any(e => e.FlightId == viewModel.Flight.FlightId)) return NotFound();
                    else throw;
                }
            }

            // If validation fails, reload dropdowns and stay on page
            viewModel.AirlineList = _context.Airlines.Select(a => new SelectListItem { Text = a.Name, Value = a.AirlineId.ToString() });
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