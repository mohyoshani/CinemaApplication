using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace CinemaApplication.Areas.Identity.Controllers
{
    [Area(SD.Identity)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager , IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            //User Creation
            var user = new ApplicationUser()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                Address = registerVM.Address
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(registerVM);
            }

            //Send Email Confirmation 
            var token = await  _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("Confirm" , "Account", new { area = SD.Identity, Token = token, UserId = user.Id } , Request.Scheme);
           await _emailSender.SendEmailAsync(user.Email, $"Hi {user.FirstName} {user.LastName} confirm Registeration" ,
                $"<h1>Confirm Email by Clicking <a href='{link}' > Here </a></h1>");


            TempData["success"] = "Account Registered Successfully";
            return RedirectToAction(nameof(Login), "Account", new { area = SD.Identity });
        }


        public async Task<IActionResult> Confirm(string Token , string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if(user is null)
            {
                return NotFound();
            }
            var result = await _userManager.ConfirmEmailAsync(user, Token);

            if (!result.Succeeded)
            {
              
                TempData["error"] = string.Join(",", result.Errors.Select(a => a.Description));
                return RedirectToAction(nameof(Login));
            }

            TempData["success"] = "Account Confirmed Successfully";
            return RedirectToAction("Index", "Home", new { area = SD.Admin });
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail) ?? await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("UserNameOrEmail", "User Name or Email is Invalid");
                ModelState.AddModelError("Password", "Password is Invalid");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("UserNameOrEmail", "User Name or Email is Invalid");
                ModelState.AddModelError("Password", "Password is Invalid");
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too Many Attemps, Try again after 5 minutes");
                }

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Access Denied");
                }
                return View(loginVM);
            }
            TempData["success"] = $"Welcome Back ! , {user.FirstName} {user.LastName}";

            return RedirectToAction(nameof(Index), "Home", new { area = nameof(SD.Admin) });
        }

    }
}
