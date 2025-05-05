using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class itemcard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "ManagerJobNum",
            //    table: "Store");

            migrationBuilder.CreateTable(
                name: "ItemCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ItemTypeCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitOfmeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Chemical = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HazardTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TypeOfAsset = table.Column<int>(type: "int", nullable: false),
                    Minimum = table.Column<int>(type: "int", nullable: false),
                    ReorderLimit = table.Column<int>(type: "int", nullable: false),
                    WarehouseStoreId = table.Column<int>(type: "int", nullable: false),
                    QuantityReceived = table.Column<int>(type: "int", nullable: false),
                    DateOfEntry = table.Column<DateOnly>(type: "date", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ShelfId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    ReceiptDocumentnumber = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCards_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCards_ROOMS_RoomId",
                        column: x => x.RoomId,
                        principalTable: "ROOMS",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCards_SHELVES_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "SHELVES",
                        principalColumn: "ShelfId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCards_Store_WarehouseStoreId",
                        column: x => x.WarehouseStoreId,
                        principalTable: "Store",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCards_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_ItemId",
                table: "ItemCards",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_RoomId",
                table: "ItemCards",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_ShelfId",
                table: "ItemCards",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_SupplierId",
                table: "ItemCards",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_WarehouseStoreId",
                table: "ItemCards",
                column: "WarehouseStoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemCards");

            //migrationBuilder.AddColumn<int>(
            //    name: "ManagerJobNum",
            //    table: "Store",
            //    type: "int",
            //    nullable: true);
        }
    }
}
