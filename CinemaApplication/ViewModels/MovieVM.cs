using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CinemaApplication.ViewModels
{
    public class MovieVM
    {
        public string Query { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
        public int CurrentPage { get; set; }
        public double totalPages { get; set; }
    }
}
