using Humanizer;
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
        private readonly IRepository<ApplicationUserOTP> _userOTP;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> userOTP)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _userOTP = userOTP;
        }

        public IActionResult AccessDenied()
        {
            return View();
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


            await SendConfirmationEmail(user);
            await _userManager.AddToRoleAsync(user, SD.Customer_Role);

            TempData["success"] = "Account Registered Successfully";
            return RedirectToAction(nameof(Login), "Account", new { area = SD.Identity });
        }


        public async Task<IActionResult> Confirm(string Token, string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null)
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
            TempData["success"] = $"Welcome Back ! , {user.UserName}";

            return RedirectToAction(nameof(Index), "Home", new { area = nameof(SD.Admin) });
        }


        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendConfirmationVM confirmationVM)
        {
            if (!ModelState.IsValid)
                return View(confirmationVM);
            var user = await _userManager.FindByEmailAsync(confirmationVM.EmailOrUserName)
                                  ?? await _userManager.FindByNameAsync(confirmationVM.EmailOrUserName);

            if (user is not null)
            {
                await SendConfirmationEmail(user);
            }
            TempData["success"] = "Confirmation Email Sent , check your Mail";
            return RedirectToAction(nameof(Login));

        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM passwordVM)
        {
            if (!ModelState.IsValid)
                return View(passwordVM);
            var user = await _userManager.FindByEmailAsync(passwordVM.EmailOrUserName) ??
                await _userManager.FindByNameAsync(passwordVM.EmailOrUserName);
            if (user is not null)
            {
                await SendOTPToMail(user);
            }
            TempData["success"] = "Confirmation Email Sent , check your Mail";
            TempData["UserId"] = user.Id;
            return RedirectToAction(nameof(ValidateOTP));

        }

        [HttpGet]
        public IActionResult ValidateOTP()
        {
            if (TempData.Peek("UserId") is null)
                return NotFound();

            return View();
        }

     
    

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM oTPVM)
        {
            if (!ModelState.IsValid)
            {
                return View(oTPVM);
            }

            var userId = TempData["UserId"];

            var otpInDb = (await _userOTP.GetAllAsync(u => u.ApplicationUserId == userId.ToString() && !u.IsUsed))
                .OrderByDescending(u => u.CreatedAt)
                .FirstOrDefault();

            if (otpInDb is null)
            {
                TempData["error"] = "OTP is not Found";
                return View(oTPVM);
            }
            if (otpInDb.OTP != oTPVM.OTP)
            {
                TempData["error"] = "OTP invalid or expired";
                return View();
            }

            return RedirectToAction("ChangePassword");

        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (TempData.Peek("UserId") is null)
                return NotFound();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordVM);
            }
            var userId = TempData["UserId"];

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return NotFound();

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, changePasswordVM.Password);
            if (!result.Succeeded)
            {

                TempData["error"] = string.Join(",", result.Errors.Select(a => a.Description));
                return View(changePasswordVM);
            }
            TempData["success"] = "Change Password Succesfuly";
            return RedirectToAction(nameof(Login));
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Clear();
            return RedirectToAction(nameof(Index), "Home", new { area = SD.Admin });
        }

        private async Task<bool> SendConfirmationEmail(ApplicationUser user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action("Confirm", "Account", new { area = SD.Identity, Token = token, UserId = user.Id }, Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, $"Hi {user.UserName} confirm Registeration",
                     $"<h1>Confirm Email by Clicking <a href='{link}' > Here </a></h1>");
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<bool> SendOTPToMail(ApplicationUser user)
        {
            try
            {
                var otp = new Random().Next(100000, 999999);
                await _emailSender.SendEmailAsync(user.Email, $"Forget Password OTP {user.FirstName}",
                    $"<h1>your otp is <b>{otp}</b> Don't Share it</h1>");

                await _userOTP.CreateAsync(new()
                {
                    OTP = otp.ToString(),
                    ApplicationUserId = user.Id,

                });

                await _userOTP.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)

            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
