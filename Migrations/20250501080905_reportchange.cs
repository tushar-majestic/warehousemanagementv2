using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class reportchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "SupplierType",
                table: "ReceivingReports");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "ReceivingReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingReports_SupplierId",
                table: "ReceivingReports",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingReports_Supplier_SupplierId",
                table: "ReceivingReports",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingReports_Supplier_SupplierId",
                table: "ReceivingReports");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingReports_SupplierId",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "ReceivingReports");

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupplierType",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
