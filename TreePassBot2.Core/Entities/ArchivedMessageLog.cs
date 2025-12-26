using System.ComponentModel.DataAnnotations;

namespace TreePassBot2.Core.Entities;

public record ArchivedMessageLog
{
    [Key]
    public long Id { get; set; }

    public long MessageId { get; set; }
    public ulong GroupId { get; set; }
    public string GroupNameSnapshot { get; set; } = string.Empty;
    public ulong UserId { get; set; }
    public string UserNameSnapshot { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset SendAt { get; set; }
    public bool IsRecalled { get; set; }
    public ulong RecalledBy { get; set; }
    public string ArchiveReason { get; set; } = string.Empty;

    public ulong OperatorId { get; set; } = 0;
    public DateTimeOffset ArchivedAt { get; set; } = DateTimeOffset.UtcNow;
}
