namespace CinemaApplication.Models
{
    public enum OrderStatus { Pending, Confirmed, Cancelled, Expired }

    public class Order  
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? PaymentReference { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public decimal TotalPrice => OrderItems?.Sum(i => i.TotalPrice) ?? 0;
    }


}
