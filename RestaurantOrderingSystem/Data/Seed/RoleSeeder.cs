using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Identity;

namespace RestaurantOrderingSystem.Data.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role
                    {
                        Name = "SuperAdmin"
                    },
                    new Role
                    {
                        Name = "RestaurantAdmin"
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}