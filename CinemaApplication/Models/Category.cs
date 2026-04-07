namespace CinemaApplication.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringValidationAttribute]
        public string Name { get; set; }
    }
}
