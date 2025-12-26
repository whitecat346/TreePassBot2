using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TreePassBot2.Core.Entities;
using TreePassBot2.Data;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

// ReSharper disable FlagArgument

namespace TreePassBot2.BotEngine.Services;

public class UserManageService(
    IServiceProvider serviceProvider,
    ICommunicationService communication,
    IAppCache cache)
{
    #region Member

    /// <summary>
    /// 获取群组成员列表
    /// </summary>
    /// <param name="groupId">群组ID</param>
    /// <param name="forceUpdate">是否强制从服务器更新</param>
    /// <returns>群组成员列表</returns>
    public Task<IReadOnlyList<QqUserInfo>> GetMemberListAsync(ulong groupId, bool forceUpdate = false)
    {
        var cacheKey = $"MemberList_{groupId}";
        var cachedList = cache.GetOrAddAsync(cacheKey, async () =>
        {
            var usersInDb = await GetMemberListFromDbAsync(groupId).ConfigureAwait(false);

            if (!forceUpdate)
            {
                return usersInDb;
            }

            var fetchedUsers = await FetchMemberListAsync(groupId).ConfigureAwait(false);
            await UpdateDbMemberListAsync(groupId, fetchedUsers).ConfigureAwait(false);

            return fetchedUsers;
        });

        return cachedList;
    }

    private async Task<IReadOnlyList<QqUserInfo>> GetMemberListFromDbAsync(ulong groupId)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var users = await db.Users
                            .Where(u => u.GroupId == groupId)
                            .ToListAsync().ConfigureAwait(false);

        return users;
    }

    private async Task UpdateDbMemberListAsync(ulong groupId, IReadOnlyList<QqUserInfo> users)
    {
        if (users == null)
        {
            throw new ArgumentNullException(nameof(users), "Member list cannot be null");
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var existingUsers = await db.Users
                                    .Where(u => u.GroupId == groupId)
                                    .ToListAsync().ConfigureAwait(false);

        var existingUserIds = existingUsers.Select(u => u.QqId).ToHashSet();
        var newUserIds = users.Select(u => u.QqId).ToHashSet();

        var usersToRemove = existingUsers.Where(u => !newUserIds.Contains(u.QqId)).ToList();

        var usersToAdd = users.Where(u => !existingUserIds.Contains(u.QqId)).ToList();

        if (usersToRemove.Count > 0 || usersToAdd.Count > 0)
        {
            if (usersToRemove.Count > 0)
            {
                db.Users.RemoveRange(usersToRemove);
            }

            if (usersToAdd.Count > 0)
            {
                await db.Users.AddRangeAsync(usersToAdd).ConfigureAwait(false);
            }

            await db.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    private async Task<IReadOnlyList<QqUserInfo>> FetchMemberListAsync(ulong groupId)
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

    #endregion

    #region Group

    public async Task<IReadOnlyList<GroupInfo>> GetGroupListAsync(bool forceUpdate = false)
    {
        var cached = await cache.GetOrAddAsync("GetGroupList", async () =>
        {
            var groupsInDb = await GetGroupListFromDbAsync().ConfigureAwait(false);
            if (!forceUpdate)
            {
                return groupsInDb;
            }

            var fetchedGroups = await FetchGroupInfoAsync(0).ConfigureAwait(false);
            await UpdateDbGroupListAsync(fetchedGroups).ConfigureAwait(false);
            return fetchedGroups;
        }).ConfigureAwait(false);

        return cached;
    }

    private Task<GroupInfo> GetGroupInfoAsync(ulong groupId)
    {
        var cached = cache.GetOrAddAsync($"GetGroupInfo_{groupId}", async () =>
        {
            var cachedGroups = await GetGroupListAsync().ConfigureAwait(false);
            var groupInfo = cachedGroups.Single(g => g.GroupId == groupId);

            return groupInfo;
        });


        return cached;
    }

    private async Task<IReadOnlyList<GroupInfo>> GetGroupListFromDbAsync()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var groups = await db.Groups.ToListAsync().ConfigureAwait(false);

        return groups;
    }

    private async Task UpdateDbGroupListAsync(IReadOnlyList<GroupInfo> groups)
    {
        if (groups == null)
        {
            throw new ArgumentNullException(nameof(groups), "Group list cannot be null");
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var existingGroups = await db.Groups.ToListAsync().ConfigureAwait(false);
        var existingGroupIds = existingGroups.Select(g => g.GroupId).ToHashSet();

        var newGroupIds = groups.Select(g => g.GroupId).ToHashSet();

        var groupsToRemove = existingGroups.Where(g => !newGroupIds.Contains(g.GroupId)).ToList();

        var groupsToAdd = groups.Where(g => !existingGroupIds.Contains(g.GroupId)).ToList();

        if (groupsToRemove.Count > 0 || groupsToAdd.Count > 0)
        {
            if (groupsToRemove.Count > 0)
            {
                db.Groups.RemoveRange(groupsToRemove);
            }

            if (groupsToAdd.Count > 0)
            {
                await db.Groups.AddRangeAsync(groupsToAdd).ConfigureAwait(false);
            }

            await db.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    private async Task<IReadOnlyList<GroupInfo>> FetchGroupInfoAsync(ulong groupId)
    {
        var response = await communication.GetGroupListAsync().ConfigureAwait(false);
        return response ?? [];
    }

    #endregion
}
