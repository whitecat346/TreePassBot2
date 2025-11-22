using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreePassBot2.Core.Entities;

public record PluginState
{
    [MaxLength(20)]
    public required string PluginId { get; set; }

    public ScopeType Scope { get; set; }
    public ulong? GroupId { get; set; }
    public ulong? UserId { get; set; }

    [Column(TypeName = "jsonb")]
    public string StateDataJson { get; set; } = "{}";

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}