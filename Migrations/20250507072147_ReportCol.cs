using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ReportCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ReceivingReports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "GeneralSupApproval",
                table: "ReceivingReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KeeperApproval",
                table: "ReceivingReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StoreManagerApproval",
                table: "ReceivingReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TechnicalMemberApproval",
                table: "ReceivingReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            
          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemCards");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "GeneralSupApproval",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "KeeperApproval",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "StoreManagerApproval",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "TechnicalMemberApproval",
                table: "ReceivingReports");
        }
    }
}
