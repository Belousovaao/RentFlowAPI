using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RentFlow.Persistance;

#nullable disable

namespace RentFlow.Persistance.Migrations;

[DbContext(typeof(RentFlowDbContext))]
[Migration("20261030090000_SimplifyBookingFinanceLifecycle")]
public partial class SimplifyBookingFinanceLifecycle : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE "BookingCharges"
            SET "Status" = 'Accrued'
            WHERE "Status" IN ('PendingApproval', 'Confirmed');
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE "BookingCharges"
            SET "Status" = 'Confirmed'
            WHERE "Status" = 'Accrued';
            """);
    }
}