using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class MovieCastController : Controller
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
            var vM = new MovieCastVM
            {
                Query = query,
                movies = movie.AsEnumerable(),
                CurrentPage = page,
                TotalPages = totalPages
            };
            return View(vM);
        }

        [HttpGet]
        public IActionResult Create(int movieid)
        {
            var actors = _context.Actors.AsNoTracking().AsQueryable();

            return View(new AssignCastVM()
            {
                MovieId = movieid,
                Actors = actors.AsEnumerable(),

            });
        }

        [HttpPost]
        public IActionResult Create(AssignCastVM vm)
        {
            if (vm.SelectedActorsIds != null && vm.SelectedActorsIds.Any())
            {
                foreach (var actorId in vm.SelectedActorsIds)
                {
                    var movieActor = new MovieActor
                    {
                        MovieId = vm.MovieId,
                        ActorId = actorId
                    };
                    _context.MovieActors.Add(movieActor);
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            vm.Actors = _context.Actors.ToList();
            return View(vm);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var selectedActorIds = _context.MovieActors
                .Where(ma => ma.MovieId == id)
                .Select(ma => ma.ActorId)
                .ToList();

            var actors = _context.Actors.AsNoTracking().ToList();

            var vm = new AssignCastVM()
            {
                MovieId = id,
                Actors = actors,
                SelectedActorsIds = selectedActorIds
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(int id, AssignCastVM model)
        {
            model.MovieId = id;
            var castInDb = _context.MovieActors.SingleOrDefault(ma => ma.MovieId == model.MovieId);
            if (castInDb == null)
            {
                return NotFound();
            }
            var Cast = _context.MovieActors.Where(ma => ma.MovieId == model.MovieId);
            _context.MovieActors.RemoveRange(Cast);

            if (model.SelectedActorsIds != null && model.SelectedActorsIds.Any())
            {
                foreach (var actorId in model.SelectedActorsIds)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        MovieId = model.MovieId,
                        ActorId = actorId
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var movieWithCast = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
                .FirstOrDefault(m => m.Id == id);

            if (movieWithCast == null)
            {
                return NotFound();
            }

            return View(movieWithCast);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {

            var movieCast = _context.MovieActors.Where(ma => ma.MovieId == id).ToList();

            if (movieCast.Any())
            {
                _context.MovieActors.RemoveRange(movieCast);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

