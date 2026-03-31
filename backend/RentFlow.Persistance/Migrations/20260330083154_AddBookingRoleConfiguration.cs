using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingRoleConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRole_Bookings_BookingId",
                table: "BookingRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_BookingRole_SignatoryId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SignatoryId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BoolingId",
                table: "BookingRole");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                table: "BookingRole",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SignatoryId",
                table: "Bookings",
                column: "SignatoryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRole_Bookings_BookingId",
                table: "BookingRole",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_BookingRole_SignatoryId",
                table: "Bookings",
                column: "SignatoryId",
                principalTable: "BookingRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingRole_Bookings_BookingId",
                table: "BookingRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_BookingRole_SignatoryId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SignatoryId",
                table: "Bookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                table: "BookingRole",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "BoolingId",
                table: "BookingRole",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SignatoryId",
                table: "Bookings",
                column: "SignatoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingRole_Bookings_BookingId",
                table: "BookingRole",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_BookingRole_SignatoryId",
                table: "Bookings",
                column: "SignatoryId",
                principalTable: "BookingRole",
                principalColumn: "Id");
        }
    }
}
