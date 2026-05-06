using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;

namespace CinemaApplication.Areas.Customer.Controllers
{
    [Area(SD.Customer)]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<OrderItemSeat> _orderItemSeat;
        private readonly IRepository<MovieTheater> _movieTheaterRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<CartItemSeat> _cartItemSeatRepository;

        public CartController(UserManager<ApplicationUser> userManager,
            IRepository<Cart> cartRepository, IRepository<OrderItemSeat> orderItemSeatRepository,
            IRepository<MovieTheater> movieTheaterRepository, IRepository<CartItem> cartItemRepository, IRepository<CartItemSeat> cartItemSeatRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _orderItemSeat = orderItemSeatRepository;
            _movieTheaterRepository = movieTheaterRepository;
            _cartItemRepository = cartItemRepository;
            _cartItemSeatRepository = cartItemSeatRepository;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _cartRepository.GetOneAsync(c => c.ApplicationUserId == user.Id, cancellationToken,
                includes: c => c.Include(c => c.CartItems).ThenInclude(i => i.MovieTheater).ThenInclude(mt => mt.Movie)
                                .Include(c => c.CartItems).ThenInclude(i => i.SelectedSeats).ThenInclude(s => s.Seat));

            return View(cart);
        }


        public async Task<IActionResult> AddtToCart(int id, List<int> seatIds, CancellationToken cancellationToken)
        {
            if (seatIds == null || !seatIds.Any())
            {
                ModelState.AddModelError("", "Select at least one seat.");
                return View();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c => c.ApplicationUserId == user.Id, cancellationToken)
                ?? new Cart { ApplicationUserId = user.Id };
            if (cart.Id == 0)
            {
                await _cartRepository.CreateAsync(cart, cancellationToken);
                await _cartRepository.SaveChangesAsync(cancellationToken);
            }

            var movieTheater = await _movieTheaterRepository.GetOneAsync(mt => mt.Id == id, cancellationToken, false,
                includes: mt => mt.Include(mt => mt.Movie));
            if (movieTheater is null) return NotFound();

            var reservedInOrders = await _orderItemSeat.GetAllAsync(
       ois => ois.OrderItem.MovieTheaterId == id && seatIds.Contains(ois.SeatId),
       cancellationToken);
            if (reservedInOrders.Any())
            {

                return BadRequest("Sorry, some seats are already reserved.");
            }

            var reservedInCarts = await _cartItemSeatRepository.GetAllAsync(
    cis => cis.CartItem.MovieTheaterId == id && seatIds.Contains(cis.SeatId),
    cancellationToken);

            if (reservedInCarts.Any())
            {
                return BadRequest("Seats reserved in another cart.");
            }

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                MovieTheaterId = id,
                PricePerSeat = movieTheater.Movie.Price,
                SeatsCount = seatIds.Count,
            };

            cartItem.SelectedSeats = seatIds.Select(seatId => new CartItemSeat
            {
                SeatId = seatId
            }).ToList();
            await _cartItemRepository.CreateAsync(cartItem, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> IncrementCount(int cartItemId, CancellationToken cancellationToken)
        {
            var cart = await _cartItemRepository.GetOneAsync(i => i.Id == cartItemId, cancellationToken);
            if (cart == null) return NotFound();

            if (cart.SeatsCount < 20) 
            {
                cart.SeatsCount++;
                await _cartItemRepository.SaveChangesAsync(cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecrementCount(int cartItemId, CancellationToken cancellationToken)
        {
            var item = await _cartItemRepository.GetOneAsync(i => i.Id == cartItemId, cancellationToken);
            if (item == null) return NotFound();

            if (item.SeatsCount > 1)
            {
                item.SeatsCount--;
                await _cartItemRepository.SaveChangesAsync(cancellationToken);
            }
            else
            {
               
                 _cartItemRepository.Delete(item);
                await _cartItemRepository.SaveChangesAsync(cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int cartItemId, CancellationToken cancellationToken)
        {
            var item = await _cartItemRepository.GetOneAsync(i => i.Id == cartItemId, cancellationToken);
            if (item != null)
            {
                _cartItemRepository.Delete(item);
                await _cartItemRepository.SaveChangesAsync(cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }


     
        public async Task<IActionResult> SelectSeat(int movieTheaterId, CancellationToken cancellationToken)
        {
            var theater = await _movieTheaterRepository.GetOneAsync(
                mt => mt.Id == movieTheaterId,
                cancellationToken,
                includes: mt => mt.Include(mt => mt.Movie)
                                  .Include(mt => mt.CinemaHall)
                                  .ThenInclude(ch => ch.Seats));

            if (theater == null) return NotFound();

         
            var reservedOrderSeats = await _orderItemSeat.GetAllAsync(
                ois => ois.OrderItem.MovieTheaterId == movieTheaterId, cancellationToken);

            var reservedInCarts = await _cartItemSeatRepository.GetAllAsync(
                cis => cis.CartItem.MovieTheaterId == movieTheaterId, cancellationToken);

           
            ViewBag.ReservedSeatIds = reservedOrderSeats.Select(s => s.SeatId)
                                        .Union(reservedInCarts.Select(c => c.SeatId))
                                        .ToList();

            return View(theater);
        }
    }
}
