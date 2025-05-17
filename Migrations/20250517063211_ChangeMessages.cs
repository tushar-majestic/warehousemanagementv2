using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaterialRequestId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MaterialRequestId",
                table: "Messages",
                column: "MaterialRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_MaterialRequests_MaterialRequestId",
                table: "Messages",
                column: "MaterialRequestId",
                principalTable: "MaterialRequests",
                principalColumn: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_MaterialRequests_MaterialRequestId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MaterialRequestId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MaterialRequestId",
                table: "Messages");
        }
    }
}
