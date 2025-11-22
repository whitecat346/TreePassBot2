using Makabaka;
using Makabaka.Messages;
using Microsoft.Extensions.Logging;
using TreePassBot2.PluginSdk.Entities;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class BotApiImpl(MakabakaApp app, ILogger<BotApiImpl> logger) : IBotApi
{
    /// <inheritdoc />
    public Task SendGroupMessageAsync(ulong groupId, string message)
    {
        logger.LogDebug("Plugin sent group message to {Group}", groupId);
        return app.BotContext.SendGroupMessageAsync(groupId, [new TextSegment(message)]);
    }

    /// <inheritdoc />
    public Task SendProvateMessageAsync(ulong userId, string message)
    {
        logger.LogDebug("Plugin sent provate meessage to: {Private}", userId);
        return app.BotContext.SendPrivateMessageAsync(userId, [new TextSegment(message)]);
    }

    /// <inheritdoc />
    public Task<MemberInfoDto?> GetGroupMemberAsync(ulong groupId, ulong userId)
    {
        throw new NotImplementedException();
    }
}