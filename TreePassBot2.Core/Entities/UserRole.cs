namespace TreePassBot2.Core.Entities;

/// <summary>
/// User role in a group.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Normal user.
    /// </summary>
    User = 0,

    /// <summary>
    /// User with auditing permissions.
    /// </summary>
    Auditor = 1,

    /// <summary>
    /// Administrator user.
    /// </summary>
    Admin = 2,

    /// <summary>
    /// Owner of a group.
    /// </summary>
    Owner = 999
}