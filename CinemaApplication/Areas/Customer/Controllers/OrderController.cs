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

        public OrderController(
            UserManager<ApplicationUser> userManager,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderItemSeat> orderItemSeatRepository,
            IRepository<Cart> cartRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderItemSeatRepository = orderItemSeatRepository;
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
        public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _cartRepository.GetOneAsync(c => c.ApplicationUserId == user!.Id, cancellationToken,
                includes: c => c.Include(c => c.CartItems).ThenInclude(i => i.SelectedSeats));

            if (cart == null || !cart.CartItems.Any()) return RedirectToAction("Index", "Cart");

           
            var order = new Order
            {
                ApplicationUserId = user.Id,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Confirmed 
            };

            await _orderRepository.CreateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

          
            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    MovieTheaterId = item.MovieTheaterId,
                    PricePerSeat = item.PricePerSeat,
                    SeatsCount = item.SeatsCount,
                    TotalPrice = item.PricePerSeat * item.SeatsCount
                };
                await _orderItemRepository.CreateAsync(orderItem, cancellationToken);
                await _orderItemRepository.SaveChangesAsync(cancellationToken);

             
                foreach (var seat in item.SelectedSeats)
                {
                    await _orderItemSeatRepository.CreateAsync(new OrderItemSeat
                    {
                        OrderItemId = orderItem.Id,
                        SeatId = seat.SeatId
                    }, cancellationToken);
                }
            }

         
            _cartRepository.Delete(cart);
            await _orderRepository.SaveChangesAsync(cancellationToken);

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