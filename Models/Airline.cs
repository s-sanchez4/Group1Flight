using System.ComponentModel.DataAnnotations;

namespace Group1Flight.Models
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

        // Navigation property to link back to Flights if needed
        public virtual ICollection<Flight>? Flights { get; set; }
    }
}