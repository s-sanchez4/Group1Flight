using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Group1Flight.Models; 
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Group1Flight.Controllers
{
    public class FlightsController : Controller
    {
        
        private readonly FlightContext _context;

        
        public FlightsController(FlightContext context)
        {
            _context = context;
        }

public async Task<IActionResult> Index(string fromCity, string toCity, DateTime? departureDate, string cabinType)
{
    
    var allFlights = await _context.Flights.Include(f => f.Airline).ToListAsync();
    
    ViewBag.FromCities = new SelectList(allFlights.Select(f => f.From).Distinct().OrderBy(c => c).ToList());
    ViewBag.ToCities = new SelectList(allFlights.Select(f => f.To).Distinct().OrderBy(c => c).ToList());

   
    var query = _context.Flights.Include(f => f.Airline).AsQueryable();

    
    if (!string.IsNullOrEmpty(fromCity))
    {
        query = query.Where(f => f.From == fromCity);
    }

    if (!string.IsNullOrEmpty(toCity))
    {
        query = query.Where(f => f.To == toCity);
    }

    
    if (departureDate.HasValue && Request.Query.ContainsKey("departureDate"))
    {
        query = query.Where(f => f.Date.Date == departureDate.Value.Date);
    }

    
    if (!string.IsNullOrEmpty(cabinType) && cabinType != "All")
    {
        query = query.Where(f => f.CabinType == cabinType);
    }

   
    if (string.IsNullOrEmpty(fromCity) && string.IsNullOrEmpty(toCity) && !departureDate.HasValue)
    {
        return View(allFlights);
    }

   
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

    
    ViewBag.FromCity = fromCity;
    ViewBag.ToCity = toCity;
    ViewBag.DepartureDate = departureDate?.ToString("yyyy-MM-dd");
    ViewBag.CabinType = cabinType;

    return View(flight);
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

    
    CookieOptions options = new CookieOptions 
    { 
        Expires = DateTime.Now.AddDays(14),
        HttpOnly = false, 
        IsEssential = true, 
    };
    
    Response.Cookies.Append(cookieName, string.Join(",", selectedIds), options);

    
   return RedirectToAction("Details", "Home", new { id = id, status = "success" });
}

public async Task<IActionResult> Selections()
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

    var flights = await _context.Flights
        .Include(f => f.Airline)
        .Where(f => selectedIds.Contains(f.FlightId))
        .ToListAsync();

    return View(flights); 
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