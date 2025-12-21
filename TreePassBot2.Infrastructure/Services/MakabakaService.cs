using Makabaka;
using Makabaka.Exceptions;
using Makabaka.Models;
using Microsoft.Extensions.Logging;
using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Infrastructure.Exceptions;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Converters;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;
using GroupInfo = TreePassBot2.Core.Entities.GroupInfo;
using MessageBuilder = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageBuilder;

namespace TreePassBot2.Infrastructure.Services;

/// <summary>
/// Makabaka通信服务实现
/// </summary>
public class MakabakaService : ICommunicationService, IAsyncDisposable
{
    private readonly ILogger<MakabakaService> _logger;
    private readonly MakabakaApp _makabakaApp;
    public IBotContext BotContext => _makabakaApp.BotContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MakabakaService(ILogger<MakabakaService> logger)
    {
        _logger = logger;
        var makaAppBuilder = new MakabakaAppBuilder();
        var makaApp = makaAppBuilder.Build();

        _makabakaApp = makaApp;
    }

    /// <summary>
    /// 发送群消息
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="message">消息对象</param>
    /// <returns>任务</returns>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/></exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public async Task SendGroupMessageAsync(ulong groupId, Message message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var msg = message.ToString();
        _logger.LogInformation("Send message: {Message}", msg);

