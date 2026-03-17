using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class Snapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingCustomerSnapshot");

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_Email",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_FactAdress",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_IPBankAccount_BIK",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_IPBankAccount_BankName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_IPBankAccount_CorrespondentAccou~",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_IPBankAccount_CurrentAccount",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_Name_FirstName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_Name_LastName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_Name_MiddleName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_OrganizationAdress",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Entrepreneur_Phone",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Email",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Name_FirstName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Name_LastName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Name_MiddleName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Passport_IssuedBy",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CustomerSnapshot_Individual_Passport_IssuedDate",
                table: "Bookings",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Passport_Number",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Passport_RegistrationAddress",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Passport_Serial",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Individual_Phone",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_BankAccount_BIK",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_BankAccount_BankName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_BankAccount_CorrespondentAccount",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_BankAccount_CurrentAccount",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_FactAdress",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_FullName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_KPP",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_OrganizationAdress",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSnapshot_Organization_ShortName",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerSnapshot_Type",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_Email",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_FactAdress",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_IPBankAccount_BIK",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_IPBankAccount_BankName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_IPBankAccount_CorrespondentAccou~",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_IPBankAccount_CurrentAccount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_Name_FirstName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_Name_LastName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_Name_MiddleName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_OrganizationAdress",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Entrepreneur_Phone",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Email",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Name_FirstName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Name_LastName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Name_MiddleName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Passport_IssuedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Passport_IssuedDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Passport_Number",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Passport_RegistrationAddress",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Passport_Serial",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Individual_Phone",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_BankAccount_BIK",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_BankAccount_BankName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_BankAccount_CorrespondentAccount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_BankAccount_CurrentAccount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_FactAdress",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_FullName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_KPP",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_OrganizationAdress",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Organization_ShortName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerSnapshot_Type",
                table: "Bookings");

            migrationBuilder.CreateTable(
                name: "BookingCustomerSnapshot",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    BookingEntrepreneurSnapshot_Email = table.Column<string>(type: "text", nullable: true),
                    BookingEntrepreneurSnapshot_FactAdress = table.Column<string>(type: "text", nullable: true),
                    BookingEntrepreneurSnapshot_OrganizationAdress = table.Column<string>(type: "text", nullable: true),
                    BookingEntrepreneurSnapshot_Phone = table.Column<string>(type: "text", nullable: true),
                    IPBankAccount_BIK = table.Column<string>(type: "text", nullable: true),
                    IPBankAccount_BankName = table.Column<string>(type: "text", nullable: true),
                    IPBankAccount_CorrespondentAccount = table.Column<string>(type: "text", nullable: true),
                    IPBankAccount_CurrentAccount = table.Column<string>(type: "text", nullable: true),
                    Name_FirstName = table.Column<string>(type: "text", nullable: true),
                    Name_LastName = table.Column<string>(type: "text", nullable: true),
                    Name_MiddleName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Passport_IssuedBy = table.Column<string>(type: "text", nullable: true),
                    Passport_IssuedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Passport_Number = table.Column<string>(type: "text", nullable: true),
                    Passport_RegistrationAddress = table.Column<string>(type: "text", nullable: true),
                    Passport_Serial = table.Column<string>(type: "text", nullable: true),
                    FactAdress = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    KPP = table.Column<string>(type: "text", nullable: true),
                    OrganizationAdress = table.Column<string>(type: "text", nullable: true),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    BankAccount_BIK = table.Column<string>(type: "text", nullable: true),
                    BankAccount_BankName = table.Column<string>(type: "text", nullable: true),
                    BankAccount_CorrespondentAccount = table.Column<string>(type: "text", nullable: true),
                    BankAccount_CurrentAccount = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCustomerSnapshot", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_BookingCustomerSnapshot_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
