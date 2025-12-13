namespace TreePassBot2.Core.Options;

public record BotOptions
{
    public const string SectionName = "BotSettings";

    public ulong BotQqId { get; set; }
    public ulong OwnerId { get; set; }
    public ulong AuditGroupId { get; set; }
    public List<ulong> MainGroupIds { get; set; } = [];

    public string PluginDir { get; set; } = "./plugins";
}
