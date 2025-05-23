using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ModifyMaterialRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AlterColumn<int>(
            //     name: "RecommendedAction",
            //     table: "ReturnRequestItems",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 0,
            //     oldClrType: typeof(string),
            //     oldType: "nvarchar(max)",
            //     oldNullable: true);

            // migrationBuilder.AddColumn<string>(
            //     name: "Notes",
            //     table: "ReturnRequestItems",
            //     type: "text",
            //     nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SupervisorId",
                table: "MaterialRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "KeeperId",
                table: "MaterialRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DeptManagerId",
                table: "MaterialRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // migrationBuilder.CreateTable(
            //     name: "DocumentTypes",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         DocType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_DocumentTypes", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "StoreTypes",
            //     columns: table => new
            //     {
            //         StoreTypeId = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         StoreType = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_StoreTypes", x => x.StoreTypeId);
            //     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "DocumentTypes");

            // migrationBuilder.DropTable(
            //     name: "StoreTypes");

            // migrationBuilder.DropColumn(
            //     name: "Notes",
            //     table: "ReturnRequestItems");

            // migrationBuilder.AlterColumn<string>(
            //     name: "RecommendedAction",
            //     table: "ReturnRequestItems",
            //     type: "nvarchar(max)",
            //     nullable: true,
            //     oldClrType: typeof(int),
            //     oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SupervisorId",
                table: "MaterialRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "KeeperId",
                table: "MaterialRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeptManagerId",
                table: "MaterialRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
