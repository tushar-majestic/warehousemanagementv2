using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ColsToReceivingReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiefResponsible",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "TechnicalMember",
                table: "ReceivingReports");

            migrationBuilder.AlterColumn<int>(
                name: "RecipientEmployeeId",
                table: "ReceivingReports",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ChiefResponsibleId",
                table: "ReceivingReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "TechnicalMemberId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiefResponsibleId",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "ItemQuantity",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "TechnicalMemberId",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "ReceivingReports");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientEmployeeId",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ChiefResponsible",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechnicalMember",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
