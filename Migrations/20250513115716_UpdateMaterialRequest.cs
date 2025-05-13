using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaterialRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MaterialRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedDate",
                table: "MaterialRequests",
                newName: "OrderDate");

            migrationBuilder.RenameColumn(
                name: "MaterialName",
                table: "MaterialRequests",
                newName: "SerialNumber");

            migrationBuilder.AddColumn<bool>(
                name: "DepartmentManagerApproval",
                table: "MaterialRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "MaterialRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "KeeperApproval",
                table: "MaterialRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RequestDocumentType",
                table: "MaterialRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RequestingSector",
                table: "MaterialRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "MaterialRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "MaterialRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentManagerApproval",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "KeeperApproval",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "RequestDocumentType",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "RequestingSector",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "MaterialRequests");

            migrationBuilder.RenameColumn(
                name: "SerialNumber",
                table: "MaterialRequests",
                newName: "MaterialName");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "MaterialRequests",
                newName: "RequestedDate");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MaterialRequests",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
