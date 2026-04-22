namespace CinemaApplication.ViewModels
{
    public class ResendConfirmationVM
    {
        public int Id { get; set; }
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
