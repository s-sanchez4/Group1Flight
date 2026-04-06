using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Group1Flight.Models; // Or wherever your Flight model is
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Group1Flight.Controllers
{
    public class FlightsController : Controller
    {
        // This is the "_context" I was referring to. 
        // It's a private variable that holds your database connection.
        private readonly FlightContext _context;

        // The "Constructor": This is where the app 'injects' the database 
        // connection defined in Program.cs into this specific controller.
        public FlightsController(FlightContext context)
        {
            _context = context;
        }

public async Task<IActionResult> Index(string fromCity, string toCity, DateTime? departureDate, string cabinType)
{
    // 1. POPULATE DROPDOWNS
    // We get all flights once to fill the sidebar menus
    var allFlights = await _context.Flights.Include(f => f.Airline).ToListAsync();
    
    ViewBag.FromCities = new SelectList(allFlights.Select(f => f.From).Distinct().OrderBy(c => c).ToList());
    ViewBag.ToCities = new SelectList(allFlights.Select(f => f.To).Distinct().OrderBy(c => c).ToList());

    // 2. START FILTERING
    // We start with the query that INCLUDES the Airline table
    var query = _context.Flights.Include(f => f.Airline).AsQueryable();

    // Filter by 'From' city
    if (!string.IsNullOrEmpty(fromCity))
    {
        query = query.Where(f => f.From == fromCity);
    }

    // Filter by 'To' city
    if (!string.IsNullOrEmpty(toCity))
    {
        query = query.Where(f => f.To == toCity);
    }

    // Filter by Date
    if (departureDate.HasValue && Request.Query.ContainsKey("departureDate"))
    {
        query = query.Where(f => f.Date.Date == departureDate.Value.Date);
    }

    // Filter by Cabin Type
    if (!string.IsNullOrEmpty(cabinType) && cabinType != "All")
    {
        query = query.Where(f => f.CabinType == cabinType);
    }

    // 3. SHOW ALL BY DEFAULT 
    // If no search parameters were provided, show the whole list (with Airlines!)
    if (string.IsNullOrEmpty(fromCity) && string.IsNullOrEmpty(toCity) && !departureDate.HasValue)
    {
        return View(allFlights);
    }

    // 4. RETURN FILTERED RESULTS
    return View(await query.ToListAsync());
}
    
    public async Task<IActionResult> Details(int id, string fromCity, string toCity, DateTime? departureDate, string cabinType)
{
    var flight = await _context.Flights
        .Include(f => f.Airline)
        .FirstOrDefaultAsync(m => m.FlightId == id);

    if (flight == null)
    {
        return NotFound();
    }

    // Keep the search criteria in ViewBag so the "Back" button can use them
    ViewBag.FromCity = fromCity;
    ViewBag.ToCity = toCity;
    ViewBag.DepartureDate = departureDate?.ToString("yyyy-MM-dd");
    ViewBag.CabinType = cabinType;

    return View(flight);
}
// POST: Flights/Select/5
[HttpPost]
public IActionResult Select(int id)
{
    string cookieName = "SelectedFlights";
    string? existingCookie = Request.Cookies[cookieName];
    
    // 1. Get current list of IDs from cookie
    List<string> selectedIds = string.IsNullOrEmpty(existingCookie) 
        ? new List<string>() 
        : existingCookie.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

    // 2. Add the new ID if it's not already there
    if (!selectedIds.Contains(id.ToString()))
    {
        selectedIds.Add(id.ToString());
    }

    // 3. Save back to Cookie with 14-day expiry
    CookieOptions options = new CookieOptions 
    { 
        Expires = DateTime.Now.AddDays(14),
        HttpOnly = false, // Set to false if you want to access it via JS, though True is safer
        IsEssential = true, // Required for the cookie to work despite tracking preventions
        Path = "/" // Important: Makes the cookie available on all pages (Index, Details, etc)
    };
    
    Response.Cookies.Append(cookieName, string.Join(",", selectedIds), options);

    // 4. Redirect back to Details or go to Selections
    return RedirectToAction("Details", new { id = id, status = "success" });
}
// GET: Flights/Selections
public async Task<IActionResult> Selections()
{
    string? existingCookie = Request.Cookies["SelectedFlights"];
    if (string.IsNullOrEmpty(existingCookie))
    {
        return View(new List<Flight>());
    }

    var selectedIds = existingCookie.Split(',').Select(int.Parse).ToList();
    var flights = await _context.Flights
        .Include(f => f.Airline)
        .Where(f => selectedIds.Contains(f.FlightId))
        .ToListAsync();

    return View(flights);
}

// POST: Flights/ClearSelections
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
        selectedIds.Remove(id.ToString()); // Remove the specific ID

        CookieOptions options = new CookieOptions { Expires = DateTime.Now.AddDays(14) };
        
        if (selectedIds.Any())
        {
            Response.Cookies.Append(cookieName, string.Join(",", selectedIds), options);
        }
        else
        {
            Response.Cookies.Delete(cookieName);
        }
    }

    return RedirectToAction("Selections");
}
}
}