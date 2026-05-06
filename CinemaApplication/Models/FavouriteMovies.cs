namespace CinemaApplication.Models
{
    public class FavouriteMovies
    {
        public int Id { get; set; }

        public int MovieId { get; set; }      
        public Movie? Movie { get; set; }      

        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
