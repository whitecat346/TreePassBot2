using System.ComponentModel.DataAnnotations;
using TreePassBot2.Core.Entities.Enums;

namespace TreePassBot2.Core.Entities;

public record AuditRequestData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required ulong UserId { get; set; }
    public required ulong GroupId { get; set; }
    public required string GroupName { get; set; } = string.Empty;

    [MaxLength(10)]
    public string VerificationCode { get; set; } = string.Empty;

    public AuditStatus Status { get; set; } = AuditStatus.Pending;

    public bool IsJoinedGroup { get; set; } = false;
    public string? RejectReason { get; set; }

    public ulong ProcessedBy { get; set; }
    public DateTimeOffset ProcessedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
