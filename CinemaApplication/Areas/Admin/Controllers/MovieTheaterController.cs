

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class MovieTheaterController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<CinemaHall> _cinemaHallRepository;
        private readonly IRepository<MovieTheater> _movieTheaterRepository;

        public MovieTheaterController(IRepository<Movie> movieRepository, IRepository<CinemaHall> cinemaHallRepository, IRepository<MovieTheater> movieTheaterRepository)
        {
            _movieRepository = movieRepository;
            _cinemaHallRepository = cinemaHallRepository;
            _movieTheaterRepository = movieTheaterRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            var movie = await _movieRepository.GetAllAsync(cancellationToken: cancellationToken, tracked: false, includes: c => c.Include(m => m.Category));
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
        public async Task<IActionResult> Create(int movieid)
        {


            var cinemaHalls = await _cinemaHallRepository.GetAllAsync(tracked: false);
            return View(new AssignTheaterVM
            {
                MovieId = movieid,
                CinemaHalls = cinemaHalls,
                Showtime = DateTime.Now
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(AssignTheaterVM vm, CancellationToken cancellationToken = default)
        {

            if (vm.SelectedCinemaHallIds != null && vm.SelectedCinemaHallIds.Any())
            {

                var movieTheatersList = new List<MovieTheater>();

                foreach (var hallId in vm.SelectedCinemaHallIds)
                {
                    var movieTheater = new MovieTheater
                    {
                        MovieId = vm.MovieId,
                        CinemaHallId = hallId,
                        Showtime = vm.Showtime
                    };

                    movieTheatersList.Add(movieTheater);
                }
                await _movieTheaterRepository.AddRangeAsync(movieTheatersList, cancellationToken: cancellationToken);
                await _movieTheaterRepository.SaveChangesAsync();
            }
            if (ModelState.IsValid)
            {
                await _movieTheaterRepository.SaveChangesAsync(cancellationToken);
                TempData["success"] = "Theater Created Successfully";
                return RedirectToAction(nameof(Index));
            }

            vm.CinemaHalls = await _cinemaHallRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {

            var selectedCinemaHallIds = (await _movieTheaterRepository.GetAllAsync(expression: ma => ma.MovieId == id,
             cancellationToken: cancellationToken,
             tracked: false))
             .Select(ma => ma.CinemaHallId)
             .ToList();


            var cinemaHalls = await _cinemaHallRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);
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
        public async Task<IActionResult> Update(int id, AssignTheaterVM vm, CancellationToken cancellationToken = default)
        {
            vm.MovieId = id;

            var movieTheaters = await _movieTheaterRepository.GetAllAsync(
                expression: mt => mt.MovieId == vm.MovieId,
                cancellationToken: cancellationToken, 
                tracked: false);
            await _movieTheaterRepository.DeleteRangeAsync(movieTheaters);
            if (vm.SelectedCinemaHallIds != null && vm.SelectedCinemaHallIds.Any())
            {
                foreach (var hallId in vm.SelectedCinemaHallIds)
                {
                    await _movieTheaterRepository.CreateAsync(new MovieTheater
                    {
                        MovieId = vm.MovieId,
                        CinemaHallId = hallId,
                        Showtime = vm.Showtime

                    });
                }
            }
            if (ModelState.IsValid)
            {

                TempData["info"] = "Theater Updated Successfully";
                await _movieTheaterRepository.SaveChangesAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            vm.CinemaHalls = await _cinemaHallRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        {
          
            var movieWithTheaters = await _movieRepository.GetOneAsync(expression: m => m.Id == id,
                cancellationToken: cancellationToken,
                tracked: false,
                includes: c =>
                c.Include(m => m.Category)
                    .Include(m => m.MovieTheaters)
                    .ThenInclude(mt => mt.CinemaHall));

            if (movieWithTheaters == null)
            {
                return NotFound();
            }

            return View(movieWithTheaters);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {

            var movieTheaters = _movieTheaterRepository.GetAllAsync(expression: mt => mt.MovieId == id, cancellationToken: cancellationToken, tracked: false).Result;

            if (movieTheaters.Any())
            {
                await _movieTheaterRepository.DeleteRangeAsync(movieTheaters);
                await _movieTheaterRepository.SaveChangesAsync(cancellationToken);
            }
            TempData["error"] = "Theater Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}

