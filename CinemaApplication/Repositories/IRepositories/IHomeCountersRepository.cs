using System.Linq.Expressions;

namespace CinemaApplication.Repositories.IRepositories
{
    public interface IHomeCountersRepository
    {
        Task<int> GetCountAsync<T>(CancellationToken cancellationToken = default) where T : class;
    }
}
