namespace CinemaApplication.Models
{
    public class Category
    {
        public int Id { get; set; }
   
        [StringValidation]
        [Required]
        public string Name { get; set; }
    }
}
