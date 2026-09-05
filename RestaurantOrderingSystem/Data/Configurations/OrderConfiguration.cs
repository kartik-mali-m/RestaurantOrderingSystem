using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrderingSystem.Models.Order;

namespace RestaurantOrderingSystem.Data.Configurations
{
    public class OrderConfiguration
        : IEntityTypeConfiguration<Order>
    {
        public void Configure(
            EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.CustomerPhone)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(x => x.SubTotal)
                .HasPrecision(18, 2);

            builder.Property(x => x.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            // Restaurant → Orders
            builder.HasOne(x => x.Restaurant)
                .WithMany()
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Restaurant Table → Orders
            builder.HasOne(x => x.Table)
                .WithMany()
                .HasForeignKey(x => x.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → OrderItems
            builder.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.RestaurantId);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}