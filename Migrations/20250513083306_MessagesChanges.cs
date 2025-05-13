using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class MessagesChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recipient",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Sender",
                table: "Messages",
                newName: "ReportType");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "ReceivingReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RecipientId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "Messages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ReportType",
                table: "Messages",
                newName: "Sender");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Recipient",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
