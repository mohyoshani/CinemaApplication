using CinemaApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CinemaApplication.ViewModels;

namespace CinemaApplication.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }


        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CinemaHall> CinemaHalls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<MovieTheater> MovieTheaters { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartItemSeat> CartItemSeats { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemSeat> OrderItemSeats { get; set; }
        public DbSet<FavouriteMovies> FavouriteMovies { get; set; }
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CartItemSeat>()
        .HasOne(cis => cis.Seat)
        .WithMany()
        .HasForeignKey(cis => cis.SeatId)
        .OnDelete(DeleteBehavior.Restrict);

           
            modelBuilder.Entity<CartItemSeat>()
                .HasOne(cis => cis.CartItem)
                .WithMany(ci => ci.SelectedSeats)
                .HasForeignKey(cis => cis.CartItemId)
                .OnDelete(DeleteBehavior.Restrict);

        }
        public DbSet<CinemaApplication.ViewModels.RegisterVM> RegisterVM { get; set; } = default!;
        public DbSet<CinemaApplication.ViewModels.LoginVM> LoginVM { get; set; } = default!;
        public DbSet<CinemaApplication.ViewModels.ResendConfirmationVM> ResendConfirmationVM { get; set; } = default!;
    }
}

