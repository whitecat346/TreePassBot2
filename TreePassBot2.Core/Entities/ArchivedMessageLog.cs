using System.ComponentModel.DataAnnotations;

namespace TreePassBot2.Core.Entities;

public record ArchivedMessageLog
{
    [Key]
    public long Id { get; set; }

    public long MessageId { get; set; }
    public ulong GroupId { get; set; }
    public ulong UserId { get; set; }
    public string? UserNickName { get; set; }
    public string ContentText { get; set; } = string.Empty;
    public DateTimeOffset SendAt { get; set; }
    public bool IsWithdrawed { get; set; }
    public string ArchiveReason { get; set; } = string.Empty;
    public ulong? OperatorId { get; set; }
    public DateTimeOffset ArchivedAt { get; set; } = DateTimeOffset.UtcNow;
}
