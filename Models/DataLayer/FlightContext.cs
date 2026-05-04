using Microsoft.EntityFrameworkCore;
using Group1Flight.Models.DomainModels;
using Group1Flight.Models.DataLayer.Configuration; // Add this specific using

namespace Group1Flight.Models.DataLayer
{
    public class FlightContext : DbContext
    {
        public FlightContext(DbContextOptions<FlightContext> options) : base(options) { }

        public DbSet<Airline> Airlines { get; set; } = null!;
        public DbSet<Flight> Flights { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfiguration(new Configuration.AirlineConfig());
    modelBuilder.ApplyConfiguration(new Configuration.FlightConfig());

   
    modelBuilder.Entity<Flight>()
        .HasIndex(f => new { f.FlightCode, f.Date }) 
        .IsUnique();

    modelBuilder.Entity<FlightNumber>().HasData(
        new FlightNumber { FlightNumberId = "FN001", Name = "American Airlines" }
    );
}
        }
    }
