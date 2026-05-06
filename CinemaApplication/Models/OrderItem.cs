namespace CinemaApplication.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        [Required]
        public int MovieTheaterId { get; set; }
        public MovieTheater? MovieTheater { get; set; }
        [Required]
        [Range(1, 20)]
        public int SeatsCount { get; set; }
        [Required]
        public decimal PricePerSeat { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<OrderItemSeat>? BookedSeats { get; set; }
    }
}
