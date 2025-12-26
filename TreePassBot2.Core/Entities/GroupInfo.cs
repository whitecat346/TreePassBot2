using System.ComponentModel.DataAnnotations;

namespace TreePassBot2.Core.Entities;

public record GroupInfo
{
    [Key]
    public ulong GroupId { get; set; }

    public required string Name { get; set; }
    public int MemberCount { get; set; }

    public virtual ICollection<QqUserInfo> Members { get; set; } = [];
    public virtual ICollection<MessageLog> Messages { get; set; } = [];
    public virtual ICollection<AuditRequestData> AuditRequests { get; set; } = [];
}
