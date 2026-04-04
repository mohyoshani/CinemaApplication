using Microsoft.VisualBasic;

namespace CinemaApplication.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Image { get; set; }

        public string? Nationality { get; set; }

        public ICollection<MovieActor>? MovieActors { get; set; }

    }
}
