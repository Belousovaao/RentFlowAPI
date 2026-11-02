using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RentFlow.Persistance;

#nullable disable

namespace RentFlow.Persistance.Migrations;

[DbContext(typeof(RentFlowDbContext))]
[Migration("20261102090000_BackfillInitialBookingCharges")]
public partial class BackfillInitialBookingCharges : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Historical booking creation timestamps are not stored. Per the chosen fallback,
        // use each booking's StartDate as an approximate CreatedAt for backfilled charges.
        // The inserts are idempotent per booking and charge type, and do not touch payments
        // or existing allocations.
        migrationBuilder.Sql("""
            WITH initial_charges AS (
                SELECT
                    b."Id" AS "BookingId",
                    'Rental'::character varying(40) AS "Type",
                    'Аренда'::character varying(160) AS "Title",
                    COALESCE(SUM(s."Total"), 0)::numeric(18,2) AS "Amount",
                    b."StartDate" AS "CreatedAt"
                FROM "Bookings" b
                LEFT JOIN "BookingRentalPriceSegments" s
                    ON s."RentalPriceBookingPriceBookingId" = b."Id"
                GROUP BY b."Id", b."StartDate"

                UNION ALL

                SELECT b."Id", 'AdditionalServices'::character varying(40),
                    'Дополнительные услуги'::character varying(160),
                    b."ServicesPrice"::numeric(18,2), b."StartDate"
                FROM "Bookings" b

                UNION ALL

                SELECT b."Id", 'RouteSurcharge'::character varying(40),
                    'Доплата за маршрут'::character varying(160),
                    b."RouteSurcharge"::numeric(18,2), b."StartDate"
                FROM "Bookings" b

                UNION ALL

                SELECT b."Id", 'Deposit'::character varying(40),
                    'Депозит'::character varying(160),
                    b."Deposit"::numeric(18,2), b."StartDate"
                FROM "Bookings" b
            )
            INSERT INTO "BookingCharges"
                ("Id", "BookingId", "Type", "Status", "Title", "Description",
                 "ReferenceNumber", "Quantity", "Unit", "UnitPrice", "Amount",
                 "Currency", "CreatedAt", "CreatedByUserId", "ReviewedAt", "ReviewedByUserId")
            SELECT
                gen_random_uuid(), c."BookingId", c."Type", 'Accrued', c."Title", NULL,
                NULL, 1, 'услуга', c."Amount", c."Amount", 'RUB', c."CreatedAt",
                NULL, NULL, NULL
            FROM initial_charges c
            WHERE c."Amount" > 0
              AND NOT EXISTS (
                  SELECT 1
                  FROM "BookingCharges" existing
                  WHERE existing."BookingId" = c."BookingId"
                    AND existing."Type" = c."Type"
              );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Backfilled rows are intentionally not deleted: they may already have allocations
        // or be referenced by later finance operations.
    }
}
