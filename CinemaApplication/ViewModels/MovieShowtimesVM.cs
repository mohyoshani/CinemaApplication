namespace CinemaApplication.ViewModels
{
    public class MovieShowtimesVM
    {
        public string Query { get; set; }
        public IEnumerable<Movie> movies { get; set; }

        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
