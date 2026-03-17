using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedSnapshotsForName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name_FirstName",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_LastName",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_MiddleName",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name_FirstName",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Name_LastName",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Name_MiddleName",
                table: "BookingCustomerSnapshot");
        }
    }
}
