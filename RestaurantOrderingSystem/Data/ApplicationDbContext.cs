using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Models.Identity;
using RestaurantOrderingSystem.Models.Menu;
using RestaurantOrderingSystem.Models.Offer;
using RestaurantOrderingSystem.Models.QRCode;
using RestaurantOrderingSystem.Models.Restaurant;

namespace RestaurantOrderingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<RestaurantQRCode> RestaurantQRCodes { get; set; }

        public DbSet<RestaurantTable> RestaurantTables { get; set; }

        public DbSet<Offer> Offers { get; set; }

        public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // IMPORTANT
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Restaurant)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<RestaurantTable>()
    .HasOne(t => t.Restaurant)
    .WithMany()
    .HasForeignKey(t => t.RestaurantId)
    .OnDelete(DeleteBehavior.Cascade);
        }

    }
}