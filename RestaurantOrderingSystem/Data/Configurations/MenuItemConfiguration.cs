using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrderingSystem.Models.Menu;

namespace RestaurantOrderingSystem.Data.Configurations
{
    public class MenuItemConfiguration
        : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(
            EntityTypeBuilder<MenuItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.Price)
                .HasPrecision(18, 2);

            // Restaurant → MenuItems
            builder.HasOne(x => x.Restaurant)
                .WithMany()
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category → MenuItems
            builder.HasOne(x => x.Category)
                .WithMany(x => x.MenuItems)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Useful for restaurant menu queries
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.CategoryId
            });
        }
    }
}