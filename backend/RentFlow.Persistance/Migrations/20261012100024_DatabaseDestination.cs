using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseDestination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price_RouteSurcharge",
                table: "Bookings",
                newName: "RouteSurcharge");

            migrationBuilder.AlterColumn<decimal>(
                name: "RouteSurcharge",
                table: "Bookings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "CustomRouteDescription",
                table: "Bookings",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationNameSnapshot",
                table: "Bookings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationType",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "Local");

            migrationBuilder.AddColumn<string>(
                name: "RouteReviewStatus",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "NotRequired");

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Region = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DistanceKm = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RouteSurcharge = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropColumn(
                name: "CustomRouteDescription",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DestinationNameSnapshot",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DestinationType",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RouteReviewStatus",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "RouteSurcharge",
                table: "Bookings",
                newName: "Price_RouteSurcharge");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price_RouteSurcharge",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}
