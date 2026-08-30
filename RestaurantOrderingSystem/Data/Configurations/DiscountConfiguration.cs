using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrderingSystem.Models.Offer;

namespace RestaurantOrderingSystem.Configurations
{
    public class DiscountConfiguration
        : IEntityTypeConfiguration<Discount>
    {
        public void Configure(
            EntityTypeBuilder<Discount> builder)
        {
            // =====================================
            // TABLE
            // =====================================

            builder.ToTable("Discounts");


            // =====================================
            // PROPERTIES
            // =====================================

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);


            builder.Property(x => x.DiscountPercentage)
                .HasColumnType("decimal(18,2)")
                .IsRequired();


            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);


            // =====================================
            // RESTAURANT RELATIONSHIP
            // =====================================

            builder.HasOne(x => x.Restaurant)
                .WithMany()
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================
            // MENU ITEM RELATIONSHIP
            // =====================================

            builder.HasOne(x => x.MenuItem)
                .WithMany()
                .HasForeignKey(x => x.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}