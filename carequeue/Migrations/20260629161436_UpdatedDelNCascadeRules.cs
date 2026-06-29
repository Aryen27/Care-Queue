using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace carequeue.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDelNCascadeRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_CreatedByUserId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtpVerifications_Users_UserId",
                table: "OtpVerifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OtpVerifications",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_OtpVerifications_UserId",
                table: "OtpVerifications",
                newName: "IX_OtpVerifications_CustomerId");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Appointments",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_CreatedByUserId",
                table: "Appointments",
                newName: "IX_Appointments_CustomerId");

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "OtpVerifications",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Customers_CustomerId",
                table: "Appointments",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OtpVerifications_Customers_CustomerId",
                table: "OtpVerifications",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Customers_CustomerId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_OtpVerifications_Customers_CustomerId",
                table: "OtpVerifications");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "OtpVerifications",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OtpVerifications_CustomerId",
                table: "OtpVerifications",
                newName: "IX_OtpVerifications_UserId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Appointments",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_CustomerId",
                table: "Appointments",
                newName: "IX_Appointments_CreatedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "OtpVerifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_CreatedByUserId",
                table: "Appointments",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OtpVerifications_Users_UserId",
                table: "OtpVerifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
