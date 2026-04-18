namespace CinemaApplication.ViewModels
{
    public class LoginVM
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Username or Email is required")]
        [Display(Name = "Username or Email")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
