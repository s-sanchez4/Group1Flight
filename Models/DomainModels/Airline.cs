using System.ComponentModel.DataAnnotations;

namespace Group1Flight.Models.DomainModels
{
    public class Airline
    {
        [Key]
        public int AirlineId { get; set; }

        [Required]
        [Display(Name = "Airline Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Logo Image")]
        public string? ImageName { get; set; }

                public virtual ICollection<Flight>? Flights { get; set; }
    }
}