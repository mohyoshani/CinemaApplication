namespace CinemaApplication.Models
{
    public class OrderItemSeat
    {
        public int Id { get; set; }
        [Required]
        public int OrderItemId { get; set; }
        public OrderItem? OrderItem { get; set; }
        [Required]
        public int SeatId { get; set; }
        public Seat? Seat { get; set; } 
    }
}
