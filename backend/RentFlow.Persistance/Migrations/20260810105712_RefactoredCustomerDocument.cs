using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredCustomerDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerDocument_Customers_CustomerId",
                table: "CustomerDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerDocument",
                table: "CustomerDocument");

            migrationBuilder.RenameTable(
                name: "CustomerDocument",
                newName: "CustomerDocuments");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerDocument_CustomerId",
                table: "CustomerDocuments",
                newName: "IX_CustomerDocuments_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerDocuments",
                table: "CustomerDocuments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerDocuments_Customers_CustomerId",
                table: "CustomerDocuments",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerDocuments_Customers_CustomerId",
                table: "CustomerDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerDocuments",
                table: "CustomerDocuments");

            migrationBuilder.RenameTable(
                name: "CustomerDocuments",
                newName: "CustomerDocument");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerDocuments_CustomerId",
                table: "CustomerDocument",
                newName: "IX_CustomerDocument_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerDocument",
                table: "CustomerDocument",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerDocument_Customers_CustomerId",
                table: "CustomerDocument",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
