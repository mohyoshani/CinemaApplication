namespace CinemaApplication.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Mainimage { get; set; }
        public bool status { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int CategoryId { get; set; }
        public int Duration { get; set; }
        public Category Category { get; set; }
        public ICollection<MovieActor>? MovieActors { get; set; }
        public ICollection<MovieTheater>? MovieTheaters { get; set; }
        public ICollection<MovieImage>? MovieImages { get; set; }

    }
}
