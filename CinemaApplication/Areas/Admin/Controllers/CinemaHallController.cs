using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class CinemaHallController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(int page = 1, string? query = null)
        {
            var cinemaHall = _context.CinemaHalls.AsQueryable();

            //Search
            if (query is not null)
            {
                var lowerQuery = query.ToLower().Trim();
                cinemaHall = cinemaHall.Where(c => c.Name.Contains(query));
            }

            int totalCinemaHalls = cinemaHall.Count();
            cinemaHall = cinemaHall.Skip((page - 1) * 5).Take(5);
            double totalpages = Math.Ceiling(totalCinemaHalls / 5.0);

            return View(new CinemaHallsVM()
            {
                TotalPages = totalpages,
                Query = query,
                CinemaHalls = cinemaHall.AsEnumerable(),
                CurrentPage = page
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CinemaHall());
        }

        [HttpPost]
        public IActionResult Create(CinemaHall cinemaHall, IFormFile Image)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please correct the errors in the form.";
                return View(cinemaHall);
            }

            if (Image is not null && Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Admin", "CinemaHalls", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                cinemaHall.Image = fileName;
            }

            _context.CinemaHalls.Add(cinemaHall);
            _context.SaveChanges();
            TempData["success"] = "Hall Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
           
            var cinemaHall = _context.CinemaHalls.Find(id);
            if (cinemaHall is null)
                return NotFound();
            return View(cinemaHall);
        }

        [HttpPost]
        public IActionResult Update(CinemaHall cinemaHall, IFormFile Image)
        {
            if (ModelState.IsValid) { 
            var cinemaHallInDb = _context.CinemaHalls.AsNoTracking().SingleOrDefault(c => c.Id == cinemaHall.Id);
            if (cinemaHallInDb == null)
            {
                return NotFound();
            }
            if (Image is not null && Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Admin", "CinemaHalls", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                cinemaHall.Image = fileName;
            }
            else
            {
                cinemaHall.Image = cinemaHallInDb.Image;
            }

            _context.CinemaHalls.Update(cinemaHall);
            _context.SaveChanges();
            TempData["info"] = "Hall Updated Successfully";
            return RedirectToAction(nameof(Index));
            }
            return View(cinemaHall);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var cinemaHall = _context.CinemaHalls.Find(id);
            if (cinemaHall is null)
                return NotFound();
            _context.CinemaHalls.Remove(cinemaHall);
            _context.SaveChanges();
            TempData["error"] = "Hall Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}