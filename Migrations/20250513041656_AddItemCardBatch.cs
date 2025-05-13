using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class AddItemCardBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCards_ROOMS_RoomId",
                table: "ItemCards");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCards_SHELVES_ShelfId",
                table: "ItemCards");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemCards_Supplier_SupplierId",
                table: "ItemCards");

            migrationBuilder.DropIndex(
                name: "IX_ItemCards_RoomId",
                table: "ItemCards");

            migrationBuilder.DropIndex(
                name: "IX_ItemCards_ShelfId",
                table: "ItemCards");

            migrationBuilder.DropIndex(
                name: "IX_ItemCards_SupplierId",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "Minimum",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "ReorderLimit",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "ShelfId",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "TypeOfAsset",
                table: "ItemCards");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "ItemCards",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ItemCardBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCardId = table.Column<int>(type: "int", nullable: false),
                    QuantityReceived = table.Column<int>(type: "int", nullable: false),
                    TypeOfAsset = table.Column<int>(type: "int", nullable: false),
                    Minimum = table.Column<int>(type: "int", nullable: false),
                    ReorderLimit = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ShelfId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCardBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCardBatches_ItemCards_ItemCardId",
                        column: x => x.ItemCardId,
                        principalTable: "ItemCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCardBatches_ROOMS_RoomId",
                        column: x => x.RoomId,
                        principalTable: "ROOMS",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCardBatches_SHELVES_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "SHELVES",
                        principalColumn: "ShelfId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCardBatches_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemCardBatches_ItemCardId",
                table: "ItemCardBatches",
                column: "ItemCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCardBatches_RoomId",
                table: "ItemCardBatches",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCardBatches_ShelfId",
                table: "ItemCardBatches",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCardBatches_SupplierId",
                table: "ItemCardBatches",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemCardBatches");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentType",
                table: "ItemCards",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Minimum",
                table: "ItemCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReorderLimit",
                table: "ItemCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "ItemCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShelfId",
                table: "ItemCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "ItemCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeOfAsset",
                table: "ItemCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCards_ROOMS_RoomId",
                table: "ItemCards",
                column: "RoomId",
                principalTable: "ROOMS",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCards_SHELVES_ShelfId",
                table: "ItemCards",
                column: "ShelfId",
                principalTable: "SHELVES",
                principalColumn: "ShelfId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCards_Supplier_SupplierId",
                table: "ItemCards",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
