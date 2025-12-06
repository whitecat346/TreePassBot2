using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.PluginSdk.Interfaces;

// because properties are initialized externally
#pragma warning disable CS8618

namespace TreePassBot2.BotEngine.Plugins;

public class CommandContextImpl(ICommunicationService communicationService) : ICommandContext
{
    /// <inheritdoc />
    public string SenderNickName { get; init; }

    /// <inheritdoc />
    public ulong SenderId { get; init; }

    /// <inheritdoc />
    public ulong GroupId { get; init; }

    /// <inheritdoc />
    public long MessageId { get; init; }

    /// <inheritdoc />
    public Infrastructure.MakabakaAdaptor.Models.Message RawMessage { get; init; }

    /// <inheritdoc />
    public string[] Args { get; }

    /// <inheritdoc />
    public Task ReplyAsync(Infrastructure.MakabakaAdaptor.Models.Message msg)
    {
        return communicationService.SendGroupMessageAsync(GroupId, msg);
    }

    /// <inheritdoc />
    public Task ReplyAsync(MessageBuilder msgBuilder)
    {
        var msg = msgBuilder.Build();
        return ReplyAsync(msg);
    }
}
