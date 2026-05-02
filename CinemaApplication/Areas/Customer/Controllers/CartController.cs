using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApplication.Areas.Customer.Controllers
{
    [Area(SD.Customer)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Movie> _movieRepository;
        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Movie> movieRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _movieRepository = movieRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddtToCart(int id , int count)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user is null) 
            {
                return NotFound();
            }
            var movie = await _movieRepository.GetOneAsync(m=>m.Id == id);
            _cartRepository.CreateAsync(new Cart
            {
                ApplicationUserId = user.Id,
                movieId = id,
                Seats = count

            });
            return View();
        }

        public IActionResult IncrementCount() 
        {
            return View();
        }

        public IActionResult DecrementCount()
        {
            return View();
        }

        public IActionResult Delete() 
        { 
            return View();
        }
    }
}
