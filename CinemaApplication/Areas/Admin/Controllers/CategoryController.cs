namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area(nameof(SD.Admin))]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(int page = 1, string? query = null)
        {

            var categories = _context.Categories.AsNoTracking().AsQueryable();

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
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Update(Category category)
        {
            var categoryInDb = _context.Categories.AsNoTracking().SingleOrDefault(c => c.Id == category.Id);
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["info"] = "Category Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["error"] = "Category Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
