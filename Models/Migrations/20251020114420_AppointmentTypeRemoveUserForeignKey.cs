using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class AppointmentTypeRemoveUserForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentTypes_AspNetUsers_UserId",
                table: "AppointmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentTypes_UserId",
                table: "AppointmentTypes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppointmentTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AppointmentTypes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentTypes_UserId",
                table: "AppointmentTypes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentTypes_AspNetUsers_UserId",
                table: "AppointmentTypes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
