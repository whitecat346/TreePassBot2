using Microsoft.Extensions.Logging;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.PluginSdk.Entities.Enums;
using TreePassBot2.PluginSdk.Interfaces;

namespace EchoPlugin;

public class EchoCommand : IBotCommand
{
    /// <inheritdoc />
    public string Trigger { get; } = ".echo";

    /// <inheritdoc />
    public string[] Aliases { get; } = [];

    /// <inheritdoc />
    public CommandScope Scope { get; } = CommandScope.Global;

    /// <inheritdoc />
    public UserRole MinRole { get; } = UserRole.User;

    /// <inheritdoc />
    public async Task ExecuteAsync(ICommandContext ctx)
    {
        if (ctx.Args.Length == 0)
        {
            ctx.Logger.LogInformation("Return at zero args");
            return;
        }

        var msg = new MessageBuilder().AddText(string.Join(' ', ctx.Args));

        await ctx.ReplyAsync(msg).ConfigureAwait(false);

        ctx.Logger.LogInformation("Execute command");

        return;
    }
}
