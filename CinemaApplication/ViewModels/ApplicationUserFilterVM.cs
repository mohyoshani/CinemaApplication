namespace CinemaApplication.ViewModels
{
    public class ApplicationUserFilterVM
    {
        public Dictionary<ApplicationUser, string> UserRoles { get; set; } = null!;
        public string? query { get; set; }
        public double totalPages { get; set; }
         public int currentPage { get; set; }
    }
}
