namespace CinemaApplication.ViewModels
{
    public class CreateMovieVM
    {
        public IEnumerable<Category>? Categories { get; set; }
        public Movie? Movie { get; set; }
    }
}
