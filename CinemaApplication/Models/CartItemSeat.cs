namespace CinemaApplication.Models
{
    public class CartItemSeat
    {
        public int Id { get; set; }
        [Required]
        public int CartItemId { get; set; }
        public CartItem? CartItem { get; set; }
        [Required]
        public int SeatId { get; set; }
        public Seat? Seat { get; set; }
    }
}
