namespace CinemaApplication.ViewModels
{
    public class CategoryVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public double TotalPages { get; set; }
        public string? Query { get; set; }

        public int CurrentPage { get; set; }

    }
}
