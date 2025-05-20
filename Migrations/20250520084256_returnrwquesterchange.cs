using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class returnrwquesterchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromSector",
                table: "ReturnRequests");

            migrationBuilder.AddColumn<int>(
                name: "FromSectorId",
                table: "ReturnRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_FromSectorId",
                table: "ReturnRequests",
                column: "FromSectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_REQUESTER_FromSectorId",
                table: "ReturnRequests",
                column: "FromSectorId",
                principalTable: "REQUESTER",
                principalColumn: "REQ_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_REQUESTER_FromSectorId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_FromSectorId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "FromSectorId",
                table: "ReturnRequests");

            migrationBuilder.AddColumn<string>(
                name: "FromSector",
                table: "ReturnRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
