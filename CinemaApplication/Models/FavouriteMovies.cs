namespace CinemaApplication.Models
{
    public class FavouriteMovies
    {
        public int Id { get; set; }

        public int movieId { get; set; }
        public Movie movie { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int MovieCount { get; set; }
    }
}
