using System.ComponentModel.DataAnnotations;
using TreePassBot2.Core.Entities.Enums;

namespace TreePassBot2.Core.Entities;

public record AuditRequestData
{
    public Guid Id { get; set; }
    public ulong UserId { get; set; }
    public ulong GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;

    [MaxLength(10)]
    public required string VerificationCode { get; set; }

    public AuditStatus Status { get; set; } = AuditStatus.Pending;
    public string? RejectReason { get; set; }

    public ulong ProcessedBy { get; set; }
    public DateTimeOffset ProcessedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; set; }
}
