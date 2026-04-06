using System;
using System.ComponentModel.DataAnnotations;
// 1. Add this using statement to access ValidateNever
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Group1Flight.Models
{
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }
        
        public int AirlineId { get; set; }

        // 2. Add the [ValidateNever] attribute here
        // This stops the Edit/Create post from failing just because 
        // the full Airline object isn't sent with the form.
        [ValidateNever]
        public virtual Airline? Airline { get; set; }

        [Required]
        [StringLength(10)]
        public string FlightCode { get; set; } = string.Empty;

        [Required]
        public string From { get; set; } = string.Empty;

        [Required]
        public string To { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public TimeSpan ArrivalTime { get; set; }

        public string CabinType { get; set; } = string.Empty;

        public double Emission { get; set; }

        public string AircraftType { get; set; } = string.Empty;

        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }
    }
}