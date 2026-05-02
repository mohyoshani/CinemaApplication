

using static System.Net.Mime.MediaTypeNames;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ActorController : Controller
    {
        //private readonly ApplicationDbContext _context = new();

        private readonly IRepository<Actor> _repositoryActor;
        public ActorController(IRepository<Actor> repositoryActor)
        {
            _repositoryActor = repositoryActor;
        }
        public async Task<IActionResult> Index(int page = 1, string? query = null , CancellationToken cancellationToken = default)
        {
            //Search

            
            var actors = await _repositoryActor.GetAllAsync(cancellationToken: cancellationToken , tracked: false);
            if (query is not null)
            {
                
                actors = actors.Where(a => a.Name.ToLower().Trim().Contains(query));
            }

            //Pagination

            var totalActors = actors.Count();
            actors = actors.Skip((page - 1) * 5).Take(5);
            double totalpages = Math.Ceiling(totalActors / 5.0);


         

            return View(new ActorsVM()
            {
                TotalPages = totalpages,
                Query = query,
                Actors = actors.AsEnumerable(),
                CurrentPage = page
            });

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile Image , CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                if (Image is not null && Image.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Admin", "Actor", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                    actor.Image = fileName;
                }
                await _repositoryActor.CreateAsync(actor, cancellationToken: cancellationToken);
                await _repositoryActor.SaveChangesAsync(cancellationToken: cancellationToken);
                TempData["success"] = "Actor Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id , CancellationToken cancellationToken = default)
        {
            var actor = await _repositoryActor.GetOneAsync(a => a.Id == id, cancellationToken: cancellationToken);

            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }


        [HttpPost]
        public async Task<IActionResult> Update(Actor actor, IFormFile Image , CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                var actorInDb = await _repositoryActor.GetOneAsync(a => a.Id == actor.Id, cancellationToken: cancellationToken);
                if (actorInDb == null)
                {
                    return NotFound();
                }

                if (Image is not null && Image.Length > 0)
                {
                   
                    if (actorInDb.Image is not null)
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Admin", "Actor", actorInDb.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Admin", "Actor", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }
                    actor.Image = fileName;
                }
                else
                {
                    actor.Image = actorInDb.Image;
                }

                _repositoryActor.Update(actor);
                await _repositoryActor.SaveChangesAsync(cancellationToken);
                TempData["info"] = "Actor Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var actor = await _repositoryActor.GetOneAsync(a => a.Id == id , cancellationToken: cancellationToken);
            if (actor == null)
            {
                return NotFound();
            }
            //_context.Actors.Remove(actor);
            //_context.SaveChanges();
            _repositoryActor.Delete(actor);
            await _repositoryActor.SaveChangesAsync(cancellationToken: cancellationToken);
            TempData["error"] = "Actor Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
