namespace CinemaApplication.ViewModels
{
    public class ActorsVM
    {
        public IEnumerable<Actor> Actors { get; set; }
        public int CurrentPage { get; set; }
        public double TotalPages { get; set; }

        public string? Query { get; set; }
    }
}
