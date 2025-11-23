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
            { "message", new RenderedMessageColumnWriter() },
            { "message_template", new MessageTemplateColumnWriter() },
            { "exception", new ExceptionColumnWriter() },
            { "properties", new LogEventSerializedColumnWriter() },
            {
                "machine_name",
                new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.Raw)
            }
        };
    }
}