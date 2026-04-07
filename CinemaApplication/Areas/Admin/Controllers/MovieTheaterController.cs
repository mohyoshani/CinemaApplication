using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class MovieTheaterController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(int page = 1, string? query = null)
        {
            var movie = _context.Movies.Include(m => m.Category).AsNoTracking().AsQueryable();
            if (query is not null)
            {
                var lowerQuery = query.ToLower().Trim();
                movie = movie.Where(m => m.Title.Contains(query) || m.Category.Name.Contains(query));
            }
            int moviesCount = movie.Count();
            movie = movie.Skip((page - 1) * 5).Take(5);
            double totalPages = Math.Ceiling(moviesCount / 5.0);
            var vm = new MovieShowtimesVM
            {
                Query = query,
                movies = movie.AsEnumerable(),
                CurrentPage = page,
                TotalPages = totalPages
            };
            return View(vm);
        }
        [HttpGet]
        public IActionResult Create(int movieid)
        {
            var cinemaHalls = _context.CinemaHalls.AsNoTracking().ToList();
            return View(new AssignTheaterVM
            {
                MovieId = movieid,
                CinemaHalls = cinemaHalls,
                Showtime = DateTime.Now
            });
        }

        [HttpPost]
        public IActionResult Create(AssignTheaterVM vm)
        {

            if (vm.SelectedCinemaHallIds != null && vm.SelectedCinemaHallIds.Any())
            {
                foreach (var hallId in vm.SelectedCinemaHallIds)
                {
                    var movieTheater = new MovieTheater
                    {
                        MovieId = vm.MovieId,
                        CinemaHallId = hallId,
                        Showtime = vm.Showtime
                    };

                    _context.MovieTheaters.Add(movieTheater);
                }
                
                _context.SaveChanges();
                TempData["success"] = "Theater Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            vm.CinemaHalls = _context.CinemaHalls.ToList();
            return View(vm);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var selectedCinemaHallIds = _context.MovieTheaters
                .Where(mt => mt.MovieId == id)
                .Select(mt => mt.CinemaHallId)
                .ToList();

            var cinemaHalls = _context.CinemaHalls.AsNoTracking().ToList();

            var vm = new AssignTheaterVM()
            {
                MovieId = id,
                CinemaHalls = cinemaHalls,
                SelectedCinemaHallIds = selectedCinemaHallIds,
                Showtime = DateTime.Now
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(int id, AssignTheaterVM vm)
        {
            vm.MovieId = id;

            var movieTheaters = _context.MovieTheaters.Where(mt => mt.MovieId == vm.MovieId);
            _context.MovieTheaters.RemoveRange(movieTheaters);
            if (vm.SelectedCinemaHallIds != null && vm.SelectedCinemaHallIds.Any())
            {
                foreach (var hallId in vm.SelectedCinemaHallIds)
                {
                    _context.MovieTheaters.Add(new MovieTheater
                    {
                        MovieId = vm.MovieId,
                        CinemaHallId = hallId,
                        Showtime = vm.Showtime

                    });
                }
            }
            
            _context.SaveChanges();
            TempData["info"] = "Theater Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var movieWithTheaters = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.MovieTheaters)
                .ThenInclude(mt => mt.CinemaHall)
                .FirstOrDefault(m => m.Id == id);

            if (movieWithTheaters == null)
            {
                return NotFound();
            }

            return View(movieWithTheaters);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {

            var movieTheaters = _context.MovieTheaters.Where(mt => mt.MovieId == id).ToList();

            if (movieTheaters.Any())
            {
                _context.MovieTheaters.RemoveRange(movieTheaters);
                _context.SaveChanges();
            }
            TempData["error"] = "Theater Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}

