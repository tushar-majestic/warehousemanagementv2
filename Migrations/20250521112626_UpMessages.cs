using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class UpMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ReturnRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnRequestId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReturnRequestId",
                table: "Messages",
                column: "ReturnRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ReturnRequests_ReturnRequestId",
                table: "Messages",
                column: "ReturnRequestId",
                principalTable: "ReturnRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ReturnRequests_ReturnRequestId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReturnRequestId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ReturnRequestId",
                table: "Messages");
        }
    }
}
