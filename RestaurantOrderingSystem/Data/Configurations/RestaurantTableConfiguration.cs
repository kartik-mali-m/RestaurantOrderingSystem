using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrderingSystem.Models.Restaurant;

namespace RestaurantOrderingSystem.Data.Configurations
{
    public class RestaurantTableConfiguration
        : IEntityTypeConfiguration<RestaurantTable>
    {
        public void Configure(
            EntityTypeBuilder<RestaurantTable> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TableNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Capacity)
                .IsRequired();

            builder.Property(x => x.IsAvailable)
                .IsRequired();

            // Restaurant → RestaurantTables
            builder.HasOne(x => x.Restaurant)
                .WithMany()
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);



            // Prevent duplicate table numbers
            // inside the same restaurant
            builder.HasIndex(x => new
            {
                x.RestaurantId,
                x.TableNumber
            })
            .IsUnique();


        }
    }
}