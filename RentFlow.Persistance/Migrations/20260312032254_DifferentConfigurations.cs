using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DifferentConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingCustomerSnapshot",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropIndex(
                name: "IX_BookingCustomerSnapshot_BookingId",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.AlterColumn<string>(
                name: "BookingDriver_Name_LastName",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookingDriver_Name_FirstName",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookingDriver_License_Number",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookingDriver_License_Categories",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_BIK",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_BankName",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_CorrespondentAccount",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount_CurrentAccount",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingEntrepreneurSnapshot_Email",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingEntrepreneurSnapshot_FactAdress",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingEntrepreneurSnapshot_OrganizationAdress",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingEntrepreneurSnapshot_Phone",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactAdress",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPBankAccount_BIK",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPBankAccount_BankName",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPBankAccount_CorrespondentAccount",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPBankAccount_CurrentAccount",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KPP",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationAdress",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Passport_IssuedBy",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Passport_IssuedDate",
                table: "BookingCustomerSnapshot",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Passport_Number",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Passport_RegistrationAddress",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Passport_Serial",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "BookingCustomerSnapshot",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingCustomerSnapshot",
                table: "BookingCustomerSnapshot",
                column: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingCustomerSnapshot",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "BankAccount_BIK",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "BankAccount_BankName",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "BankAccount_CorrespondentAccount",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "BankAccount_CurrentAccount",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "BookingEntrepreneurSnapshot_Email",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "BookingEntrepreneurSnapshot_FactAdress",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "BookingEntrepreneurSnapshot_OrganizationAdress",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "BookingEntrepreneurSnapshot_Phone",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "FactAdress",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "IPBankAccount_BIK",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "IPBankAccount_BankName",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "IPBankAccount_CorrespondentAccount",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "IPBankAccount_CurrentAccount",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "KPP",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "OrganizationAdress",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Passport_IssuedBy",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Passport_IssuedDate",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Passport_Number",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Passport_RegistrationAddress",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Passport_Serial",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "BookingCustomerSnapshot");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "BookingCustomerSnapshot");

            migrationBuilder.AlterColumn<string>(
                name: "BookingDriver_Name_LastName",
                table: "Bookings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BookingDriver_Name_FirstName",
                table: "Bookings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BookingDriver_License_Number",
                table: "Bookings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BookingDriver_License_Categories",
                table: "Bookings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingCustomerSnapshot",
                table: "BookingCustomerSnapshot",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "City Center" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Airport" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomerSnapshot_BookingId",
                table: "BookingCustomerSnapshot",
                column: "BookingId",
                unique: true);
        }
    }
}
