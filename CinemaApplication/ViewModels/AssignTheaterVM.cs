namespace CinemaApplication.ViewModels
{
    public class AssignTheaterVM
    {
        public IEnumerable<CinemaHall> CinemaHalls { get; set; }
        public int MovieId { get; set; }
        public List<int> SelectedCinemaHallIds { get; set; } = new();
        public DateTime Showtime { get; set; }

    }
}
