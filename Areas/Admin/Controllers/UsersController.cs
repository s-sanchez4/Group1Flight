using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Group1Flight.Models;
using Group1Flight.Extensions;

namespace Group1Flight.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            
            ViewData["BodyClass"] = "admin-bg"; 
            var sessionFilter = HttpContext.Session.GetObject<FlightViewModel>("UserFilter");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}