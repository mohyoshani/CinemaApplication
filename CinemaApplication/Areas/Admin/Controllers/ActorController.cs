

using static System.Net.Mime.MediaTypeNames;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(int page = 1, string? query = null)
        {
            //Search

            var actors = _context.Actors.AsNoTracking().AsQueryable();
            if (query is not null)
            {
                var lowerQuery = query.ToLower().Trim();
                actors = actors.Where(a => a.Name.Contains(lowerQuery));
            }

            //Pagination

            var totalActors = actors.Count();
            actors = actors.AsNoTracking().Skip((page - 1) * 5).Take(5);
            double totalpages = Math.Ceiling(totalActors / 5.0);


            if (!actors.Any())
                return NotFound();


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
        public IActionResult Create(Actor actor, IFormFile Image)
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
                _context.Actors.Add(actor);
                _context.SaveChanges();
                TempData["success"] = "Actor Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var actor = _context.Actors.Find(id);

            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }
        [HttpPost]

        public IActionResult Update(Actor actor, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                var actorInDb = _context.Actors.AsNoTracking().SingleOrDefault(a => a.Id == actor.Id);
                if (actorInDb == null)
                {
                    return NotFound();
                }

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
                else
                {
                    actor.Image = actorInDb.Image;
                }
                _context.Actors.Update(actor);
                _context.SaveChanges();
                TempData["info"] = "Actor Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor == null)
            {
                return NotFound();
            }
            _context.Actors.Remove(actor);
            _context.SaveChanges();
            TempData["error"] = "Actor Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
