namespace CinemaApplication.Repositories
{
    public class MovieImageRepository : Repository<MovieImage>, IRepositoryMovieImage
    {
        public MovieImageRepository(ApplicationDbContext context)
            : base(context)
        {
            
        }
        public void DeleteRange(IEnumerable<MovieImage> movieImages)
        {
            foreach (var item in movieImages)
            {
                _context.MovieImages.Remove(item);
            }
        }
    }
}
