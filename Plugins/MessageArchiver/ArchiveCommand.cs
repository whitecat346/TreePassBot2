using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.PluginSdk.Entities.Enums;
using TreePassBot2.PluginSdk.Interfaces;

namespace MessageArchiver;

public class ArchiveCommand : IBotCommand
{
    /// <inheritdoc />
    public string Trigger { get; } = ".archive";

    /// <inheritdoc />
    public string[] Aliases { get; } = [];

    /// <inheritdoc />
    public CommandScope Scope { get; } = CommandScope.Global;

    /// <inheritdoc />
    public UserRole MinRole { get; } = UserRole.Admin;

    /// <inheritdoc />
    public async Task ExecuteAsync(ICommandContext ctx)
    {
        if (ctx.ReferMessage == 0)
        {
            var errorMsg = "你没有指定要存档的消息ID。用法: <引用消息> <at> .archive <存档原因>";
            var msgB = new MessageBuilder()
                      .AddAt(ctx.SenderId)
                      .AddText(errorMsg)
                      .Build();
            await ctx.ReplyAsync(msgB).ConfigureAwait(false);
            return;
        }

        var msgId = ctx.ReferMessage;
        await ctx.BotApi
                 .ArchiveMessageAsync(msgId, string.Join(' ', ctx.Args), TimeSpan.FromHours(1))
                 .ConfigureAwait(false);
    }
}
