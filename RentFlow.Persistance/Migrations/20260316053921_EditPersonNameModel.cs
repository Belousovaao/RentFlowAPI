using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class EditPersonNameModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name_FirstName",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_LastName",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_MiddleName",
                table: "Customers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name_FirstName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Name_LastName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Name_MiddleName",
                table: "Customers");
        }
    }
}
