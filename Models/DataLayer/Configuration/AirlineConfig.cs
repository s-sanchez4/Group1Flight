using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Group1Flight.Models.DomainModels;

namespace Group1Flight.Models.DataLayer.Configuration
{
    public class AirlineConfig : IEntityTypeConfiguration<Airline>
    {
        public void Configure(EntityTypeBuilder<Airline> entity)
        {
            entity.HasData(
                new Airline { AirlineId = 1, Name = "American Airlines", ImageName = "aa_logo.png" },
                new Airline { AirlineId = 2, Name = "Emirates", ImageName = "emirates_logo.png" },
                new Airline { AirlineId = 3, Name = "Cathay Pacific", ImageName = "cathay_logo.jpg" },
                new Airline { AirlineId = 4, Name = "Japan Airlines", ImageName = "jal_logo.png" },
                new Airline { AirlineId = 5, Name = "Air France", ImageName = "af_logo.png" }
            );
        }
    }
}