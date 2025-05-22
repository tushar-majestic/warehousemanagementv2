using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabMaterials.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReturnRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DestOffApprovalDate",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestOffId",
                table: "ReturnRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InspOffApprovalDate",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InspOffId",
                table: "ReturnRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "KeeperApprovalDate",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KeeperId",
                table: "ReturnRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ManagerApprovalDate",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "ReturnRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecOffId",
                table: "ReturnRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SupervisorApprovalDate",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "ReturnRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_DestOffId",
                table: "ReturnRequests",
                column: "DestOffId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_InspOffId",
                table: "ReturnRequests",
                column: "InspOffId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_KeeperId",
                table: "ReturnRequests",
                column: "KeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ManagerId",
                table: "ReturnRequests",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_RecOffId",
                table: "ReturnRequests",
                column: "RecOffId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_SupervisorId",
                table: "ReturnRequests",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_DestOffId",
                table: "ReturnRequests",
                column: "DestOffId",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_InspOffId",
                table: "ReturnRequests",
                column: "InspOffId",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_KeeperId",
                table: "ReturnRequests",
                column: "KeeperId",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_ManagerId",
                table: "ReturnRequests",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_RecOffId",
                table: "ReturnRequests",
                column: "RecOffId",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_SupervisorId",
                table: "ReturnRequests",
                column: "SupervisorId",
                principalTable: "Users",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_DestOffId",
                table: "ReturnRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_InspOffId",
                table: "ReturnRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_KeeperId",
                table: "ReturnRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_ManagerId",
                table: "ReturnRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_RecOffId",
                table: "ReturnRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_SupervisorId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_DestOffId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_InspOffId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_KeeperId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_ManagerId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_RecOffId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_SupervisorId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "DestOffApprovalDate",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "DestOffId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "InspOffApprovalDate",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "InspOffId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "KeeperApprovalDate",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "KeeperId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ManagerApprovalDate",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "RecOffId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "SupervisorApprovalDate",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "ReturnRequests");
        }
    }
}
