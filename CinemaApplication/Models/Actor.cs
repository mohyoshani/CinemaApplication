using Microsoft.VisualBasic;


namespace CinemaApplication.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [StringValidationAttribute]
        public string Name { get; set; }

        public string? Image { get; set; }
        [Required]
        public string? Nationality { get; set; }

        public ICollection<MovieActor>? MovieActors { get; set; }

    }
}
