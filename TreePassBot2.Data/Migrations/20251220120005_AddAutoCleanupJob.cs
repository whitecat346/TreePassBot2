using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TreePassBot2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoCleanupJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivedMessageLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserNickName = table.Column<string>(type: "text", nullable: true),
                    ContentText = table.Column<string>(type: "text", nullable: false),
                    SendAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsWithdrawed = table.Column<bool>(type: "boolean", nullable: false),
                    ArchiveReason = table.Column<string>(type: "text", nullable: false),
                    OperatorId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedMessageLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GroupId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: false),
                    VerificationCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RejectReason = table.Column<string>(type: "text", nullable: true),
                    ProcessedBy = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    OwnerId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MemberCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "MessageLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SendAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRecalled = table.Column<bool>(type: "boolean", nullable: false),
                    RecalledBy = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    RecalledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PluginStates",
                columns: table => new
                {
                    PluginId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    StateDataJson = table.Column<string>(type: "jsonb", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginStates", x => new { x.PluginId, x.Scope, x.GroupId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    QqId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GroupId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NickName = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.QqId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedMessageLogs_GroupId_UserId",
                table: "ArchivedMessageLogs",
                columns: new[] { "GroupId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedMessageLogs_UserNickName",
                table: "ArchivedMessageLogs",
                column: "UserNickName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditRequests_UserId_Status",
                table: "AuditRequests",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditRequests_VerificationCode",
                table: "AuditRequests",
                column: "VerificationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupId",
                table: "Groups",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_GroupId_UserId_SendAt",
                table: "MessageLogs",
                columns: new[] { "GroupId", "UserId", "SendAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_MessageId",
                table: "MessageLogs",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_SendAt",
                table: "MessageLogs",
                column: "SendAt");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_UserName",
                table: "MessageLogs",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_QqId",
                table: "Users",
                column: "QqId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedMessageLogs");

            migrationBuilder.DropTable(
                name: "AuditRequests");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "MessageLogs");

            migrationBuilder.DropTable(
                name: "PluginStates");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
