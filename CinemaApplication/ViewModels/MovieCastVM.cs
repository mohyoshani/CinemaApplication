using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CinemaApplication.ViewModels
{
    public class MovieCastVM
    {
        public string Query { get; set; }
        public IEnumerable<Movie> movies { get; set; }

        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }


    }
}
