using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class StoreDataResultKeeper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KeeperJobNum",
                table: "StoreDataResult",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeeperName",
                table: "StoreDataResult",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeeperJobNum",
                table: "StoreDataResult");

            migrationBuilder.DropColumn(
                name: "KeeperName",
                table: "StoreDataResult");
        }
    }
}
