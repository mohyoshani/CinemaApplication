using System.ComponentModel.DataAnnotations;

namespace CinemaApplication.Models
{
    public class CinemaHall
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string? Location { get; set; } 
        public string? Image { get; set; }
        public ICollection<MovieTheater>? MovieTheaters { get; set; }
    }
}
