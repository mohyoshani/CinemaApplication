
namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _repositoryCategory;
        public CategoryController(IRepository<Category> repositoryCategory)
        {
            _repositoryCategory = repositoryCategory;
        }
        public async Task<IActionResult> Index(int page = 1, string? query = null , CancellationToken cancellationToken = default)
        {

            var categories = await _repositoryCategory.GetAllAsync(cancellationToken:cancellationToken , tracked: false);

            if (query is not null)
            {
                var lowerquery = query.ToLower().Trim();
                categories = categories.Where(c => c.Name.Contains(lowerquery));
            }

            int CategoryCount = categories.Count();
            categories = categories.Skip((page - 1) * 5).Take(5);
            double totalPages = Math.Ceiling(CategoryCount / 5.0);

            if (!categories.Any())
                return NotFound();

            return View(new CategoryVM()
            {
                TotalPages = totalPages,
                Query = query,
                Categories = categories.AsEnumerable(),
                CurrentPage = page
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category , CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                await _repositoryCategory.CreateAsync(category, cancellationToken);
                await _repositoryCategory.SaveChangesAsync(cancellationToken);
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            var category = await _repositoryCategory.GetOneAsync(c => c.Id == id, cancellationToken);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category, CancellationToken cancellationToken = default)
        {
            var categoryInDb = await _repositoryCategory.GetOneAsync(c => c.Id == category.Id, cancellationToken);
            if (ModelState.IsValid)
            {
               _repositoryCategory.Update(category);
                await _repositoryCategory.SaveChangesAsync(cancellationToken);
                TempData["info"] = "Category Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id , CancellationToken cancellationToken = default)
        {
            var category = await _repositoryCategory.GetOneAsync(c => c.Id == id, cancellationToken);
           
            if (category == null)
            {
                return NotFound();
            }
            _repositoryCategory.Delete(category);
            await _repositoryCategory.SaveChangesAsync(cancellationToken);
            TempData["error"] = "Category Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
