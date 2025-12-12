using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface ICommandContext
{
    string SenderNickName { get; }
    ulong SenderId { get; }
    ulong GroupId { get; }

    long MessageId { get; }
    Message RawMessage { get; }
    string[] Args { get; }
    IPluginStateStorage State { get; }
    IBotApi BotApi { get; }
    Task ReplyAsync(Message msg);

    Task ReplyAsync(MessageBuilder msgBuilder);
}
