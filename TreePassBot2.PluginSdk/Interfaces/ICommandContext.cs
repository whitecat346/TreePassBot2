using Microsoft.Extensions.Logging;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface ICommandContext
{
    string SenderName { get; }
    ulong SenderId { get; }
    ulong GroupId { get; }

    long MessageId { get; }
    Message RawMessage { get; }
    long ReferMessage { get; }
    string[] Args { get; }
    IPluginStateStorage State { get; }
    IBotApi BotApi { get; }
    ILogger<ICommandContext> Logger { get; }
    Task ReplyAsync(Message msg);

    Task ReplyAsync(MessageBuilder msgBuilder);
}
