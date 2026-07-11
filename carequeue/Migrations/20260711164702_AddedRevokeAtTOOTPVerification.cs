using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carequeue.Migrations
{
    /// <inheritdoc />
    public partial class AddedRevokeAtTOOTPVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "OtpVerifications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "OtpVerifications");
        }
    }
}
