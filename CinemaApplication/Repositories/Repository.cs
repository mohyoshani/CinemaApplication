
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CinemaApplication.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }



        public async Task<IEnumerable<T>> GetAllAsync(
     Expression<Func<T, bool>>? expression = null, CancellationToken cancellationToken = default,
     Func<IQueryable<T>, IQueryable<T>>? include = null,
     bool tracked = true)
     
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
                query = query.AsNoTracking();


            if (include is not null)
            {
                query = include(query);
            }

            if (expression is not null)
                query = query.Where(expression);

            return await query.ToListAsync(cancellationToken);
        }


        public async Task<T?> GetOneAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default, bool tracked = true, Func<IQueryable<T>, IQueryable<T>>? includes = null)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked) query = query.AsNoTracking();

            if (includes is not null)
            {
                query = includes(query);
            }

            return await query.SingleOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }

            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred while saving changes: {ex.Message}");
                return 0;
            }
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                await _dbSet.AddAsync(entity, cancellationToken);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {

            foreach (var entity in entities)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            if (filter != null)
            {
                return await _dbSet.CountAsync(filter, cancellationToken);
            }
            return await _dbSet.CountAsync(cancellationToken);
        }

    }
}
