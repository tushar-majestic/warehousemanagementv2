using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerJobNum",
                table: "Store",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseManagerName",
                table: "Store",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseStatus",
                table: "Store",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerJobNum",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "WarehouseManagerName",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "WarehouseStatus",
                table: "Store");
        }
    }
}
