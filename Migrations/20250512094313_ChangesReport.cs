using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ChangesReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRejected",
                table: "ReceivingReports",
                newName: "IsRejectedByTechnicalMember");

            migrationBuilder.AddColumn<bool>(
                name: "IsRejectedByGeneralSupervisor",
                table: "ReceivingReports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRejectedByGeneralSupervisor",
                table: "ReceivingReports");

            migrationBuilder.RenameColumn(
                name: "IsRejectedByTechnicalMember",
                table: "ReceivingReports",
                newName: "IsRejected");
        }
    }
}
