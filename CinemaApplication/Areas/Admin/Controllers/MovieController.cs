using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;

namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class MovieController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<MovieImage> _movieImageRepository;
        public MovieController(IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<MovieImage> movieImageRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _movieImageRepository = movieImageRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null , CancellationToken cancellationToken = default)
        {
            var movie = await _movieRepository.GetAllAsync(cancellationToken: cancellationToken,
                tracked: false, 
                includes: c => c.Include(m => m.Category));
            if (query is not null)
            {
                
                movie = movie.Where(m => m.Title.ToLower().Trim().Contains(query));
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
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            var Categories = await _categoryRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);
            return View(new CreateMovieVM
            {
                Categories = Categories.AsEnumerable()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieVM vm, IFormFile MainImage, List<IFormFile> SubImages, CancellationToken cancellationToken = default)
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

                await _movieRepository.CreateAsync(vm.Movie, cancellationToken);
                await _movieRepository.SaveChangesAsync(cancellationToken);
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

                            await _movieImageRepository.CreateAsync(new MovieImage
                            {
                                MovieId = vm.Movie.Id,
                                ImageUrl = fileName
                            });
                        }
                    }
                    await _movieImageRepository.SaveChangesAsync(cancellationToken);
                }
                TempData["success"] = "Movie Created Successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Failed to Create Movie. Please check the input data.";
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            var movie = await _movieRepository.GetOneAsync(expression: m => m.Id == id, cancellationToken: cancellationToken, tracked: false);
            if (movie == null)
                return NotFound();

            return View(new CreateMovieVM()
            {
                Movie = movie,
                Categories = await _categoryRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken),
                
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(int Id , CreateMovieVM vm, IFormFile? MainImage, List<IFormFile> SubImages, CancellationToken cancellationToken = default)
        {
            vm.Movie.Id = Id;
            if (!ModelState.IsValid)
            {
                vm.Categories = await _categoryRepository.GetAllAsync(tracked: false, cancellationToken: cancellationToken);
                TempData["error"] = "Failed to Update Movie. Please check the input data.";
                return View(vm);
            }

            var movieInDb = await _movieRepository.GetOneAsync(expression: m => m.Id == vm.Movie.Id, cancellationToken: cancellationToken, tracked: false);
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

                 _movieRepository.Update(vm.Movie);
                await _movieRepository.SaveChangesAsync(cancellationToken);

                if (SubImages != null && SubImages.Count > 0)
                {
                    var oldImages = _movieImageRepository.GetAllAsync(m => m.MovieId == vm.Movie.Id, tracked: false, cancellationToken: cancellationToken).Result.ToList();

                   
                    foreach (var oldImg in oldImages)
                    {
                        var oldImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Admin", "Movie", "Subimages", oldImg.ImageUrl);
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    if (oldImages.Any())
                    {
                        await _movieImageRepository.DeleteRangeAsync(oldImages);
                    }

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

                            _movieImageRepository.CreateAsync(new MovieImage
                            {
                                MovieId = vm.Movie.Id,
                                ImageUrl = fileName
                            } ,cancellationToken :cancellationToken);
                        }
                    }
                    await _movieImageRepository.SaveChangesAsync(cancellationToken);
                }
                TempData["info"] = "Movie Updated Successfully";

                return RedirectToAction(nameof(Index));
            
           
        }

        [HttpPost]

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(m => m.Id == id, cancellationToken: cancellationToken);
            if (movie == null)
            {
                return NotFound();
            }
            _movieRepository.Delete(movie);
            await _movieRepository.SaveChangesAsync(cancellationToken);
            TempData["error"] = "Movie Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
          
            var movie = await _movieRepository.GetOneAsync( m => m.Id == id,
                cancellationToken: cancellationToken,
                tracked: false,
                includes: c =>
                c.Include(m => m.Category)
                 .Include(m => m.MovieImages));



            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

    }
}


