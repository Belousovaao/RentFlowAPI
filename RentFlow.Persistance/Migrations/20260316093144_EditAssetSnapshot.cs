using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class EditAssetSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AssetSnapshot_CanDeliver",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AssetSnapshot_Category",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AssetSnapshot_DailyPrice",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AssetSnapshot_DeliveryPrice",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AssetSnapshot_Deposit",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetSnapshot_Name",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AssetSnapshot_Type",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetSnapshot_CanDeliver",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AssetSnapshot_Category",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AssetSnapshot_DailyPrice",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AssetSnapshot_DeliveryPrice",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AssetSnapshot_Deposit",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AssetSnapshot_Name",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AssetSnapshot_Type",
                table: "Bookings");
        }
    }
}
