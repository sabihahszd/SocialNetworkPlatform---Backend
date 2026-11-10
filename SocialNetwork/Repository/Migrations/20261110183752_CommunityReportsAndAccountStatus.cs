using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class CommunityReportsAndAccountStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedUntil",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "Report",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CommunityId",
                table: "Report",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportedUserId",
                table: "Report",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Report_CommunityId",
                table: "Report",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportedUserId",
                table: "Report",
                column: "ReportedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Community_CommunityId",
                table: "Report",
                column: "CommunityId",
                principalTable: "Community",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Users_ReportedUserId",
                table: "Report",
                column: "ReportedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_Community_CommunityId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Users_ReportedUserId",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_CommunityId",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportedUserId",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspendedUntil",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportedUserId",
                table: "Report");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "Report",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
