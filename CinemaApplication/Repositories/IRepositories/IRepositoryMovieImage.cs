namespace CinemaApplication.Repositories.IRepositories
{
    public interface IRepositoryMovieImage : IRepository<MovieImage>
    {
        void DeleteRange(IEnumerable<MovieImage> movieImages);
    }
}