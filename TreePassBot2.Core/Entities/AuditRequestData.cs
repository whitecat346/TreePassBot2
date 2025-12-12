using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreePassBot2.Core.Entities.Enums;

namespace TreePassBot2.Core.Entities;

public record AuditRequestData
{
    public Guid Id { get; set; }
    public ulong RequestQqId { get; set; }
    public ulong TargetGroupId { get; set; }

    [MaxLength(10)]
    public required string Passcode { get; set; }

    public AuditStatus Status { get; set; } = AuditStatus.Pending;

    [Column(TypeName = "jsonb")]
    public string FormDataJson { get; set; } = "{}";

    public string? RejectReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; set; }
}
