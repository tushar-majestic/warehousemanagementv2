using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class receivingreportnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizedBy",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "IssuedBy",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ReceivingReports");

            migrationBuilder.AlterColumn<string>(
                name: "ReceivingWarehouse",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivingDate",
                table: "ReceivingReports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FiscalYear",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BasedOnDocument",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChiefResponsible",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DocumentDate",
                table: "ReceivingReports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientEmployeeId",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecipientSector",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SectorNumber",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SerialNumber",
                table: "ReceivingReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupplierType",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechnicalMember",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "BasedOnDocument",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "ChiefResponsible",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "DocumentDate",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "RecipientEmployeeId",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "RecipientSector",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "SectorNumber",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "SupplierType",
                table: "ReceivingReports");

            migrationBuilder.DropColumn(
                name: "TechnicalMember",
                table: "ReceivingReports");

            migrationBuilder.AlterColumn<string>(
                name: "ReceivingWarehouse",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivingDate",
                table: "ReceivingReports",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "FiscalYear",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AuthorizedBy",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuedBy",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedBy",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ReceivingReports",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
