namespace CinemaApplication.ViewModels
{
    public class CinemaHallsVM
    {
        public IEnumerable<CinemaHall> CinemaHalls { get; set; }
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? Query { get; set; }
    }
}
