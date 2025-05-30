using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class BatchesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RecommendedAction",
                table: "ReturnRequestItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

          

            migrationBuilder.AddColumn<int>(
                name: "QuantityAvailable",
                table: "ItemCardBatches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityAvailable",
                table: "ItemCardBatches");

            migrationBuilder.AlterColumn<int>(
                name: "RecommendedAction",
                table: "ReturnRequestItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

         
        }
    }
}
