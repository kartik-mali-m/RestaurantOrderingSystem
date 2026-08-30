using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrderingSystem.Models.Offer;

namespace RestaurantOrderingSystem.Configurations
{
    public class OfferConfiguration
        : IEntityTypeConfiguration<Offer>
    {
        public void Configure(
            EntityTypeBuilder<Offer> builder)
        {
            // =====================================
            // TABLE
            // =====================================

            builder.ToTable("Offers");


            // =====================================
            // PROPERTIES
            // =====================================

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);


            builder.Property(x => x.Description)
                .HasMaxLength(500);


            builder.Property(x => x.DiscountPercentage)
                .HasColumnType("decimal(18,2)")
                .IsRequired();


            builder.Property(x => x.TargetType)
                .IsRequired();


            builder.Property(x => x.StartDate)
                .IsRequired();


            builder.Property(x => x.EndDate)
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
            // CATEGORY RELATIONSHIP
            // =====================================

            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
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