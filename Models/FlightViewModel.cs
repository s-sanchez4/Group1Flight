using Microsoft.AspNetCore.Mvc.Rendering;

namespace Group1Flight.Models
{
    public class FlightViewModel
    {
        
        public Flight Flight { get; set; } = new();

       
        public IEnumerable<SelectListItem> AirlineList { get; set; } = new List<SelectListItem>();

        
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