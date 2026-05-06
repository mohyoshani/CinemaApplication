namespace CinemaApplication.Models
{
    public class Seat
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(5)]
        public string Row { get; set; } = string.Empty;
        [Required]
        [Range(1, 100)]
        public int Number { get; set; }
        public string SeatLabel => $"{Row}{Number}";
        [Required]
        public int CinemaHallId { get; set; }
        public CinemaHall? CinemaHall { get; set; }
        public ICollection<CartItemSeat>? CartItemSeats { get; set; }
        public ICollection<OrderItemSeat>? OrderItemSeats { get; set; }
    }
}
