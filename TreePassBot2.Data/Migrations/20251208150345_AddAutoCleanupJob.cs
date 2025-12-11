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
            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_GroupId_UserId",
                table: "MessageLogs");

            migrationBuilder.DropColumn(
                name: "RawMessageJson",
                table: "MessageLogs");

            migrationBuilder.RenameColumn(
                name: "Scoop",
                table: "PluginStates",
                newName: "Scope");

            migrationBuilder.RenameColumn(
                name: "IsFlagged",
                table: "MessageLogs",
                newName: "IsWithdrawed");

            migrationBuilder.AlterColumn<long>(
                name: "MessageId",
                table: "MessageLogs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "UserNickName",
                table: "MessageLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WithdrawedBy",
                table: "MessageLogs",
                type: "numeric(20,0)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_GroupId_UserId_SendAt",
                table: "MessageLogs",
                columns: new[] { "GroupId", "UserId", "SendAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_MessageId",
                table: "MessageLogs",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_UserNickName",
                table: "MessageLogs",
                column: "UserNickName",
                filter: "UserNickName IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedMessageLogs_GroupId_UserId",
                table: "ArchivedMessageLogs",
                columns: new[] { "GroupId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedMessageLogs_UserNickName",
                table: "ArchivedMessageLogs",
                column: "UserNickName",
                filter: "UserNickName IS NOT NULL");

            // load pg_cron extension
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_cron;");

            // schedule cleanup job for MessageLogs table
            // execute at 3:00 AM daily, delete records older than 7 days
            migrationBuilder.Sql("""
                SELECT cron.unschedule('Cleanup_Old_Messages');

                SELECT cron.schedule(
                    'Cleanup_Old_Messages',
                    '0 3 * * *',
                    $$DELETE FROM "MessageLogs" WHERE "SendAt" < now() - INTERVAL '7 days'$$
                );
            """);

            // schedule VACUUM job for MessageLogs table
            migrationBuilder.Sql("""
                SELECT cron.unschedule('Vacuum_MessageLogs');

                SELECT cron.schedule(
                    'Vacuum_MessageLogs',
                    '30 3 * * *',
                    $$VACUUM "MessageLogs"$$
                );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedMessageLogs");

            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_GroupId_UserId_SendAt",
                table: "MessageLogs");

            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_MessageId",
                table: "MessageLogs");

            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_UserNickName",
                table: "MessageLogs");

            migrationBuilder.DropColumn(
                name: "UserNickName",
                table: "MessageLogs");

            migrationBuilder.DropColumn(
                name: "WithdrawedBy",
                table: "MessageLogs");

            migrationBuilder.RenameColumn(
                name: "Scope",
                table: "PluginStates",
                newName: "Scoop");

            migrationBuilder.RenameColumn(
                name: "IsWithdrawed",
                table: "MessageLogs",
                newName: "IsFlagged");

            migrationBuilder.AlterColumn<int>(
                name: "MessageId",
                table: "MessageLogs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "RawMessageJson",
                table: "MessageLogs",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_GroupId_UserId",
                table: "MessageLogs",
                columns: new[] { "GroupId", "UserId" });

            // remove loaded extension jobs
            migrationBuilder.Sql("SELECT cron.unschedule('Cleanup_Old_Messages');");
            migrationBuilder.Sql("SELECT cron.unschedule('Vacuum_MessageLogs');");
        }
    }
}