        try
        {
            var makabakaMessage = MessageConverter.ConvertToMakabakaMessage(message);

            var response = await BotContext.SendGroupMessageAsync(groupId, makabakaMessage).ConfigureAwait(false);
            response.EnsureSuccess();
        }
        catch (APIException ex)
        {
            throw new FailedToExcuteApiException($"发送群消息失败: {ex.Message}", ex);
        }
        catch (APITimeoutException ex)
        {
            throw new TimeoutException($"发送群消息超时: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"发送群消息时发生异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="message">消息对象</param>
    /// <returns>任务</returns>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/></exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public async Task SendPrivateMessageAsync(ulong userId, Message message)
    {
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            var makabakaMessage = MessageConverter.ConvertToMakabakaMessage(message);

            var response = await BotContext.SendPrivateMessageAsync(userId, makabakaMessage).ConfigureAwait(false);
            response.EnsureSuccess();
        }
        catch (APIException ex)
        {
            throw new FailedToExcuteApiException($"发送私聊消息失败: {ex.Message}", ex);
        }
        catch (APITimeoutException ex)
        {
            throw new TimeoutException($"发送私聊消息超时: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"发送私聊消息时发生异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 发送群消息（使用消息构建器）
    /// </summary>
    /// <param name="groupId">群号</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    /// <exception cref="ArgumentNullException"><paramref name="messageBuilder"/> is <see langword="null"/></exception>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public Task SendGroupMessageAsync(ulong groupId, MessageBuilder messageBuilder)
    {
        ArgumentNullException.ThrowIfNull(messageBuilder);

        var message = messageBuilder.Build();
        return SendGroupMessageAsync(groupId, message);
    }

    /// <summary>
    /// 发送私聊消息（使用消息构建器）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="messageBuilder">消息构建器</param>
    /// <returns>任务</returns>
    /// <exception cref="ArgumentNullException"><paramref name="messageBuilder"/> is <see langword="null"/></exception>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public Task SendPrivateMessageAsync(ulong userId, MessageBuilder messageBuilder)
    {
        ArgumentNullException.ThrowIfNull(messageBuilder);

        var message = messageBuilder.Build();
        return SendPrivateMessageAsync(userId, message);
    }

    /// <summary>
    /// 获取合并转发消息内容
    /// </summary>
    /// <param name="forwardId">合并转发ID</param>
    /// <returns>合并转发的消息列表</returns>
    /// <exception cref="Exception">Throws if failed to get forward message.</exception>
    /// <exception cref="FailedToExcuteApiException">Throws if failed to execute api.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="forwardId"/> is <see langword="null"/></exception>
    /// <exception cref="TimeoutException">Throws if timed out when execute api.</exception>
    public async Task<List<Message>?> GetForwardMessageAsync(string forwardId)
    {
        if (string.IsNullOrEmpty(forwardId))
            throw new ArgumentNullException(nameof(forwardId));

        try
        {
            var response = await BotContext.GetForwardMessageAsync(forwardId).ConfigureAwait(false);

            response.EnsureSuccess();

            var result = new List<Message>();
            if (response.Data?.Message is { Count: > 0 })
            {
                var message = MessageConverter.ConvertToBotMessage(response.Data.Message);
                result.AddRange(message);
            }

            return result;
        }
        catch (APIException ex)
        {
            throw new FailedToExcuteApiException($"获取合并转发消息失败: {ex.Message}", ex);
        }
        catch (APITimeoutException ex)
        {
            throw new TimeoutException($"获取合并转发消息超时: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"获取合并转发消息时发生异常: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<MemberInfo?> GetGroupMemberInfoAsync(ulong groupId, ulong userId)
    {
        var apiRep = await _makabakaApp.BotContext.GetGroupMemberInfoAsync(groupId, userId).ConfigureAwait(false);
        var data = apiRep.Data;

        if (data is null)
        {
            return null;
        }

        var mamberInfo = ConvertToMyMemberInfo(data);

        return mamberInfo;
    }

    /// <inheritdoc />
    public async Task<List<MemberInfo>?> GetGroupMemberListAsync(ulong groupId)
    {
        var apiRep = await _makabakaApp.BotContext.GetGroupMemberListAsync(groupId).ConfigureAwait(false);
        var data = apiRep.Data;

        var memberInfoList = data?.Select(ConvertToMyMemberInfo).ToList();
        return memberInfoList;
    }

    /// <inheritdoc />
    public Task KickMemberAsync(ulong groupId, ulong userId, bool rejectRequest = false) =>
        _makabakaApp.BotContext.KickGroupMemberAsync(groupId, userId, rejectRequest);

    /// <inheritdoc />
    public Task WithdrawMessageAsync(long messageId) =>
        _makabakaApp.BotContext.RevokeMessageAsync(messageId);

    /// <inheritdoc />
    public async Task<GroupInfo?> GetGroupInfoAsync(ulong groupId)
    {
        var response = await _makabakaApp.BotContext.GetGroupInfoAsync(groupId).ConfigureAwait(false);

        var data = response.Data;
        if (data is null)
        {
            return null;
        }

        // convert
        var groupInfo = new GroupInfo
        {
            GroupId = data.GroupId,
            Name = data.GroupName,
            MemberCount = data.MemberCount,
            OwnerId = 0 // TODO: i will impl this later
        };

        return groupInfo;
    }

    /// <inheritdoc />
    public Task AnnounceApprovedAuditActionAsync(
        ulong targetUserId, ulong targetGroupId, string message, string verificationCode)
    {
        var msg = new MessageBuilder()
                 .AddAt(targetUserId)
                 .AddText($" {message}\n")
                 .AddText($"验证码: {verificationCode}")
                 .Build();

        return SendGroupMessageAsync(targetGroupId, msg);
    }

    /// <inheritdoc />
    public Task AnnounceRejectedAuditActionAsync(ulong targetUserId, ulong targetGroupId, string message)
    {
        var msg = new MessageBuilder()
                 .AddAt(targetUserId)
                 .AddText($" {message}\n")
                 .Build();

        return SendGroupMessageAsync(targetGroupId, msg);
    }

    /// <inheritdoc />
    public Task ConnectAsync()
    {
        return _makabakaApp.StartAsync();
    }

    /// <inheritdoc />
    public Task DisconnectAsync()
    {
        return _makabakaApp.StopAsync();
    }

    /// <inheritdoc />
    public async Task<List<GroupInfo>?> GetGroupListAsync()
    {
        var response = await _makabakaApp.BotContext.GetGroupListAsync().ConfigureAwait(false);
        var data = response.Data;

        // convert
        var res = data?.Select(group => new GroupInfo
        {
            GroupId = group.GroupId,
            Name = group.GroupName,
            MemberCount = group.MemberCount,
            OwnerId = 0 // TODO: i will impl this later
        }).ToList();

        return res;
    }

    private static MemberInfo ConvertToMyMemberInfo(GroupMemberInfo memberInfo) =>
        new(memberInfo.UserId,
            memberInfo.Nickname,
            memberInfo.Nickname,
            ConvertToMyRole(memberInfo.Role),
            memberInfo.JoinTime);

    private static UserRole ConvertToMyRole(GroupRoleType roleType) =>
        roleType switch
        {
            GroupRoleType.Owner => UserRole.Owner,
            GroupRoleType.Admin => UserRole.Admin,
            GroupRoleType.Member => UserRole.Member,
            _ => UserRole.Member,
        };

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await DisconnectAsync().ConfigureAwait(false);

        _makabakaApp.Dispose();
    }
}
