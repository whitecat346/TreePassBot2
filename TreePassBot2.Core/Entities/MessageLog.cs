using System.ComponentModel.DataAnnotations.Schema;

namespace TreePassBot2.Core.Entities;

public record MessageLog
{
    public long Id { get; set; }

    public long MessageId { get; set; }

    public ulong GroupId { get; set; }
    public ulong UserId { get; set; }
    public string? UserNickName { get; set; }
    public string ContentText { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public string RawMessageJson { get; set; } = "{}";

    public DateTimeOffset SendAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsFlagged { get; set; } = false;
}