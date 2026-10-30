using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations;

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