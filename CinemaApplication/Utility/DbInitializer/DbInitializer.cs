using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;

namespace CinemaApplication.Utility.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbInitializer> _logger;
        public DbInitializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<DbInitializer> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }
        public async Task Initialize()
        {
            try
            {

                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (_roleManager.Roles.IsNullOrEmpty())
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdmin_Role));
                    await _roleManager.CreateAsync(new IdentityRole(SD.Admin_Role));
                    await _roleManager.CreateAsync(new IdentityRole(SD.Employee_Role));
                    await _roleManager.CreateAsync(new IdentityRole(SD.Customer_Role));

                    await _userManager.CreateAsync(new()
                    {
                        Email = "SuperAdmin@mail.com",
                        EmailConfirmed = true,
                        FirstName = "Super",
                        LastName = "Admin",
                        UserName = "SuperAdmin"
                    }, password: "Admin@94");

                    var user = await _userManager.FindByEmailAsync("SuperAdmin@mail.com");
                    if (user is not null)
                    {
                        await _userManager.AddToRoleAsync(user, SD.SuperAdmin_Role);
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
            }
        }
    }
}
