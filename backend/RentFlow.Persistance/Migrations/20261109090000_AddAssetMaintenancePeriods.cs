using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RentFlow.Persistance;

#nullable disable

namespace RentFlow.Persistance.Migrations;

[DbContext(typeof(RentFlowDbContext))]
[Migration("20261109090000_AddAssetMaintenancePeriods")]
public partial class AddAssetMaintenancePeriods : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AssetMaintenancePeriods",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssetMaintenancePeriods", x => x.Id);
                table.CheckConstraint("CK_AssetMaintenancePeriods_DateRange", "\"EndDate\" > \"StartDate\"");
                table.ForeignKey(
                    name: "FK_AssetMaintenancePeriods_Assets_AssetId",
                    column: x => x.AssetId,
                    principalTable: "Assets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AssetMaintenancePeriods_AssetId_StartDate_EndDate",
            table: "AssetMaintenancePeriods",
            columns: new[] { "AssetId", "StartDate", "EndDate" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AssetMaintenancePeriods");
    }
}
