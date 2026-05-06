namespace CinemaApplication.Models
{
    public class Cart
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<CartItem>? CartItems { get; set; }
        public decimal TotalPrice => CartItems?.Sum(i => i.TotalPrice) ?? 0;
    }
}