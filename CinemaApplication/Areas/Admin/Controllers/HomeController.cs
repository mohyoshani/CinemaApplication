using CinemaApplication.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
                var vm = new HomeCountersVM
                {
                    MoviesCount = _context.Movies.Count(),
                    ActorsCount = _context.Actors.Count(),
                    CinemaHallsCount = _context.CinemaHalls.Count(),
                    CategoriesCount = _context.Categories.Count()
                };
                
            return View(vm);
        }
    }
}
