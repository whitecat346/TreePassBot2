using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using TreePassBot2.Core.Entities;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class UserManageService(
    IServiceProvider serviceProvider,
    ICommunicationService communication)
{
    private List<GroupInfo> _groups = [];
    private readonly ConcurrentDictionary<ulong, List<QqUserInfo>> _users = [];

    public IReadOnlyList<GroupInfo> Groups => _groups.AsReadOnly();

    #region Member

    public async Task<IReadOnlyList<QqUserInfo>> GetMemberListFromApiAsync(ulong groupId, bool forceUpdate = false)
    {
        if (_users.TryGetValue(groupId, out var members))
        {
            if (!forceUpdate)
            {
                return members;
            }

            var memberList = await FetchMemberListFromSourceAsync(groupId).ConfigureAwait(false);
            _users.AddOrUpdate(groupId, _ => memberList, (_, _) => memberList);

            return memberList;
        }

        var remoteMembers = await FetchMemberListFromSourceAsync(groupId).ConfigureAwait(false);
        _users.TryAdd(groupId, remoteMembers);

        return remoteMembers;
    }

    public async Task<IReadOnlyList<QqUserInfo>> GetMemberListFromDbAsync(ulong groupId, bool forceUpdate = false)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        if (!forceUpdate)
        {
            var members = await db.Users
                                  .Where(it => it.GroupId == groupId)
                                  .ToListAsync()
                                  .ConfigureAwait(false);
            return members;
        }

        var remoteMembers = await FetchMemberListFromSourceAsync(groupId).ConfigureAwait(false);
        var remoteIds = remoteMembers.Select(it => it.QqId).ToHashSet();

        var existingMembers = await db.Users
                                      .Where(it => remoteIds.Contains(it.QqId) && it.GroupId == groupId)
                                      .Select(user => user.QqId)
                                      .ToListAsync()
                                      .ConfigureAwait(false);

        var newUsers = remoteMembers
                      .Where(it => !existingMembers.Contains(it.QqId))
                      .ToList();

        if (newUsers.Count > 0)
        {
            await db.AddRangeAsync(newUsers).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }

        _users.AddOrUpdate(groupId, _ => remoteMembers, (_, _) => remoteMembers);

        return remoteMembers;
    }

    #endregion

    #region Group

    public async Task<string> GetGroupNameAsync(ulong groupId)
    {
        var groups = await GetGroupListFromApiAsync().ConfigureAwait(false);
        var group = groups.SingleOrDefault(it => it.GroupId == groupId);
        return group?.Name ?? string.Empty;
    }

    public async Task<List<GroupInfo>> GetGroupListFromApiAsync(bool forceUpdate = false)
    {
        if (_groups.Count > 0 && !forceUpdate)
        {
            return _groups;
        }

        if (_groups.Count == 0 || forceUpdate)
        {
            var response = await communication.GetGroupListAsync().ConfigureAwait(false);
            if (response is null)
            {
                return [];
            }

            _groups = response;
        }

        return _groups;
    }

    public async Task<List<GroupInfo>> GetGroupListFromDbAsync(bool forceUpdate = false)
    {
        if (_groups.Count > 0 && !forceUpdate)
        {
            return _groups;
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var groupsInDb = await db.Groups.ToListAsync().ConfigureAwait(false);

        if (groupsInDb.Count == 0 || forceUpdate)
        {
            var response = await communication.GetGroupListAsync().ConfigureAwait(false);
            if (response is null)
            {
                return [];
            }

            var existingGroupIds = groupsInDb.Select(it => it.GroupId).ToHashSet();

            var newGroups = response
                           .Where(it => !existingGroupIds.Contains(it.GroupId))
                           .ToList();

            await db.AddRangeAsync(newGroups).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }

        _groups = groupsInDb;
        return _groups;
    }

    #endregion

    private async Task<List<QqUserInfo>> FetchMemberListFromSourceAsync(ulong groupId)
    {
        var response = await communication.GetGroupMemberListAsync(groupId).ConfigureAwait(false);

        if (response is null)
        {
            return [];
        }

        var users = response.Select(it => new QqUserInfo
        {
            QqId = it.QqId,
            GroupId = groupId,
            UserName = it.UserName,
            NickName = it.NickName,
            Role = it.Role,
            JoinedAt = it.JoinedAt
        }).ToList();

        return users;
    }
}
