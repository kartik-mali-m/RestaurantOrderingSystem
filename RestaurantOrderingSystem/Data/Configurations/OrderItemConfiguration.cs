using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrderingSystem.Models.Order;

namespace RestaurantOrderingSystem.Data.Configurations
{
    public class OrderItemConfiguration
        : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(
            EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ItemName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.TotalPrice)
                .HasPrecision(18, 2);

            // Order → OrderItems
            builder.HasOne(x => x.Order)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItem → OrderItems
            builder.HasOne(x => x.MenuItem)
                .WithMany()
                .HasForeignKey(x => x.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.OrderId);

            builder.HasIndex(x => x.MenuItemId);
        }
    }
}