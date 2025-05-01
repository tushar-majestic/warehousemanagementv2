using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class roomchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "ROOMS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KeeperJobNum",
                table: "ROOMS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeeperName",
                table: "ROOMS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfShelves",
                table: "ROOMS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomDesc",
                table: "ROOMS",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomStatus",
                table: "ROOMS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "KeeperJobNum",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "KeeperName",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "NoOfShelves",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "RoomDesc",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "RoomStatus",
                table: "ROOMS");
        }
    }
}
