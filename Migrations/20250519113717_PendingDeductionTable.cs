using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class PendingDeductionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendingDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCardId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ShelfId = table.Column<int>(type: "int", nullable: false),
                    ReduceQty = table.Column<int>(type: "int", nullable: false),
                    OutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartyId = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingDeductions_ItemCards_ItemCardId",
                        column: x => x.ItemCardId,
                        principalTable: "ItemCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingDeductions_ROOMS_RoomId",
                        column: x => x.RoomId,
                        principalTable: "ROOMS",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingDeductions_SHELVES_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "SHELVES",
                        principalColumn: "ShelfId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingDeductions_ItemCardId",
                table: "PendingDeductions",
                column: "ItemCardId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingDeductions_RoomId",
                table: "PendingDeductions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingDeductions_ShelfId",
                table: "PendingDeductions",
                column: "ShelfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingDeductions");
        }
    }
}
