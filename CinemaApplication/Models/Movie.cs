
namespace CinemaApplication.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [StringValidation]
        
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        
        [Required]
        [Range(1 , 500)]
        public decimal Price { get; set; }
        
        public string? Mainimage { get; set; }
        
        [Required]
        public bool status { get; set; }
        
        [Required]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int Duration { get; set; }
        public Category? Category { get; set; }
        public ICollection<MovieActor>? MovieActors { get; set; }
        public ICollection<MovieTheater>? MovieTheaters { get; set; }
        public ICollection<MovieImage>? MovieImages { get; set; }

    }
}
