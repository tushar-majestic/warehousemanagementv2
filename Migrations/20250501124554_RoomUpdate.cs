using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class RoomUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeeperName",
                table: "ROOMS");

            migrationBuilder.RenameColumn(
                name: "KeeperJobNum",
                table: "ROOMS",
                newName: "KeeperID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeeperID",
                table: "ROOMS",
                newName: "KeeperJobNum");

            migrationBuilder.AddColumn<string>(
                name: "KeeperName",
                table: "ROOMS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
