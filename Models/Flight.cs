using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;


namespace Group1Flight.Models
{
    public class Flight
    {
        public int FlightId { get; set; }

       [Required]
                [RegularExpression(@"^[a-zA-Z]{2}\d{1,4}$", ErrorMessage = "Flight Code must start with 2 letters followed by 1-4 digits.")]
               
                [Remote(action: "CheckFlightCode", "Validation", areaName: "Airlines", AdditionalFields = "Date")]
                public string FlightCode { get; set; } = string.Empty;
        [Required]
        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "City name must contain letters only.")]
        public string From { get; set; } = String.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "City name must contain letters only.")]
        public string To { get; set; } = String.Empty;

        [Required]
        [DataType(DataType.Date)]
        [FutureDate(3)] 
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 5000, ErrorMessage = "Emission cannot exceed 5000kg CO2e.")]
        public double Emission { get; set; }

        [Required]
        [Range(0, 50000, ErrorMessage = "Price must be between 0 and 50,000 USD.")]
        public decimal Price { get; set; }

        
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }

        public int AirlineId { get; set; }
        public Airline? Airline { get; set; }
        public string? CabinType { get; set; }
        public string? AircraftType { get; set; }
    }
}