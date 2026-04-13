using System.Linq.Expressions;

namespace CinemaApplication.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null,
            CancellationToken cancellationToken = default,
            Func<IQueryable<T>, IQueryable<T>>? includes = null,
            bool tracked = true);
        Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            CancellationToken cancellationToken = default,
            bool tracked = true,
            Func<IQueryable<T>, IQueryable<T>>? includes = null);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
    }
}
