using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Warehouse",
                table: "ReturnRequests",
                newName: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_WarehouseId",
                table: "ReturnRequests",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Store_WarehouseId",
                table: "ReturnRequests",
                column: "WarehouseId",
                principalTable: "Store",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Store_WarehouseId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_WarehouseId",
                table: "ReturnRequests");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "ReturnRequests",
                newName: "Warehouse");
        }
    }
}
