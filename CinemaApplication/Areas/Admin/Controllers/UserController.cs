using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(SD.Admin)]
    //[Authorize(Roles = $"{SD.Admin_Role} , {SD.SuperAdmin_Role}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null ,CancellationToken cancellationToken = default)
        {
            var users = _userManager.Users.AsQueryable();
            var upperQuery = query?.ToUpper().Trim();
            if (query is not null)
            {
                users = users.Where(u => u.NormalizedUserName!.Contains(upperQuery!));
            }

            //pagination 

            var totalUsers = users.Count();
            var usersList = users.Skip((page - 1) * 5).Take(5).ToList();
            var totalPages = Math.Ceiling(totalUsers / 5.0);
            var currentPage = page;

            var UserRoles = new Dictionary<ApplicationUser, string>();
            foreach (var item in usersList) 
            {
                UserRoles.Add(item, (await _userManager.GetRolesAsync(item)).FirstOrDefault()!);
            }
            //var viewModel = users.Adapt<ApplicationUserFilterVM>();
            var viewModel = new ApplicationUserFilterVM()
            {
                UserRoles = UserRoles.ToDictionary(),
                query = query,
                totalPages = totalPages,
                currentPage = currentPage
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRole(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();
            //if (await _userManager.IsInRoleAsync(user, SD.SuperAdmin_Role)) return NotFound();
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;
            return View(new UserWithRoleVM()
            {
               
                ApplicationUser = user,
                RoleName = userRole,
                IdentityRoles = _roleManager.Roles.AsEnumerable()

            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(UserWithRoleVM userRolesVM)
        {
            

            var user = await _userManager.FindByIdAsync(userRolesVM.Id);
            if (user is null) return NotFound();

            var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (currentRole != null)
            {
                await _userManager.RemoveFromRoleAsync(user, currentRole);
            }

            await _userManager.AddToRoleAsync(user, userRolesVM.RoleName);

            TempData["success"] = "User role updated successfully";
            return RedirectToAction(nameof(Index));
        }


   
       public async Task<IActionResult> LockUnlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            user.LockoutEnabled = !user.LockoutEnabled;
            if(user.LockoutEnabled)
            {
                user.LockoutEnd = user.LockoutEnd = DateTime.Now.AddDays(14); ;
            }
            else
            {
                user.LockoutEnd = null;
            }
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
