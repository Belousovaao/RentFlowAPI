using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseNameForEmailAndPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_AppUsers_Phone",
                table: "AppUsers",
                newName: "UX_AppUsers_Phone");

            migrationBuilder.RenameIndex(
                name: "IX_AppUsers_Email",
                table: "AppUsers",
                newName: "UX_AppUsers_Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_AppUsers_Phone",
                table: "AppUsers",
                newName: "IX_AppUsers_Phone");

            migrationBuilder.RenameIndex(
                name: "UX_AppUsers_Email",
                table: "AppUsers",
                newName: "IX_AppUsers_Email");
        }
    }
}
