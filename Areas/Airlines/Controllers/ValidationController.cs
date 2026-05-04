using Microsoft.AspNetCore.Mvc;
using Group1Flight.Models.DataLayer; 
using Group1Flight.Models.DomainModels; 
using System.Linq;
using System;

namespace Group1Flight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    public class ValidationController : Controller
    {
        private readonly FlightContext _context;
        public ValidationController(FlightContext context) => _context = context;

        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckFlightCode(string flightCode, DateTime date)
        {
            // We use _context.Flights directly here to check the database
            var flight = _context.Flights.FirstOrDefault(f => 
                f.FlightCode == flightCode && 
                f.Date.Date == date.Date);

            if (flight == null) 
            {
                return Json(true); // No duplicate found, validation passes
            } 
            else 
            {
                // This message will appear under the FlightCode textbox in the browser
                return Json($"STOP: Flight {flightCode} is already scheduled for {date.ToShortDateString()}.");
            }
        }
    }
}