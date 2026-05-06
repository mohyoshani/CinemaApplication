using Microsoft.AspNetCore.Identity;

namespace CinemaApplication.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderItemSeat> _orderItemSeatRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<MovieTheater> _movieTheaterRepository;

        public OrderController(
            UserManager<ApplicationUser> userManager,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderItemSeat> orderItemSeatRepository,
            IRepository<Cart> cartRepository ,
            IRepository<MovieTheater> movieTheaterRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderItemSeatRepository = orderItemSeatRepository;
            _movieTheaterRepository = movieTheaterRepository;
            _cartRepository = cartRepository;
        }

      
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _orderRepository.GetAllAsync(
                o => o.ApplicationUserId == user!.Id,
                cancellationToken);

            return View(orders.OrderByDescending(o => o.OrderDate));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int movieTheaterId, List<int> seatIds, CancellationToken cancellationToken)
        {
         
            if (seatIds == null || !seatIds.Any())
            {
                TempData["Error"] = "Please select at least one seat.";
                return RedirectToAction("SelectSeat", new { movieTheaterId });
            }

            var user = await _userManager.GetUserAsync(User);

           
            var alreadyBooked = await _orderItemSeatRepository.GetAllAsync(
                s => seatIds.Contains(s.SeatId) && s.OrderItem.MovieTheaterId == movieTheaterId,
                cancellationToken);

            if (alreadyBooked.Any())
            {
                TempData["Error"] = "Sorry, some of the selected seats were just booked by someone else.";
                return RedirectToAction("SelectSeat", new { movieTheaterId });
            }

            
            var order = new Order
            {
                ApplicationUserId = user!.Id,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Confirmed 
            };

            await _orderRepository.CreateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            
            var theater = await _movieTheaterRepository.GetOneAsync(mt => mt.Id == movieTheaterId, cancellationToken);

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                MovieTheaterId = movieTheaterId,
                SeatsCount = seatIds.Count,
                PricePerSeat = theater.Movie.Price, 
                TotalPrice = theater.Movie.Price * seatIds.Count
            };

            await _orderItemRepository.CreateAsync(orderItem, cancellationToken);
            await _orderItemRepository.SaveChangesAsync(cancellationToken);

            
            foreach (var seatId in seatIds)
            {
                await _orderItemSeatRepository.CreateAsync(new OrderItemSeat
                {
                    OrderItemId = orderItem.Id,
                    SeatId = seatId
                }, cancellationToken);
            }

            await _orderItemSeatRepository.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "Booking completed successfully!";
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }


        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOneAsync(o => o.Id == id, cancellationToken,
                includes: o => o.Include(o => o.OrderItems!)
                                .ThenInclude(oi => oi.MovieTheater!)
                                .ThenInclude(mt => mt.Movie)
                                .Include(o => o.OrderItems!)
                                .ThenInclude(oi => oi.BookedSeats!)
                                .ThenInclude(os => os.Seat));

            if (order == null) return NotFound();
            return View(order);
        }
    }
}