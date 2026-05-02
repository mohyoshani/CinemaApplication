namespace CinemaApplication.ViewModels
{
    public class HomeMoviesVM
    {
        public IEnumerable<Category> Categories { get; set; } = null!;
        public string? Query { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }
    }
}
