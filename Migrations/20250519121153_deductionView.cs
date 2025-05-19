using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class deductionView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PendingDeductions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeductedBy",
                table: "PendingDeductions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "PendingDeductions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DeductionExtended",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemCardId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfmeasure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Chemical = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HazardTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuantityReceived = table.Column<int>(type: "int", nullable: false),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ShelfId = table.Column<int>(type: "int", nullable: false),
                    TypeOfAsset = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductionExtended", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingDeductions_DeductedBy",
                table: "PendingDeductions",
                column: "DeductedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingDeductions_Users_DeductedBy",
                table: "PendingDeductions",
                column: "DeductedBy",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingDeductions_Users_DeductedBy",
                table: "PendingDeductions");

            migrationBuilder.DropTable(
                name: "DeductionExtended");

            migrationBuilder.DropIndex(
                name: "IX_PendingDeductions_DeductedBy",
                table: "PendingDeductions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PendingDeductions");

            migrationBuilder.DropColumn(
                name: "DeductedBy",
                table: "PendingDeductions");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PendingDeductions");
        }
    }
}
