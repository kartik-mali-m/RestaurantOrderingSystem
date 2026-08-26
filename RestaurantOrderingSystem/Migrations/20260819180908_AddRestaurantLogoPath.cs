using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantOrderingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantLogoPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Logo",
                table: "Restaurants",
                newName: "LogoPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoPath",
                table: "Restaurants",
                newName: "Logo");
        }
    }
}
