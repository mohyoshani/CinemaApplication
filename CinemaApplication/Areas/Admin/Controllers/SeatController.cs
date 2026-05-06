namespace CinemaApplication.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SeatController : Controller
    {
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<CinemaHall> _hallRepository;

        public SeatController(IRepository<Seat> seatRepository, IRepository<CinemaHall> hallRepository)
        {
            _seatRepository = seatRepository;
            _hallRepository = hallRepository;
        }

        public async Task<IActionResult> Index(int hallId, CancellationToken cancellationToken)
        {
            var seats = await _seatRepository.GetAllAsync(s => s.CinemaHallId == hallId, cancellationToken);
            ViewBag.HallId = hallId;
            return View(seats);
        }

        [HttpGet]
        public IActionResult Create(int hallId)
        {
            return View(new Seat { CinemaHallId = hallId });
        }

        [HttpPost]
   
        public async Task<IActionResult> Create(Seat seat, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                await _seatRepository.CreateAsync(seat, cancellationToken);
                await _seatRepository.SaveChangesAsync(cancellationToken);
                return RedirectToAction(nameof(Index), new { hallId = seat.CinemaHallId });
            }
            return View(seat);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken)
        {
            var seat = await _seatRepository.GetOneAsync(s => s.Id == id, cancellationToken);
            if (seat == null) return NotFound();
            return View(seat);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Seat seat, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                _seatRepository.Update(seat);
                await _seatRepository.SaveChangesAsync(cancellationToken);
                return RedirectToAction(nameof(Index), new { hallId = seat.CinemaHallId });
            }
            return View(seat);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var seat = await _seatRepository.GetOneAsync(s => s.Id == id, cancellationToken);
            if (seat != null)
            {
                _seatRepository.Delete(seat);
                await _seatRepository.SaveChangesAsync(cancellationToken);
            }
            return RedirectToAction(nameof(Index), new { hallId = seat?.CinemaHallId });
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSeats(int hallId, int rowsCount, int seatsPerRow, CancellationToken cancellationToken)
        {
            for (int i = 1; i <= rowsCount; i++)
            {
                char rowLetter = (char)('A' + i - 1); 
                for (int j = 1; j <= seatsPerRow; j++)
                {
                    var seat = new Seat
                    {
                        CinemaHallId = hallId,
                        Row = rowLetter.ToString(),
                        Number = j
                    };
                    await _seatRepository.CreateAsync(seat, cancellationToken);
                }
            }
            await _seatRepository.SaveChangesAsync(cancellationToken);
            return RedirectToAction(nameof(Index), new { hallId = hallId });
        }
    }
}