using System.ComponentModel.DataAnnotations;

namespace TreePassBot2.Core.Entities;

public record MessageLog
{
    [Key]
    public long Id { get; set; }

    public long MessageId { get; set; }

    public ulong GroupId { get; set; }
    public ulong UserId { get; set; }
    public string? UserNickName { get; set; }
    public string ContentText { get; set; } = string.Empty;

    public DateTimeOffset SendAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsWithdrawed { get; set; } = false;
    public ulong? WithdrawedBy { get; set; }
}