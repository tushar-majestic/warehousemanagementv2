using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ShelveItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "ShelveItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCardId = table.Column<int>(type: "int", nullable: false),
                    ShelfId = table.Column<int>(type: "int", nullable: false),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShelveItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShelveItems_ItemCards_ItemCardId",
                        column: x => x.ItemCardId,
                        principalTable: "ItemCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShelveItems_SHELVES_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "SHELVES",
                        principalColumn: "ShelfId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShelveItems_ItemCardId",
                table: "ShelveItems",
                column: "ItemCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ShelveItems_ShelfId",
                table: "ShelveItems",
                column: "ShelfId");

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropTable(
                name: "ShelveItems");

            

            

           
        }
    }
}
