using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class RoomColsToStoreDataResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "StoreDataResult",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfShelves",
                table: "StoreDataResult",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomDesc",
                table: "StoreDataResult",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomStatus",
                table: "StoreDataResult",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "StoreDataResult");

            migrationBuilder.DropColumn(
                name: "NoOfShelves",
                table: "StoreDataResult");

            migrationBuilder.DropColumn(
                name: "RoomDesc",
                table: "StoreDataResult");

            migrationBuilder.DropColumn(
                name: "RoomStatus",
                table: "StoreDataResult");

           
        }
    }
}
