using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "nl");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LanguageCode",
                table: "AspNetUsers",
                column: "LanguageCode");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Languages_LanguageCode",
                table: "AspNetUsers",
                column: "LanguageCode",
                principalTable: "Languages",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Languages_LanguageCode",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LanguageCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "AspNetUsers");
        }
    }
}
