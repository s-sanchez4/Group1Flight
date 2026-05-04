using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Group1Flight.Models.DomainModels; // This MUST match the namespace in Airline.cs
using Group1Flight.Models.ViewModels;
using Group1Flight.Models.DataLayer.Repositories;

namespace Group1Flight.Areas.Airlines.Controllers
{
    [Area("Airlines")]
    [Route("Airlines/[controller]/[action]")]
    public class FlightsController : Controller
    {
        // Use interfaces instead of the concrete DbContext
        private IRepository<Flight> flightData { get; set; }
        private IRepository<Group1Flight.Models.DomainModels.Airline> airlineData { get; set; }

        public FlightsController(IRepository<Flight> flightRep, IRepository<Airline> airlineRep)
        {
            flightData = flightRep;
            airlineData = airlineRep;
        }

        public IActionResult Index()
        {
            ViewData["BodyClass"] = "airlines-bg";
            
            // Use QueryOptions to handle the eager loading of the Airline entity
            var options = new QueryOptions<Flight> {
                Includes = new[] { "Airline" },
                
            };

            var flights = flightData.List(options);
            return View(flights);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["BodyClass"] = "airlines-bg";
            var viewModel = new FlightViewModel
            {
                // Retrieve the list of airlines through the repository
                AirlineList = airlineData.List(new QueryOptions<Airline>())
                    .Select(a => new SelectListItem
                    {
                        Text = a.Name,
                        Value = a.AirlineId.ToString()
                    })
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FlightViewModel viewModel)
        {
            // Logic to check for duplicate flight codes
            var existing = flightData.Get(new QueryOptions<Flight> {
                Where = f => f.FlightCode == viewModel.Flight.FlightCode && 
                             f.Date == viewModel.Flight.Date
            });

            if (existing != null)
            {
                ModelState.AddModelError("Flight.FlightCode", "STOP: This flight code is already scheduled for this date.");
            }

            if (ModelState.IsValid)
            {
                flightData.Insert(viewModel.Flight);
                flightData.Save(); // Persist changes to the database
                
                TempData["Message"] = $"Success: Flight {viewModel.Flight.FlightCode} added.";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            // Re-populate the dropdown if validation fails
            viewModel.AirlineList = airlineData.List(new QueryOptions<Airline>())
                .Select(a => new SelectListItem 
                { 
                    Text = a.Name, 
                    Value = a.AirlineId.ToString() 
                });

            ViewData["BodyClass"] = "airlines-bg";
            return View(viewModel);
        }

        [HttpGet]
        [Route("{id?}")]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var flight = flightData.Get((int)id);
            if (flight == null) return NotFound();

            ViewData["BodyClass"] = "airlines-bg";
            var viewModel = new FlightViewModel
            {
                Flight = flight,
                AirlineList = airlineData.List(new QueryOptions<Airline>())
                    .Select(a => new SelectListItem
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
        public IActionResult Edit(int id, FlightViewModel viewModel)
        {
            if (id != viewModel.Flight.FlightId) return NotFound();

            if (ModelState.IsValid)
            {
                flightData.Update(viewModel.Flight);
                flightData.Save();
                
                TempData["Message"] = "Flight updated successfully!";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            viewModel.AirlineList = airlineData.List(new QueryOptions<Airline>())
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.AirlineId.ToString()
                });

            ViewData["BodyClass"] = "airlines-bg";
            return View(viewModel);
        }
                        [HttpPost, ActionName("Delete")]
                        [ValidateAntiForgeryToken]
                        public IActionResult DeleteConfirmed(int id)
                        {
                            var flight = flightData.Get(id);

                            if (flight != null)
                            {
                                // PHASE 4 REQUIREMENT: Check if the flight is reserved before deleting
                                if (flight.IsReserved)
                                {
                                    TempData["Message"] = $"CONFLICT: Flight {flight.FlightCode} is reserved and cannot be deleted.";
                                    TempData["MessageType"] = "danger";
                                    
                                    // Sending the user back to the airline dashboard/index with an error
                                    return RedirectToAction(nameof(Index)); 
                                }

                                flightData.Delete(flight);
                                flightData.Save();
                                TempData["Message"] = "Flight deleted successfully.";
                                TempData["MessageType"] = "success";
                            }
                            
                            return RedirectToAction(nameof(Index));
                        }
                            }
}