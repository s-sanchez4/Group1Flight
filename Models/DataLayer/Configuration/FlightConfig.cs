using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Group1Flight.Models.DomainModels; 

namespace Group1Flight.Models.DataLayer.Configuration
{
    public class FlightConfig : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> entity)
        {
            entity.HasData(
                new Flight { 
                    FlightId = 1, FlightCode = "AA101", AirlineId = 1, 
                    From = "Chicago", To = "New York", Date = new DateTime(2026, 2, 15), 
                    Price = 150m, CabinType = "Economy", AircraftType = "Boeing 737", Emission = 120.5 
                },
                new Flight { 
                    FlightId = 2, FlightCode = "EK202", AirlineId = 2, 
                    From = "Dubai", To = "London", Date = new DateTime(2026, 3, 1), 
                    Price = 850m, CabinType = "Business", AircraftType = "Airbus A380", Emission = 450.0 
                },
                new Flight { 
                    FlightId = 3, FlightCode = "CX303", AirlineId = 3, 
                    From = "Hong Kong", To = "San Francisco", Date = new DateTime(2026, 6, 15), 
                    Price = 1200m, CabinType = "Economy Plus", AircraftType = "Boeing 777", Emission = 380.2 
                },
                new Flight { 
                    FlightId = 4, FlightCode = "JL404", AirlineId = 4, 
                    From = "Los Angeles", To = "Tokyo", Date = new DateTime(2026, 4, 10), 
                    Price = 950m, CabinType = "Economy", AircraftType = "Boeing 787", Emission = 310.0 
                },
                new Flight { 
                    FlightId = 5, FlightCode = "AF505", AirlineId = 5, 
                    From = "Paris", To = "Rome", Date = new DateTime(2026, 5, 20), 
                    Price = 110m, CabinType = "Basic Economy", AircraftType = "Airbus A320", Emission = 85.5 
                }
            );
        }
    }
}