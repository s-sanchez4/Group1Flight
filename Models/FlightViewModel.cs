using Microsoft.AspNetCore.Mvc.Rendering;

namespace Group1Flight.Models
{
    public class FlightViewModel
    {
        // The flight being created or edited
        public Flight Flight { get; set; } = new();

        // For the Airline Dropdown
        public IEnumerable<SelectListItem> AirlineList { get; set; } = new List<SelectListItem>();

        // Requirement #3: Predefined Data as Static Lists
        public static List<string> AircraftTypes = new() 
        { 
            "Airbus A319", "Airbus A320", "Airbus A321", 
            "Boeing 737-800", "Boeing 737 MAX 8", "Boeing 737 MAX 9" 
        };

        public static List<string> CabinTypes = new() 
        { 
            "Economy", "Economy Plus", "Business", "First Class" 
        };
    }
}