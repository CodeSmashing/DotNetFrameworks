using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleModelAndReferenceInAgendaUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LicencePlate = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoadCapacity = table.Column<double>(type: "float", nullable: false),
                    WeightCapacity = table.Column<double>(type: "float", nullable: false),
                    FuelType = table.Column<int>(type: "int", nullable: false),
                    IsManuel = table.Column<bool>(type: "bit", nullable: false),
                    IsInUse = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VehicleId",
                table: "AspNetUsers",
                column: "VehicleId",
                unique: true,
                filter: "[VehicleId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Vehicles_VehicleId",
                table: "AspNetUsers",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Vehicles_VehicleId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VehicleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "AspNetUsers");
        }
    }
}
