using CinemaApplication.Utility;
using CinemaApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CinemaApplication.Areas.Customer.Controllers
{
    [Area(nameof(SD.Customer))]
    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Actor> _actorRepository;
        public HomeController(IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<Actor> actorRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _actorRepository = actorRepository;
        }
        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {

            var movie = await _movieRepository.GetAllAsync(
                includes: m=>m.Include(m=>m.Category)
                .Include(m=>m.MovieActors)
                .ThenInclude(n=>n.Actor)
                , cancellationToken: default);

            if (query is not null)
            {
            
           movie = movie.Where(m => m.Title.ToLower().Trim().Contains(query) 
            || m.Category.Name.ToLower().Trim().Contains(query) 
            || m.MovieActors.Any(ma => ma.Actor.Name.ToLower().Trim().Contains(query)));
            }

            int movieCount = movie.Count();
            double totalPages = Math.Ceiling(movieCount / 4.0);
            var movies = movie.Skip((page - 1) * 4).Take(4);
            var categories = await _categoryRepository.GetAllAsync(cancellationToken: cancellationToken);
            return View(new HomeMoviesVM()
            {
                Movies = movies,
                Categories = categories,
                CurrentPage = page,
                TotalPages = totalPages,
                Query = query
            });
        }

      

    }
}
