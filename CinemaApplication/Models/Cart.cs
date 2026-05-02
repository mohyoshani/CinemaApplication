namespace CinemaApplication.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int movieId { get; set; }
        public Movie movie { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int Seats { get; set; }
        public double PricePerMovie { get; set; }

        public double TotalPrice { get; set; }
    }
}
