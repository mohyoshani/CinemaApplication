using CinemaApplication.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class HomeController : Controller
    {
  
        private readonly IHomeCountersRepository _homeCountersRepo;

        public HomeController(IHomeCountersRepository homeCountersRepo)
        {
            _homeCountersRepo = homeCountersRepo;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {

            var vm = new HomeCountersVM
            {

                MoviesCount = await _homeCountersRepo.GetCountAsync<Movie>(cancellationToken),
                ActorsCount = await _homeCountersRepo.GetCountAsync<Actor>(cancellationToken),
                CategoriesCount = await _homeCountersRepo.GetCountAsync<Category>(cancellationToken),
                CinemaHallsCount = await _homeCountersRepo.GetCountAsync<CinemaHall>(cancellationToken)
              
            };

            return View(vm);
        }
    }
}
