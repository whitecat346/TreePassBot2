using Makabaka;
using Makabaka.Messages;
using TreePassBot2.Core.Interfaces;

namespace TreePassBot2.Infrastructure.Services;

public class MakabakaSender(MakabakaApp client) : IMessageSender
{
    /// <inheritdoc />
    public Task SendGroupMessageAsync(ulong groupId, string message)
    {
        // TODO: impl more complex message types
        return client.BotContext.SendGroupMessageAsync(groupId, [new TextSegment(message)]);
    }

    /// <inheritdoc />
    public Task SendPrivateMessageAsync(ulong userId, string message)
    {
        // TODO: impl more complex message types
        return client.BotContext.SendPrivateMessageAsync(userId, [new TextSegment(message)]);
    }
}