using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class storeDeletedManagerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerJobNum",
                table: "Store");

            migrationBuilder.RenameColumn(
                name: "WarehouseManagerName",
                table: "Store",
                newName: "WarehouseManagerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WarehouseManagerID",
                table: "Store",
                newName: "WarehouseManagerName");

            migrationBuilder.AddColumn<int>(
                name: "ManagerJobNum",
                table: "Store",
                type: "int",
                nullable: true);
        }
    }
}
