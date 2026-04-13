using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class MovieCastController : Controller
    {
        private readonly IRepository<Movie> _repositoryMovie;
        private readonly IRepository<Actor> _repositoryActor;
        private readonly IRepository<MovieActor> _repositoryMovieActor;

        public MovieCastController(IRepository<Movie> repositoryMovie, IRepository<Actor> repositoryActor, IRepository<MovieActor> repositoryMovieActor)
        {
            _repositoryMovie = repositoryMovie;
            _repositoryActor = repositoryActor;
            _repositoryMovieActor = repositoryMovieActor;
        }
        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            //var movie = _context.Movies.Include(m => m.Category).AsNoTracking().AsQueryable();
            var movie = await _repositoryMovie.GetAllAsync(cancellationToken: cancellationToken, tracked: false, includes: c => c.Include(m => m.Category));
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
        public async Task<IActionResult> Create(int movieid, CancellationToken cancellationToken = default)
        {
            //var actors = _context.Actors.AsNoTracking().AsQueryable();
            var actors = await _repositoryActor.GetAllAsync(cancellationToken: cancellationToken, tracked: false);

            return View(new AssignCastVM()
            {
                MovieId = movieid,
                Actors = actors.AsEnumerable(),

            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(AssignCastVM vm, CancellationToken cancellationToken = default)
        {

            if (vm.SelectedActorsIds != null && vm.SelectedActorsIds.Any())
            {
                var movieActorsList = new List<MovieActor>();

                foreach (var actorId in vm.SelectedActorsIds)
                {
                    movieActorsList.Add(new MovieActor
                    {
                        MovieId = vm.MovieId,
                        ActorId = actorId
                    });
                }
                if (ModelState.IsValid)
                {
                    await _repositoryMovieActor.AddRangeAsync(movieActorsList, cancellationToken);
                    await _repositoryMovieActor.SaveChangesAsync(cancellationToken);

                    TempData["success"] = "Cast Assigned Successfully";
                    return RedirectToAction(nameof(Index));
                }

            }

            vm.Actors = await _repositoryActor.GetAllAsync(cancellationToken: cancellationToken, tracked: false);
            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            //var selectedactorids = _context.movieactors
            //    .where(ma => ma.movieid == id)
            //    .select(ma => ma.actorid)
            //    .tolist();

            var selectedActorIds = (await _repositoryMovieActor.GetAllAsync(expression: ma => ma.MovieId == id,
                cancellationToken: cancellationToken, 
                tracked: false))
                .Select(ma => ma.ActorId)
                .ToList();

            var actors = await _repositoryActor.GetAllAsync(cancellationToken: cancellationToken, tracked: false);

            var vm = new AssignCastVM()
            {
                MovieId = id,
                Actors = actors,
                SelectedActorsIds = selectedActorIds
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, AssignCastVM model, CancellationToken cancellationToken = default)
        {
            model.MovieId = id;


            var Cast = await _repositoryMovieActor.GetAllAsync(expression: ma => ma.MovieId == model.MovieId, cancellationToken: cancellationToken, tracked: false);
            await _repositoryMovieActor.DeleteRangeAsync(Cast);

            if (model.SelectedActorsIds != null && model.SelectedActorsIds.Any())
            {
                foreach (var actorId in model.SelectedActorsIds)
                {
                    await _repositoryMovieActor.CreateAsync(new MovieActor
                    {
                        MovieId = model.MovieId,
                        ActorId = actorId
                    }, cancellationToken);
                }
            }
            if (ModelState.IsValid)
            {
                TempData["info"] = "Cast Updated Successfully";
                await _repositoryMovieActor.SaveChangesAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            model.Actors = await _repositoryActor.GetAllAsync(cancellationToken: cancellationToken, tracked: false);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        {
            //var movieWithCast = _context.Movies
            //    .Include(m => m.Category)
            //    .Include(m => m.MovieActors)
            //    .ThenInclude(ma => ma.Actor)
            //    .FirstOrDefault(m => m.Id == id);

            var movieWithCast = await _repositoryMovie.GetAllAsync(m => m.Id == id,
                cancellationToken: cancellationToken, tracked: false,
                includes: q => q.Include(m => m.Category)
                    .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor));

            if (movieWithCast == null)
            {
                return NotFound();
            }

            return View(movieWithCast);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

     
            var movieCast = await _repositoryMovieActor.GetAllAsync(expression: ma => ma.MovieId == id, tracked: false);

            if (movieCast.Any())
            {
                await _repositoryMovieActor.DeleteRangeAsync(movieCast);
                await _repositoryMovieActor.SaveChangesAsync();
            }
            TempData["error"] = "Cast Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

