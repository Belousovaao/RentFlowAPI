using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredAssetSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssetSnapshot_Name",
                table: "Bookings",
                newName: "AssetSnapshot_Model");

            migrationBuilder.AddColumn<string>(
                name: "AssetSnapshot_BrandName",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AssetSnapshot_Year",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetSnapshot_BrandName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AssetSnapshot_Year",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "AssetSnapshot_Model",
                table: "Bookings",
                newName: "AssetSnapshot_Name");
        }
    }
}
