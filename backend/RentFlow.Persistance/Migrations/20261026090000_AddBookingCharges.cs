using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RentFlow.Persistance;
#nullable disable
namespace RentFlow.Persistance.Migrations;
[DbContext(typeof(RentFlowDbContext))]
[Migration("20261026090000_AddBookingCharges")]
public partial class AddBookingCharges : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "BookingChargeSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                LateCancellationTariff = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                RefuelingPricePerLiter = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BookingChargeSettings", x => x.Id);
                table.CheckConstraint("CK_BookingChargeSettings_Singleton", "\"Id\" = 1");
            });

        migrationBuilder.InsertData(
            table: "BookingChargeSettings",
            columns: new[] { "Id", "LateCancellationTariff", "RefuelingPricePerLiter", "UpdatedAt", "UpdatedByUserId" },
            values: new object[] { 1, 0m, 0m, new DateTimeOffset(2026, 10, 26, 0, 0, 0, TimeSpan.Zero), null });

        migrationBuilder.CreateTable(
            name: "BookingCharges",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BookingCharges", x => x.Id);
                table.ForeignKey("FK_BookingCharges_Bookings_BookingId", x => x.BookingId, "Bookings", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "PaymentAllocations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                BookingChargeId = table.Column<Guid>(type: "uuid", nullable: false),
                Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaymentAllocations", x => x.Id);
                table.ForeignKey("FK_PaymentAllocations_Payments_PaymentId", x => x.PaymentId, "Payments", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_PaymentAllocations_BookingCharges_BookingChargeId", x => x.BookingChargeId, "BookingCharges", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_BookingCharges_BookingId_Status", "BookingCharges", new[] { "BookingId", "Status" });
        migrationBuilder.CreateIndex("IX_PaymentAllocations_PaymentId", "PaymentAllocations", "PaymentId");
        migrationBuilder.CreateIndex("IX_PaymentAllocations_BookingChargeId", "PaymentAllocations", "BookingChargeId");
        migrationBuilder.CreateIndex("IX_PaymentAllocations_PaymentId_BookingChargeId", "PaymentAllocations", new[] { "PaymentId", "BookingChargeId" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("PaymentAllocations");
        migrationBuilder.DropTable("BookingCharges");
        migrationBuilder.DropTable("BookingChargeSettings");
    }
}