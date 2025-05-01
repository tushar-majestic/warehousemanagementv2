using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class StoreDeletedManagerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarehouseManagerID",
                table: "Store");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseManagerID",
                table: "Store",
                type: "int",
                nullable: true);
        }
    }
}
