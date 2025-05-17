using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class returnrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReturnRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToSector = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FromSector = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Warehouse = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsSurplus = table.Column<bool>(type: "bit", nullable: false),
                    IsExpired = table.Column<bool>(type: "bit", nullable: false),
                    IsInvalid = table.Column<bool>(type: "bit", nullable: false),
                    IsDamaged = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReturnRequestItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnRequestId = table.Column<int>(type: "int", nullable: false),
                    ItemCardId = table.Column<int>(type: "int", nullable: false),
                    ItemGroup = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemNameArabic = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemNameEnglish = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TypeOfContract = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Chemical = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RiskRating = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StateOfMatter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReturnedQuantity = table.Column<int>(type: "int", nullable: false),
                    ReturnNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnRequestItems_ItemCards_ItemCardId",
                        column: x => x.ItemCardId,
                        principalTable: "ItemCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnRequestItems_ReturnRequests_ReturnRequestId",
                        column: x => x.ReturnRequestId,
                        principalTable: "ReturnRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequestItems_ItemCardId",
                table: "ReturnRequestItems",
                column: "ItemCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequestItems_ReturnRequestId",
                table: "ReturnRequestItems",
                column: "ReturnRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReturnRequestItems");

            migrationBuilder.DropTable(
                name: "ReturnRequests");
        }
    }
}
