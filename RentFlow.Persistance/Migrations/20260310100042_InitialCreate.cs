using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    BrandName = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: false),
                    FullDescription = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    DailyPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Deposit = table.Column<decimal>(type: "numeric", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanDeliver = table.Column<bool>(type: "boolean", nullable: false),
                    DeliveryPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    IndividualPassport_RegistrationAddress = table.Column<string>(type: "text", nullable: true),
                    IndividualDriverLicense_Number = table.Column<string>(type: "text", nullable: true),
                    IndividualBankAccount_CurrentAccount = table.Column<string>(type: "text", nullable: true),
                    IndividualBankAccount_CorrespondentAccount = table.Column<string>(type: "text", nullable: true),
                    IndividualBankAccount_BIK = table.Column<string>(type: "text", nullable: true),
                    IndividualBankAccount_BankName = table.Column<string>(type: "text", nullable: true),
                    IPPassport_RegistrationAddress = table.Column<string>(type: "text", nullable: true),
                    INN = table.Column<string>(type: "text", nullable: true),
                    OGRNIP = table.Column<string>(type: "text", nullable: true),
                    OrganizationAdress = table.Column<string>(type: "text", nullable: true),
                    FactAdress = table.Column<string>(type: "text", nullable: true),
                    IPBankAccount_CurrentAccount = table.Column<string>(type: "text", nullable: true),
                    IPBankAccount_CorrespondentAccount = table.Column<string>(type: "text", nullable: true),
                    IPBankAccount_BIK = table.Column<string>(type: "text", nullable: true),
                    IPBankAccount_BankName = table.Column<string>(type: "text", nullable: true),
                    RepresentativeId = table.Column<Guid>(type: "uuid", nullable: true),
                    LegalFormShortName = table.Column<string>(type: "text", nullable: true),
                    LegalFormFullName = table.Column<string>(type: "text", nullable: true),
                    EntrepreneurDriverLicense_Number = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    Organization_INN = table.Column<string>(type: "text", nullable: true),
                    OGRN = table.Column<string>(type: "text", nullable: true),
                    KPP = table.Column<string>(type: "text", nullable: true),
                    OrganizationAddress = table.Column<string>(type: "text", nullable: true),
                    FactAddress = table.Column<string>(type: "text", nullable: true),
                    Organization_RepresentativeId = table.Column<Guid>(type: "uuid", nullable: true),
                    SigningBasis_AttorneyNumber = table.Column<string>(type: "text", nullable: true),
                    SigningBasis_AttorneyDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OrganizationBankAccount_CurrentAccount = table.Column<string>(type: "text", nullable: true),
                    OrganizationBankAccount_CorrespondentAccount = table.Column<string>(type: "text", nullable: true),
                    OrganizationBankAccount_BIK = table.Column<string>(type: "text", nullable: true),
                    OrganizationBankAccount_BankName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PricingRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Multiplier = table.Column<decimal>(type: "numeric", nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetPhotos_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingCustomerSnapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCustomerSnapshot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoolingId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleType = table.Column<int>(type: "integer", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SignatoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookingDriver_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingDriver_Name_FirstName = table.Column<string>(type: "text", nullable: false),
                    BookingDriver_Name_LastName = table.Column<string>(type: "text", nullable: false),
                    BookingDriver_Name_MiddleName = table.Column<string>(type: "text", nullable: true),
                    BookingDriver_License_Number = table.Column<string>(type: "text", nullable: false),
                    BookingDriver_License_Categories = table.Column<string>(type: "text", nullable: false),
                    BookingDriver_License_IssuedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BookingDriver_License_ExpirationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BookingDriver_Phone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_BookingRole_SignatoryId",
                        column: x => x.SignatoryId,
                        principalTable: "BookingRole",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "City Center" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Airport" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetPhotos_AssetId",
                table: "AssetPhotos",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomerSnapshot_BookingId",
                table: "BookingCustomerSnapshot",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingRole_BookingId",
                table: "BookingRole",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SignatoryId",
                table: "Bookings",
                column: "SignatoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingCustomerSnapshot_Bookings_BookingId",
                table: "BookingCustomerSnapshot",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRole_Bookings_BookingId",
                table: "BookingRole",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRole_Bookings_BookingId",
                table: "BookingRole");

            migrationBuilder.DropTable(
                name: "AssetPhotos");

            migrationBuilder.DropTable(
                name: "BookingCustomerSnapshot");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "PricingRules");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "BookingRole");
        }
    }
}
