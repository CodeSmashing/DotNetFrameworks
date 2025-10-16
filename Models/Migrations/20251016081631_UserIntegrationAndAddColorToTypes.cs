using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class UserIntegrationAndAddColorToTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "AppointmentTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "#FF000000");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AppointmentTypes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "-");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "-");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentTypes_UserId",
                table: "AppointmentTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_UserId",
                table: "Appointments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentTypes_AspNetUsers_UserId",
                table: "AppointmentTypes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_UserId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentTypes_AspNetUsers_UserId",
                table: "AppointmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentTypes_UserId",
                table: "AppointmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "AppointmentTypes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppointmentTypes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Appointments");
        }
    }
}
