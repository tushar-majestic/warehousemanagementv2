using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ItemCardExtended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "ItemCards");

            migrationBuilder.RenameColumn(
                name: "QuantityReceived",
                table: "ItemCards",
                newName: "QuantityAvailable");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "ItemCardBatches",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "ItemCardBatches");

            migrationBuilder.RenameColumn(
                name: "QuantityAvailable",
                table: "ItemCards",
                newName: "QuantityReceived");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "ItemCards",
                type: "datetime",
                nullable: true);
        }
    }
}
