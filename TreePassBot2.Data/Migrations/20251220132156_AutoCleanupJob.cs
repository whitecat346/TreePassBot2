using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreePassBot2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AutoCleanupJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_MessageId",
                table: "MessageLogs");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_MessageId",
                table: "MessageLogs",
                column: "MessageId",
                unique: true);

            migrationBuilder.Sql("""
                CREATE EXTENSION IF NOT EXISTS pg_cron;
            """);

            migrationBuilder.Sql("""
                SELECT cron.schedule(
                    'Cleanup_Old_Messages', 
                    '0 3 * * *', 
                    $$DELETE FROM "MessageLogs" WHERE "SendAt" < now() - INTERVAL '7 days'$$
                );
            """);

            // 添加 VACUUM 任务以释放磁盘空间
            migrationBuilder.Sql("""
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
            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_MessageId",
                table: "MessageLogs");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_MessageId",
                table: "MessageLogs",
                column: "MessageId");

            migrationBuilder.Sql("SELECT cron.unschedule('Cleanup_Old_Messages');");
            migrationBuilder.Sql("SELECT cron.unschedule('Vacuum_MessageLogs');");
        }
    }
}
