using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class ApprovalDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReplied",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "RejectedById",
                table: "ReceivingReports");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeptManagerApprovalDate",
                table: "MaterialRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "KeeperApprovalDate",
                table: "MaterialRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SectorManagerApprovalDate",
                table: "MaterialRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SupervisorApprovalDate",
                table: "MaterialRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeptManagerApprovalDate",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "KeeperApprovalDate",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "SectorManagerApprovalDate",
                table: "MaterialRequests");

            migrationBuilder.DropColumn(
                name: "SupervisorApprovalDate",
                table: "MaterialRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsReplied",
                table: "ReceivingReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RejectedById",
                table: "ReceivingReports",
                type: "int",
                nullable: true);
        }
    }
}
