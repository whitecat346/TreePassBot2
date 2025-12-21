using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreePassBot2.Data.Migrations
{
    /// <inheritdoc />
    public partial class IndexUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_GroupId_UserId_SendAt",
                table: "MessageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ArchivedMessageLogs_UserNickName",
                table: "ArchivedMessageLogs");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "AuditRequests");

            migrationBuilder.RenameColumn(
                name: "UserNickName",
                table: "ArchivedMessageLogs",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "IsWithdrawed",
                table: "ArchivedMessageLogs",
                newName: "IsRecalled");

            migrationBuilder.RenameColumn(
                name: "ContentText",
                table: "ArchivedMessageLogs",
                newName: "Content");

            migrationBuilder.AddColumn<bool>(
                name: "IsJoinedGroup",
                table: "AuditRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RecalledBy",
                table: "ArchivedMessageLogs",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_GroupId_MessageId_SendAt",
                table: "MessageLogs",
                columns: new[] { "GroupId", "MessageId", "SendAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditRequests_Id",
                table: "AuditRequests",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedMessageLogs_GroupId_SendAt_MessageId",
                table: "ArchivedMessageLogs",
                columns: new[] { "GroupId", "SendAt", "MessageId" });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedMessageLogs_MessageId",
                table: "ArchivedMessageLogs",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_GroupId_MessageId_SendAt",
                table: "MessageLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditRequests_Id",
                table: "AuditRequests");

            migrationBuilder.DropIndex(
                name: "IX_ArchivedMessageLogs_GroupId_SendAt_MessageId",
                table: "ArchivedMessageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ArchivedMessageLogs_MessageId",
                table: "ArchivedMessageLogs");

            migrationBuilder.DropColumn(
                name: "IsJoinedGroup",
                table: "AuditRequests");

            migrationBuilder.DropColumn(
                name: "RecalledBy",
                table: "ArchivedMessageLogs");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ArchivedMessageLogs",
                newName: "UserNickName");

            migrationBuilder.RenameColumn(
                name: "IsRecalled",
                table: "ArchivedMessageLogs",
                newName: "IsWithdrawed");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "ArchivedMessageLogs",
                newName: "ContentText");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiresAt",
                table: "AuditRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_GroupId_UserId_SendAt",
                table: "MessageLogs",
                columns: new[] { "GroupId", "UserId", "SendAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedMessageLogs_UserNickName",
                table: "ArchivedMessageLogs",
                column: "UserNickName");
        }
    }
}
