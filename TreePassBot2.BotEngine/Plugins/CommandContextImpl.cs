using Microsoft.Extensions.Logging;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.PluginSdk.Interfaces;

// because properties are initialized externally
#pragma warning disable CS8618

namespace TreePassBot2.BotEngine.Plugins;

public class CommandContextImpl(ICommunicationService communicationService) : ICommandContext
{
    private ICommunicationService CommunicationService { get; } = communicationService;

    /// <inheritdoc />
    public string SenderName { get; init; }

    /// <inheritdoc />
    public ulong SenderId { get; init; }

    /// <inheritdoc />
    public ulong GroupId { get; init; }

    /// <inheritdoc />
    public long MessageId { get; init; }

    /// <inheritdoc />
    public Infrastructure.MakabakaAdaptor.Models.MessageSegments.Message RawMessage { get; init; }

    /// <inheritdoc />
    public required string[] Args { get; init; }

    /// <inheritdoc />
    public required IPluginStateStorage State { get; init; }

    /// <inheritdoc />
    public required IBotApi BotApi { get; init; }

    public required ILogger<ICommandContext> Logger { get; set; }

    /// <inheritdoc />
    public Task ReplyAsync(Infrastructure.MakabakaAdaptor.Models.MessageSegments.Message msg)
    {
        return CommunicationService.SendGroupMessageAsync(GroupId, msg);
    }

    /// <inheritdoc />
    public Task ReplyAsync(MessageBuilder msgBuilder)
    {
        var msg = msgBuilder.Build();
        return ReplyAsync(msg);
    }
}
