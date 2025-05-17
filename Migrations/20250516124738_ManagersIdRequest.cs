using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ManagersIdRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Item_ItemHazardType",
            //     table: "Item");

            migrationBuilder.AddColumn<int>(
                name: "DeptManagerId",
                table: "MaterialRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KeeperId",
                table: "MaterialRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<bool>(
                name: "SupervisorApproval",
                table: "MaterialRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "MaterialRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // migrationBuilder.AlterColumn<string>(
            //     name: "HazardTypeName",
            //     table: "Item",
            //     type: "nvarchar(50)",
            //     maxLength: 50,
            //     nullable: false,
            //     defaultValue: "",
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(50)",
            //     oldMaxLength: 50,
            //     oldNullable: true);

            // migrationBuilder.AddColumn<bool>(
            //     name: "Chemical",
            //     table: "Item",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);

            // migrationBuilder.AddColumn<string>(
            //     name: "ItemNameAr",
            //     table: "Item",
            //     type: "nvarchar(200)",
            //     maxLength: 200,
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<string>(
            //     name: "RiskRating",
            //     table: "Item",
            //     type: "nvarchar(100)",
            //     maxLength: 100,
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddColumn<string>(
            //     name: "StateofMatter",
            //     table: "Item",
            //     type: "nvarchar(200)",
            //     maxLength: 200,
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Item_ItemHazardType",
            //     table: "Item",
            //     column: "HazardTypeName",
            //     principalTable: "HazardType",
            //     principalColumn: "HazardTypeName",
            //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Item_ItemHazardType",
            //     table: "Item");

            migrationBuilder.DropColumn(
                name: "DeptManagerId",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "KeeperId",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "SectorManagerApproval",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "SectorManagerId",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "SupervisorApproval",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "MaterialRequests");

            // migrationBuilder.DropColumn(
            //     name: "Chemical",
            //     table: "Item");

            // migrationBuilder.DropColumn(
            //     name: "ItemNameAr",
            //     table: "Item");

            // migrationBuilder.DropColumn(
            //     name: "RiskRating",
            //     table: "Item");

            // migrationBuilder.DropColumn(
            //     name: "StateofMatter",
            //     table: "Item");

            // migrationBuilder.AlterColumn<string>(
            //     name: "HazardTypeName",
            //     table: "Item",
            //     type: "nvarchar(50)",
            //     maxLength: 50,
            //     nullable: true,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(50)",
            //     oldMaxLength: 50);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Item_ItemHazardType",
            //     table: "Item",
            //     column: "HazardTypeName",
            //     principalTable: "HazardType",
            //     principalColumn: "HazardTypeName");
        }
    }
}
