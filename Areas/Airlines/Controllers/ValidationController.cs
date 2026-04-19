using Microsoft.AspNetCore.Mvc;
using Group1Flight.Models;

[Area("Airlines")]
public class ValidationController : Controller
{
    private readonly FlightContext _context;
    public ValidationController(FlightContext context) => _context = context;

    [AcceptVerbs("GET", "POST")]
   [AcceptVerbs("GET", "POST")]
public IActionResult CheckFlightCode([Bind(Prefix = "Flight")] string flightCode, [Bind(Prefix = "Flight")] DateTime date)
{
    // Now 'flightCode' and 'date' will correctly catch the data from the ViewModel
    bool exists = _context.Flights.Any(f => f.FlightCode == flightCode && f.Date.Date == date.Date);

    if (exists)
    {
        return Json($"Flight {flightCode} is already scheduled for {date.ToShortDateString()}.");
    }

    return Json(true); // This tells the browser "Green light! You can submit."
}
}