using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(int page = 1, string? query = null)
        {
            var movie = _context.Movies.Include(m => m.Category).AsNoTracking().AsQueryable();

            if (query is not null)
            {
                var lowerQuery = query.ToLower().Trim();
                movie = movie.Where(m => m.Title.Contains(query));
            }

            int moviesCount = movie.Count();
            movie = movie.Skip((page - 1) * 5).Take(5);
            double totalPages = Math.Ceiling(moviesCount / 5.0);
            return View(new MovieVM()
            {
                CurrentPage = page,
                Movies = movie.AsEnumerable(),
                Query = query,
                totalPages = totalPages
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var Categories = _context.Categories.AsNoTracking().AsQueryable();
            return View(new CreateMovieVM
            {
                Categories = Categories.AsEnumerable()
            });
        }

        [HttpPost]
        public IActionResult Create(CreateMovieVM vm, IFormFile MainImage, List<IFormFile> SubImages)
        {
            if (ModelState.IsValid)
            {
                if (vm.Movie == null) vm.Movie = new Movie();

                if (MainImage is not null && MainImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Admin", "Movie", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        MainImage.CopyTo(stream);
                    }
                    vm.Movie.Mainimage = fileName;
                }

                _context.Movies.Add(vm.Movie);
                _context.SaveChanges();
                TempData["success"] = "Movie Created Successfully";

                if (SubImages != null && SubImages.Count > 0)
                {
                    foreach (var subImage in SubImages)
                    {
                        if (subImage.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(subImage.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Admin", "Movie", "Subimages", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                subImage.CopyTo(stream);
                            }

                            _context.MovieImages.Add(new MovieImage
                            {
                                MovieId = vm.Movie.Id,
                                ImageUrl = fileName
                            });
                        }
                    }
                    _context.SaveChanges();
                }
                TempData["success"] = "Movie Created Successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Failed to Create Movie. Please check the input data.";
            return View(vm);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
                return NotFound();


            return View(new CreateMovieVM()
            {
                Movie = movie,
                Categories = _context.Categories.AsNoTracking().AsEnumerable()
            });
        }
        [HttpPost]
        public IActionResult Update(CreateMovieVM vm, IFormFile MainImage, List<IFormFile> SubImages)
        {
            if (ModelState.IsValid)
            {

                var movieInDb = _context.Movies.AsNoTracking().SingleOrDefault(m => m.Id == vm.Movie.Id);
                if (movieInDb == null)
                    return NotFound();

                if (MainImage is not null && MainImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Admin", "Movie", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        MainImage.CopyTo(stream);
                    }
                    vm.Movie.Mainimage = fileName;
                }
                else
                {
                    vm.Movie.Mainimage = movieInDb.Mainimage;
                }

                _context.Movies.Update(vm.Movie);
                _context.SaveChanges();


                if (SubImages != null && SubImages.Count > 0)
                {
                    foreach (var subImage in SubImages)
                    {
                        if (subImage.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(subImage.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Admin", "Movie", "Subimages", fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                subImage.CopyTo(stream);
                            }

                            _context.MovieImages.Add(new MovieImage
                            {
                                MovieId = vm.Movie.Id,
                                ImageUrl = fileName
                            });
                        }
                    }
                    _context.SaveChanges();
                }
                TempData["info"] = "Movie Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Failed to Update Movie. Please check the input data.";
            return View(vm);
        }

        [HttpPost]

        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            TempData["error"] = "Movie Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var movie = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.MovieImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

    }
}


