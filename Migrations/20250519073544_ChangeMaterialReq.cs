using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMaterialReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectorManagerApproval",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "SectorManagerId",
                table: "MaterialRequests");

            migrationBuilder.RenameColumn(
                name: "SectorManagerApprovalDate",
                table: "MaterialRequests",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "MaterialRequests",
                newName: "SectorManagerApprovalDate");

            migrationBuilder.AddColumn<bool>(
                name: "SectorManagerApproval",
                table: "MaterialRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SectorManagerId",
                table: "MaterialRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
