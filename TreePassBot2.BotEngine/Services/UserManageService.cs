using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TreePassBot2.BotEngine.Utils;
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
    #region Init

    public async Task InitInfoAsync()
    {
        var groupList = await GetGroupListAsync(true).ConfigureAwait(false);
        foreach (var group in groupList)
        {
            _ = await GetMemberListAsync(group.GroupId, true).ConfigureAwait(false);
        }
    }

    #endregion

    #region Member

    /// <summary>
    /// 获取群组成员列表
    /// </summary>
    /// <param name="groupId">群组ID</param>
    /// <param name="forceUpdate">是否强制从服务器更新</param>
    /// <returns>群组成员列表</returns>
    public Task<IReadOnlyList<QqUserInfo>> GetMemberListAsync(ulong groupId, bool forceUpdate = false)
    {
        var cacheKey = $"GetMemberList_{groupId}";
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

    public async Task<QqUserInfo> GetMemberInfoAsync(ulong groupId, ulong userId, bool forceUpdate = false)
    {
        var cacheKey = $"GetMemberInfo_{groupId}_{userId}";
        var cachedInfo = await cache.GetOrAddAsync(cacheKey, async () =>
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

            var userInDb = await db.Users.SingleOrDefaultAsync(u => u.GroupId == groupId && u.QqId == userId)
                                   .ConfigureAwait(false);

            if (userInDb is null)
            {
                var userInfo = await FetchMemberInfoAsync(groupId, userId).ConfigureAwait(false);
                await db.Users.AddAsync(userInfo).ConfigureAwait(false);

                return userInfo;
            }

            if (forceUpdate)
            {
                var userInfo = await FetchMemberInfoAsync(groupId, userId).ConfigureAwait(false);
                db.Update(userInfo);

                return userInfo;
            }

            return userInDb;
        }).ConfigureAwait(false);

        return cachedInfo;
    }

    private async Task<QqUserInfo> FetchMemberInfoAsync(ulong groupId, ulong userId)
    {
        var userInfo = await communication.GetGroupMemberInfoAsync(groupId, userId).ConfigureAwait(false);
        if (userInfo is null)
        {
            throw new InvalidDataException("User info cannot be null.");
        }

        return MemberToUserConverter.ConverToQqUserInfo(userInfo, groupId);
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

        var users = response.Select(m => MemberToUserConverter.ConverToQqUserInfo(m, groupId)).ToList();

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

            var fetchedGroups = await FetchGroupListAsync().ConfigureAwait(false);

            if (fetchedGroups.Count == 0)
            {
                return [];
            }

            await UpdateDbGroupListAsync(fetchedGroups).ConfigureAwait(false);
            return fetchedGroups;
        }).ConfigureAwait(false);

        return cached;
    }

    public Task<GroupInfo> GetGroupInfoAsync(ulong groupId, bool forceUpdate = false)
    {
        var cached = cache.GetOrAddAsync($"GetGroupInfo_{groupId}", async () =>
        {
            var cachedGroups = await GetGroupListAsync().ConfigureAwait(false);
            var groupInfoInDb = cachedGroups.SingleOrDefault(g => g.GroupId == groupId);

            await using var scope = serviceProvider.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();

            if (groupInfoInDb is null)
            {
                var groupInfo = await FetchGroupInfoAsync(groupId).ConfigureAwait(false);

                if (groupInfo is null)
                {
                    throw new InvalidDataException("Group info cannot be null");
                }

                await db.Groups.AddAsync(groupInfo).ConfigureAwait(false);

                return groupInfo;
            }

            if (forceUpdate)
            {
                var groupInfo = await FetchGroupInfoAsync(groupId).ConfigureAwait(false);

                if (groupInfo is null)
                {
                    throw new InvalidDataException("Group info cannot be null");
                }

                db.Update(groupInfo);

                return groupInfo;
            }

            return groupInfoInDb;
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

    private async Task<IReadOnlyList<GroupInfo>> FetchGroupListAsync()
    {
        var response = await communication.GetGroupListAsync().ConfigureAwait(false);
        return response ?? [];
    }

    private async Task<GroupInfo?> FetchGroupInfoAsync(ulong groupId)
    {
        var response = await communication.GetGroupInfoAsync(groupId).ConfigureAwait(false);
        return response;
    }

    #endregion
}
