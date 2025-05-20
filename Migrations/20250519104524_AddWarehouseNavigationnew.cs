using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseNavigationnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Store_WarehouseId",
                table: "ReturnRequests");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Store_WarehouseId",
                table: "ReturnRequests",
                column: "WarehouseId",
                principalTable: "Store",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
