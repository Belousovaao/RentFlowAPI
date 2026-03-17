using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class SnapshotNew3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_ShortName",
                table: "Bookings",
                newName: "CustomerSnapshot_ShortName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_KPP",
                table: "Bookings",
                newName: "CustomerSnapshot_KPP");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_BankAccount_CurrentAccount",
                table: "Bookings",
                newName: "CustomerSnapshot_BankAccount_CurrentAccount");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_BankAccount_CorrespondentAccount",
                table: "Bookings",
                newName: "CustomerSnapshot_BankAccount_CorrespondentAccount");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_BankAccount_BankName",
                table: "Bookings",
                newName: "CustomerSnapshot_BankAccount_BankName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_BankAccount_BIK",
                table: "Bookings",
                newName: "CustomerSnapshot_BankAccount_BIK");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Phone",
                table: "Bookings",
                newName: "CustomerSnapshot_Phone");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Passport_Serial",
                table: "Bookings",
                newName: "CustomerSnapshot_Passport_Serial");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Passport_RegistrationAddress",
                table: "Bookings",
                newName: "CustomerSnapshot_Passport_RegistrationAddress");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Passport_Number",
                table: "Bookings",
                newName: "CustomerSnapshot_Passport_Number");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Passport_IssuedDate",
                table: "Bookings",
                newName: "CustomerSnapshot_Passport_IssuedDate");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Passport_IssuedBy",
                table: "Bookings",
                newName: "CustomerSnapshot_Passport_IssuedBy");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Name_MiddleName",
                table: "Bookings",
                newName: "CustomerSnapshot_Name_MiddleName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Name_LastName",
                table: "Bookings",
                newName: "CustomerSnapshot_Name_LastName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Name_FirstName",
                table: "Bookings",
                newName: "CustomerSnapshot_Name_FirstName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Individual_Email",
                table: "Bookings",
                newName: "CustomerSnapshot_Email");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_OrganizationAdress",
                table: "Bookings",
                newName: "CustomerSnapshot_OrganizationName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_FullName",
                table: "Bookings",
                newName: "CustomerSnapshot_OrganizationAddress");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Organization_FactAdress",
                table: "Bookings",
                newName: "CustomerSnapshot_FactAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_ShortName",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_ShortName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Phone",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Phone");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Passport_Serial",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Passport_Serial");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Passport_RegistrationAddress",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Passport_RegistrationAddress");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Passport_Number",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Passport_Number");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Passport_IssuedDate",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Passport_IssuedDate");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Passport_IssuedBy",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Passport_IssuedBy");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Name_MiddleName",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Name_MiddleName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Name_LastName",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Name_LastName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Name_FirstName",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Name_FirstName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_KPP",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_KPP");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_Email",
                table: "Bookings",
                newName: "CustomerSnapshot_Individual_Email");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_BankAccount_CurrentAccount",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_BankAccount_CurrentAccount");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_BankAccount_CorrespondentAccount",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_BankAccount_CorrespondentAccount");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_BankAccount_BankName",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_BankAccount_BankName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_BankAccount_BIK",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_BankAccount_BIK");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_OrganizationName",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_OrganizationAdress");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_OrganizationAddress",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_FullName");

            migrationBuilder.RenameColumn(
                name: "CustomerSnapshot_FactAddress",
                table: "Bookings",
                newName: "CustomerSnapshot_Organization_FactAdress");

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
        }
    }
}
