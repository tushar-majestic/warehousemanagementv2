using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class storedataresultcol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagerJobNum",
                table: "StoreDataResult",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreType",
                table: "StoreDataResult",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseManagerName",
                table: "StoreDataResult",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseStatus",
                table: "StoreDataResult",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerJobNum",
                table: "StoreDataResult");

            migrationBuilder.DropColumn(
                name: "StoreType",
                table: "StoreDataResult");

            migrationBuilder.DropColumn(
                name: "WarehouseManagerName",
                table: "StoreDataResult");

            migrationBuilder.DropColumn(
                name: "WarehouseStatus",
                table: "StoreDataResult");
        }
    }
}
