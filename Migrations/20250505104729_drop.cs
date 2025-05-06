using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class drop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "ItemQuantity",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "ReceivingReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "ReceivingReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemQuantity",
                table: "ReceivingReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPrice",
                table: "ReceivingReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitPrice",
                table: "ReceivingReports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
