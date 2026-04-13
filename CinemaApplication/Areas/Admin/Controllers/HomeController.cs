using CinemaApplication.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _repositoryMovie;
        private readonly IRepository<Actor> _repositoryActor;
        private readonly IRepository<CinemaHall> _repositoryCinemaHall;
        private readonly IRepository<Category> _repositoryCategory;
        public HomeController(IRepository<Movie> repositoryMovie, IRepository<Actor> repositoryActor, IRepository<CinemaHall> repositoryCinemaHall, IRepository<Category> repositoryCategory)
        {
            _repositoryMovie = repositoryMovie;
            _repositoryActor = repositoryActor;
            _repositoryCinemaHall = repositoryCinemaHall;
            _repositoryCategory = repositoryCategory;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {

            var vm = new HomeCountersVM
            {

                MoviesCount = await _repositoryMovie.CountAsync(cancellationToken: cancellationToken),
                ActorsCount = await _repositoryActor.CountAsync(cancellationToken: cancellationToken),
                CinemaHallsCount = await _repositoryCinemaHall.CountAsync(cancellationToken: cancellationToken),
                CategoriesCount = await _repositoryCategory.CountAsync(cancellationToken: cancellationToken)
            };

            return View(vm);
        }
    }
}
