namespace CinemaApplication.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        [Required]
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
        [Required]
        public int MovieTheaterId { get; set; }
        public MovieTheater? MovieTheater { get; set; }
        [Required]
        [Range(1, 20)]
        public int SeatsCount { get; set; }
        [Required]
        public decimal PricePerSeat { get; set; }
        public decimal TotalPrice => SeatsCount * PricePerSeat;
        public ICollection<CartItemSeat>? SelectedSeats { get; set; }
    }
}
