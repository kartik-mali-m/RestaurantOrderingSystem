using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantOrderingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Restaurants");
        }
    }
}
