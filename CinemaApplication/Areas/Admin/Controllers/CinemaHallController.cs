using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class CinemaHallController : Controller
    {
        private readonly IRepository<CinemaHall> _repositoryCinemaHall;
        public CinemaHallController(IRepository<CinemaHall> repositoryCinemaHall)
        {
            _repositoryCinemaHall = repositoryCinemaHall;
        }
        public async Task<IActionResult> Index(int page = 1, string? query = null , CancellationToken cancellationToken = default)
        {
            var cinemaHall = await _repositoryCinemaHall.GetAllAsync(cancellationToken:cancellationToken , tracked: false);

            //Search
            if (query is not null)
            {
                
                cinemaHall = cinemaHall.Where(c => c.Name.ToLower().Trim().Contains(query));
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
        public async Task<IActionResult> Create(CinemaHall cinemaHall, IFormFile Image , CancellationToken cancellationToken = default)
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

            await _repositoryCinemaHall.CreateAsync(cinemaHall, cancellationToken);
            await _repositoryCinemaHall.SaveChangesAsync(cancellationToken);
            
            TempData["success"] = "Hall Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
           
            var cinemaHall = await _repositoryCinemaHall.GetOneAsync(c => c.Id == id, cancellationToken);
            if (cinemaHall is null)
                return NotFound();
            return View(cinemaHall);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CinemaHall cinemaHall, IFormFile Image, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid) 
            { 
            var cinemaHallInDb = await _repositoryCinemaHall.GetOneAsync(c => c.Id == cinemaHall.Id, cancellationToken);
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

            _repositoryCinemaHall.Update(cinemaHall);
            await _repositoryCinemaHall.SaveChangesAsync(cancellationToken);
            TempData["info"] = "Hall Updated Successfully";
            return RedirectToAction(nameof(Index));
            }
            return View(cinemaHall);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var cinemaHall = await _repositoryCinemaHall.GetOneAsync(ch => ch.Id == id, cancellationToken);
            if (cinemaHall is null)
                return NotFound();
            _repositoryCinemaHall.Delete(cinemaHall);
            await _repositoryCinemaHall.SaveChangesAsync(cancellationToken);
            TempData["error"] = "Hall Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}