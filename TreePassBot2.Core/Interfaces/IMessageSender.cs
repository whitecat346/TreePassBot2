namespace TreePassBot2.Core.Interfaces;

/// <summary>
/// Interface for sending messages.
/// </summary>
public interface IMessageSender
{
    /// <summary>
    /// Sends a message to a group.
    /// </summary>
    /// <param name="groupId">Target group id.</param>
    /// <param name="message">Message content.</param>
    /// <returns></returns>
    Task SendGroupMessageAsync(ulong groupId, string message);

    /// <summary>
    /// Send a private to a user.
    /// </summary>
    /// <param name="userId">Target user id.</param>
    /// <param name="message">Message content.</param>
    /// <returns></returns>
    Task SendPrivateMessageAsync(ulong userId, string message);
}