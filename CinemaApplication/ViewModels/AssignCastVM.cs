namespace CinemaApplication.ViewModels
{
    public class AssignCastVM
    {
        public IEnumerable<Actor>? Actors { get; set; }
        public int MovieId { get; set; }
        public List<int> SelectedActorsIds { get; set; }

    }
}
