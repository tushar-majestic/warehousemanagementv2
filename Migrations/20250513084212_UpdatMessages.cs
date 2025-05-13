using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class UpdatMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ReceivingReports_ReceivingReportId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReceivingReportId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReceivingReportId",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReportId",
                table: "Messages",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ReceivingReports_ReportId",
                table: "Messages",
                column: "ReportId",
                principalTable: "ReceivingReports",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ReceivingReports_ReportId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReportId",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "ReceivingReportId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceivingReportId",
                table: "Messages",
                column: "ReceivingReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ReceivingReports_ReceivingReportId",
                table: "Messages",
                column: "ReceivingReportId",
                principalTable: "ReceivingReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
