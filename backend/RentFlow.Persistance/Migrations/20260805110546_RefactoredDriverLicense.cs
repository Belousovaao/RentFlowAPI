using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredDriverLicense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntrepreneurDriverLicense_ExprerienceFromYear",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IndividualDriverLicense_ExprerienceFromYear",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookingDriver_License_ExprerienceFromYear",
                table: "Bookings",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntrepreneurDriverLicense_ExprerienceFromYear",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualDriverLicense_ExprerienceFromYear",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BookingDriver_License_ExprerienceFromYear",
                table: "Bookings");
        }
    }
}
