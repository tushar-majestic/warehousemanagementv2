using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class isrepliedtoreports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GeneralSupervisorApprovalDate",
                table: "ReceivingReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "ReceivingReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReplied",
                table: "ReceivingReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TechnicalMemberApprovalDate",
                table: "ReceivingReports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneralSupervisorApprovalDate",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "IsReplied",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "TechnicalMemberApprovalDate",
                table: "ReceivingReports");
        }
    }
}
