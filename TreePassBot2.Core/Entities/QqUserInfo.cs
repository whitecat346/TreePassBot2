using System.ComponentModel.DataAnnotations;
using TreePassBot2.Core.Entities.Enums;

namespace TreePassBot2.Core.Entities;

/// <summary>
/// A QQ user entity.
/// </summary>
public record QqUserInfo
{
    /// <summary>
    /// User qq id.
    /// </summary>
    [Key]
    public ulong QqId { get; set; }

    /// <summary>
    /// Group id that the user belongs to.
    /// </summary>
    public ulong GroupId { get; set; }

    /// <summary>
    /// User name displaied in QQ or MessageTracker.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Nick name of the user. (The original name)
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// The role of the user.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Member;

    /// <summary>
    /// The time the user joined group.
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }
}
