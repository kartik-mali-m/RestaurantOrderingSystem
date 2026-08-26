using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Identity;

namespace RestaurantOrderingSystem.Data.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "SuperAdmin");

            if (adminRole == null)
            {
                return;
            }

            var adminExists = await context.Users
                .AnyAsync(u => u.Email == "admin@restaurant.com");

            if (adminExists)
            {
                return;
            }

            var passwordHasher = new PasswordHasher<object>();

            var admin = new User
            {
                Name = "System Administrator",

                Email = "admin@restaurant.com",

                RoleId = adminRole.Id,

                RestaurantId = null
            };

            admin.PasswordHash =
                passwordHasher.HashPassword(
                    new object(),
                    "Admin@123");

            context.Users.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}