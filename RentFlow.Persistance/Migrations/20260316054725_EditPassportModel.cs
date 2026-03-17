using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class EditPassportModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IPPassport_IssuedBy",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IPPassport_IssuedDate",
                table: "Customers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPPassport_Number",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPPassport_Serial",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndividualPassport_IssuedBy",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IndividualPassport_IssuedDate",
                table: "Customers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndividualPassport_Number",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndividualPassport_Serial",
                table: "Customers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPPassport_IssuedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IPPassport_IssuedDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IPPassport_Number",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IPPassport_Serial",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualPassport_IssuedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualPassport_IssuedDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualPassport_Number",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualPassport_Serial",
                table: "Customers");
        }
    }
}
