namespace CinemaApplication.Models
{
    public class MovieTheater
    {
        public int Id { get; set; }
        public DateTime Showtime { get; set; }
        public int CinemaHallId { get; set; }
        public int MovieId { get; set; }
        public CinemaHall? CinemaHall { get; set; }
        public Movie? Movie { get; set; }
    }
}
