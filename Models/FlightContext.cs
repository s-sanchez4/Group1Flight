using Microsoft.EntityFrameworkCore;

namespace Group1Flight.Models
{
    public class FlightContext : DbContext
    {
        public FlightContext(DbContextOptions<FlightContext> options)
            : base(options)
        { }

        public DbSet<Flight> Flights { get; set; } = null!;
        public DbSet<FlightNumber> FlightNumbers { get; set; } = null!;
        public DbSet<Airline> Airlines { get; set; } = null!;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ... your existing Flight seed data ...

    modelBuilder.Entity<Airline>().HasData(
        new Airline { AirlineId = 1, Name = "American Airlines", ImageName = "aa_logo.png" },
        new Airline { AirlineId = 2, Name = "Emirates", ImageName = "emirates_logo.png" },
        new Airline { AirlineId = 3, Name = "Cathay Pacific", ImageName = "cathay_logo.jpg" },
        new Airline { AirlineId = 4, Name = "Japan Airlines", ImageName = "jal_logo.png" },
        new Airline { AirlineId = 5, Name = "Air France", ImageName = "af_logo.png" }
    );
            // Seed Data for Flights
           modelBuilder.Entity<Flight>().HasData(
    new Flight { 
        FlightId = 1, 
        FlightCode = "AA101", 
        AirlineId = 1, // American Airlines
        From = "Chicago", 
        To = "New York", 
        Date = new DateTime(2026, 2, 15), 
        Price = 150m, 
        CabinType = "Economy", 
        AircraftType = "Boeing 737", 
        Emission = 120.5 
    },
    new Flight { 
        FlightId = 2, 
        FlightCode = "EK202", 
        AirlineId = 2, // Emirates
        From = "Dubai", 
        To = "London", 
        Date = new DateTime(2026, 3, 1), 
        Price = 850m, 
        CabinType = "Business", 
        AircraftType = "Airbus A380", 
        Emission = 450.0 
    },
    new Flight { 
        FlightId = 3, 
        FlightCode = "CX303", 
        AirlineId = 3, // Cathay Pacific
        From = "Hong Kong", 
        To = "San Francisco", 
        Date = new DateTime(2026, 6, 15), 
        Price = 1200m, 
        CabinType = "Economy Plus", 
        AircraftType = "Boeing 777", 
        Emission = 380.2 
    },
    new Flight { 
        FlightId = 4, 
        FlightCode = "JL404", 
        AirlineId = 4, // Japan Airlines
        From = "Los Angeles", 
        To = "Tokyo", 
        Date = new DateTime(2026, 4, 10), 
        Price = 950m, 
        CabinType = "Economy", 
        AircraftType = "Boeing 787", 
        Emission = 310.0 
    },
    new Flight { 
        FlightId = 5, 
        FlightCode = "AF505", 
        AirlineId = 5, // Air France
        From = "Paris", 
        To = "Rome", 
        Date = new DateTime(2026, 5, 20), 
        Price = 110m, 
        CabinType = "Basic Economy", 
        AircraftType = "Airbus A320", 
        Emission = 85.5 
    }
);
            // Seed Data for FlightNumbers (if needed)
            modelBuilder.Entity<FlightNumber>().HasData(
                new FlightNumber { FlightNumberId = "FN001", Name = "American Airlines" }
            );
        }
    }
}