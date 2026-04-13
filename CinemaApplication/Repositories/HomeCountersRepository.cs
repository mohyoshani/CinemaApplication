using System.Linq.Expressions;

namespace CinemaApplication.Repositories
{
    public class HomeCountersRepository : IHomeCountersRepository
    {
        protected readonly ApplicationDbContext _context;

        public HomeCountersRepository(ApplicationDbContext context)
        {

            _context = context;
        }

        public Task<int> GetCountAsync<T>(CancellationToken cancellationToken = default) where T : class
        {
            return _context.Set<T>().CountAsync(cancellationToken);
        }
    }
}
