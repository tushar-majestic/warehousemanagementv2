using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class AddDispensedFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ItemCards_ItemCardId",
                table: "DespensedItems",
                column: "ItemCardId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequests_MaterialRequestId",
                table: "DespensedItems",
                column: "MaterialRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_DespensedItems_ItemCards_ItemCardId",
                table: "DespensedItems",
                column: "ItemCardId",
                principalTable: "ItemCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DespensedItems_MaterialRequests_MaterialRequestId",
                table: "DespensedItems",
                column: "MaterialRequestId",
                principalTable: "MaterialRequests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DespensedItems_ItemCards_ItemCardId",
                table: "DespensedItems");

            migrationBuilder.DropForeignKey(
                name: "FK_DespensedItems_MaterialRequests_MaterialRequestId",
                table: "DespensedItems");

            migrationBuilder.DropIndex(
                name: "IX_ItemCards_ItemCardId",
                table: "DespensedItems");

            migrationBuilder.DropIndex(
                name: "IX_MaterialRequests_MaterialRequestId",
                table: "DespensedItems");
        }
    }
}
