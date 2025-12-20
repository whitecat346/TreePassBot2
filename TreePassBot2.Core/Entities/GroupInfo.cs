using System.ComponentModel.DataAnnotations;

namespace TreePassBot2.Core.Entities;

public record GroupInfo
{
    [Key]
    public ulong GroupId { get; set; }

    public ulong OwnerId { get; set; }
    public required string Name { get; set; }
    public int MemberCount { get; set; }
}
