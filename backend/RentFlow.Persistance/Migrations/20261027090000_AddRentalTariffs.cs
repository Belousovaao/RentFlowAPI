using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RentFlow.Persistance;

#nullable disable

namespace RentFlow.Persistance.Migrations;

[DbContext(typeof(RentFlowDbContext))]
[Migration("20261027090000_AddRentalTariffs")]
public partial class AddRentalTariffs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "RentalTariffs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RentalTariffs", x => x.Id);
                table.CheckConstraint("CK_RentalTariffs_Price_NonNegative", "\"Price\" >= 0");
            });

        migrationBuilder.CreateIndex(
            name: "IX_RentalTariffs_Code",
            table: "RentalTariffs",
            column: "Code",
            unique: true);

        var initialUpdatedAt = new DateTimeOffset(2026, 10, 27, 0, 0, 0, TimeSpan.Zero);
        migrationBuilder.InsertData(
            table: "RentalTariffs",
            columns: new[] { "Id", "Code", "Price", "Unit", "UpdatedAtUtc" },
            columnTypes: new[] { "uuid", "character varying(100)", "numeric(18,2)", "character varying(20)", "timestamp with time zone" },
            values: new object[,]
            {
                { new Guid("c1f6bce5-4d55-4b31-9b2e-1c20a7e9c101"), "fuel_per_liter", 100m, "л", initialUpdatedAt },
                { new Guid("c1f6bce5-4d55-4b31-9b2e-1c20a7e9c102"), "excess_mileage_per_km", 15m, "км", initialUpdatedAt },
                { new Guid("c1f6bce5-4d55-4b31-9b2e-1c20a7e9c103"), "body_wash", 2400m, "услуга", initialUpdatedAt }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "RentalTariffs");
    }
}
