using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDriverLicenseCategoriesType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "IndividualDriverLicense_Categories",
                table: "Customers",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "character varying(10)[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "EntrepreneurDriverLicense_Categories",
                table: "Customers",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "character varying(10)[]",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "IndividualDriverLicense_Categories",
                table: "Customers",
                type: "character varying(10)[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "EntrepreneurDriverLicense_Categories",
                table: "Customers",
                type: "character varying(10)[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);
        }
    }
}
