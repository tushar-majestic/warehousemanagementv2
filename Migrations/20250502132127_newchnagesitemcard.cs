using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class newchnagesitemcard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCards_Store_WarehouseStoreId",
                table: "ItemCards");

            migrationBuilder.RenameColumn(
                name: "WarehouseStoreId",
                table: "ItemCards",
                newName: "StoreId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemCards_WarehouseStoreId",
                table: "ItemCards",
                newName: "IX_ItemCards_StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_GroupCode",
                table: "ItemCards",
                column: "GroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_HazardTypeName",
                table: "ItemCards",
                column: "HazardTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_ItemTypeCode",
                table: "ItemCards",
                column: "ItemTypeCode");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCards_HazardType_HazardTypeName",
                table: "ItemCards",
                column: "HazardTypeName",
                principalTable: "HazardType",
                principalColumn: "HazardTypeName");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCards_ItemGroup_GroupCode",
                table: "ItemCards",
                column: "GroupCode",
                principalTable: "ItemGroup",
                principalColumn: "GroupCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCards_ItemType_ItemTypeCode",
                table: "ItemCards",
                column: "ItemTypeCode",
                principalTable: "ItemType",
                principalColumn: "ItemTypeCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCards_Store_StoreId",
                table: "ItemCards",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCards_HazardType_HazardTypeName",
                table: "ItemCards");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCards_ItemGroup_GroupCode",
                table: "ItemCards");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCards_ItemType_ItemTypeCode",
                table: "ItemCards");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCards_Store_StoreId",
                table: "ItemCards");

            migrationBuilder.DropIndex(
                name: "IX_ItemCards_GroupCode",
                table: "ItemCards");

            migrationBuilder.DropIndex(
                name: "IX_ItemCards_HazardTypeName",
                table: "ItemCards");

            migrationBuilder.DropIndex(
                name: "IX_ItemCards_ItemTypeCode",
                table: "ItemCards");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "ItemCards",
                newName: "WarehouseStoreId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemCards_StoreId",
                table: "ItemCards",
                newName: "IX_ItemCards_WarehouseStoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCards_Store_WarehouseStoreId",
                table: "ItemCards",
                column: "WarehouseStoreId",
                principalTable: "Store",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
