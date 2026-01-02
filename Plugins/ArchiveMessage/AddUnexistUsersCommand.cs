using TreePassBot2.Infrastructure.MakabakaAdaptor.Models;
using TreePassBot2.PluginSdk.Entities.Enums;
using TreePassBot2.PluginSdk.Interfaces;

namespace AuditPlugin;

public class AddUnexistUsersCommand : IBotCommand
{
    /// <inheritdoc />
    public string Trigger { get; } = ".add-notexist";

    /// <inheritdoc />
    public string[] Aliases { get; } = [];

    /// <inheritdoc />
    public CommandScope Scope { get; } = CommandScope.Global;

    /// <inheritdoc />
    public UserRole MinRole { get; } = UserRole.Admin;

    /// <inheritdoc />
    public async Task ExecuteAsync(ICommandContext ctx)
    {
        if (ctx.GroupId != ctx.Options.AuditGroupId)
        {
            return;
        }

        var groupMembers = await ctx.BotApi.GetGroupMemberListAsync(ctx.Options.AuditGroupId).ConfigureAwait(false);

        if (groupMembers is null)
        {
            var failedMsg = new MessageBuilder()
                           .AddAt(ctx.SenderId)
                           .AddText("无法获取到群成员信息");

            await ctx.ReplyAsync(failedMsg).ConfigureAwait(false);
            return;
        }

        var existUsrs = await ctx.BotApi.GetAuditListAsync().ConfigureAwait(false);
        var groupUsers = groupMembers.Select(m => m.QqId);

        var needAdd = groupUsers.Except(existUsrs).ToList();

        foreach (var userId in needAdd)
        {
            await ctx.BotApi.AddAuditRequestAsync(userId).ConfigureAwait(false);
        }

        var successMsg = new MessageBuilder()
                        .AddAt(ctx.SenderId)
                        .AddText($"成功添加{needAdd.Count}个待审核人员");
        await ctx.ReplyAsync(successMsg).ConfigureAwait(false);
    }
}
