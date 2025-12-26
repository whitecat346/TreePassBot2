using System.ComponentModel.DataAnnotations;

namespace TreePassBot2.Core.Entities;

public record MessageLog
{
    [Key]
    public long Id { get; set; }

    public long MessageId { get; set; }

    public ulong GroupId { get; set; }
    public virtual GroupInfo Group { get; set; } = null!;
    public ulong UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset SendAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsRecalled { get; set; } = false;
    public ulong RecalledBy { get; set; } = 0;
    public DateTimeOffset? RecalledAt { get; set; }
}
