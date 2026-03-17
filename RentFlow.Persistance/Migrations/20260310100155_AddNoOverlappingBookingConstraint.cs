using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddNoOverlappingBookingConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE EXTENSION IF NOT EXISTS btree_gist;

            ALTER TABLE ""Bookings""
            ADD CONSTRAINT ""no_overlapping_bookings""
            EXCLUDE USING gist (
                ""AssetId"" WITH =, tstzrange(""StartDate"", ""EndDate"", '[]') WITH &&);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Bookings""
                DROP CONTRAINT IF EXISTS ""no_overlapping_bookings""; 
            ");
        }
    }
}
