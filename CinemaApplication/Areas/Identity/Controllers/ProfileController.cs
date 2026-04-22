using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace CinemaApplication.Areas.Identity.Controllers
{
    [Area(SD.Identity)]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Update()
        {
         
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

      
            var result = user.Adapt<ApplicationUserVM>();

           
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ApplicationUserVM applicationUserVM)
        {
            if (!ModelState.IsValid)
                return View(applicationUserVM);

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            user.FirstName = applicationUserVM.FirstName;
            user.LastName = applicationUserVM.LastName;
            user.Address = applicationUserVM.Address;
            user.PhoneNumber = applicationUserVM.PhoneNumber;

       
            if (user.Email != applicationUserVM.Email)
            {
                user.Email = applicationUserVM.Email;
                user.UserName = applicationUserVM.Email;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(applicationUserVM);
            }

            if (applicationUserVM.CurrentPassword is not null && applicationUserVM.NewPassword is not null)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user,
                    applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(applicationUserVM);
                }
            }

        
            await _signInManager.RefreshSignInAsync(user);

            TempData["success"] = "Profile and Password Updated Successfully";
            return RedirectToAction(nameof(Update));
        }
    }
}
