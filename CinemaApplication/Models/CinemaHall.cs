
namespace CinemaApplication.Models
{
    public class CinemaHall
    {
        public int Id { get; set; }
        [Required]
        [StringValidationAttribute] 
        public string Name { get; set; } 
        [Required]
        public string? Location { get; set; } 
        public string? Image { get; set; }
        public ICollection<MovieTheater>? MovieTheaters { get; set; }
    }
}
