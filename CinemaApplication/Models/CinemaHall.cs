
namespace CinemaApplication.Models
{
    public class CinemaHall
    {
        public int Id { get; set; }
   
        [StringValidation]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;
        public string? Image { get; set; }
        [Required]
        [Range(1,500)]
        public int TotalSeats { get; set; }
        public ICollection<MovieTheater>? MovieTheaters { get; set; }
        public ICollection<Seat>? Seats { get; set; }
    }
}
