using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ChangeReceivingItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "ReceivingReports");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "ReceivingItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "ReceivingItems");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
