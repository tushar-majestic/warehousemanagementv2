using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ItemCardUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ceiling",
                table: "ItemCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivity",
                table: "ItemCards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Minimum",
                table: "ItemCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReorderLimit",
                table: "ItemCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeOfAsset",
                table: "ItemCards",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ceiling",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "LastActivity",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "Minimum",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "ReorderLimit",
                table: "ItemCards");

            migrationBuilder.DropColumn(
                name: "TypeOfAsset",
                table: "ItemCards");
        }
    }
}
