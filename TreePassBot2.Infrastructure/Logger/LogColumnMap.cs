using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL;

namespace TreePassBot2.Infrastructure.Logger;

public static class LogColumnMap
{
    public static IDictionary<string, ColumnWriterBase> GetColumnOptions()
    {
        return new Dictionary<string, ColumnWriterBase>
        {
            { "timestamp", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Text) },
            { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            // 核心改进：将上下文属性存为 JSONB，这允许你查询特定用户的操作日志
            { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
            // 记录机器名，方便Docker多实例排查
            {
                "machine_name",
                new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.Raw, NpgsqlDbType.Text)
            }
        };
    }
}