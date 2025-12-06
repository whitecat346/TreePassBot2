using System.ComponentModel.DataAnnotations;

namespace TreePassBot2.Core.Entities;

/// <summary>
/// A QQ user entity.
/// </summary>
public record QqUser
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// User qq id.
    /// </summary>
    public ulong QqId { get; set; }

    /// <summary>
    /// User name displaied in QQ or MessageTracker.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The role of the user.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Member;

    /// <summary>
    /// The last time the user was seen.
    /// </summary>
    public DateTimeOffset LastSeenAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The time when the user was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}