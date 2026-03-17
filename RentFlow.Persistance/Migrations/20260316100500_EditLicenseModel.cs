using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class EditLicenseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<char>>(
                name: "EntrepreneurDriverLicense_Categories",
                table: "Customers",
                type: "character(1)[]",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EntrepreneurDriverLicense_ExpirationDate",
                table: "Customers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EntrepreneurDriverLicense_IssuedDate",
                table: "Customers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<List<char>>(
                name: "IndividualDriverLicense_Categories",
                table: "Customers",
                type: "character(1)[]",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IndividualDriverLicense_ExpirationDate",
                table: "Customers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IndividualDriverLicense_IssuedDate",
                table: "Customers",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntrepreneurDriverLicense_Categories",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EntrepreneurDriverLicense_ExpirationDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EntrepreneurDriverLicense_IssuedDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualDriverLicense_Categories",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualDriverLicense_ExpirationDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualDriverLicense_IssuedDate",
                table: "Customers");
        }
    }
}
