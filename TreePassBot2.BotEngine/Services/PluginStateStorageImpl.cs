using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TreePassBot2.Core.Entities;
using TreePassBot2.Core.Entities.Enums;
using TreePassBot2.Data;
using TreePassBot2.PluginSdk.Entities.Enums;
using TreePassBot2.PluginSdk.Interfaces;

namespace TreePassBot2.BotEngine.Services;

public class PluginStateStorageImpl(
    string pluginId,
    IServiceProvider scopeFactory) : IPluginStateStorage
{
    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, StorageScope scope, ulong? groupId = null, ulong? userId = null)
    {
        await using var serviceScope = scopeFactory.CreateAsyncScope();
        var db = serviceScope.ServiceProvider.GetRequiredService<BotDbContext>();

        var entityScope = MapToEntityScope(scope);

        var entity = await db.PluginStates
                             .AsNoTracking()
                             .FirstOrDefaultAsync(p =>
                                                      p.PluginId == pluginId &&
                                                      p.Scope == entityScope &&
                                                      p.GroupId == groupId &&
                                                      p.UserId == userId)
                             .ConfigureAwait(false);

        if (entity == null)
        {
            return default;
        }

        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(entity.StateDataJson);

        if (dict != null && dict.TryGetValue(key, out var jEle))
        {
            return jEle.Deserialize<T>();
        }

        return default;
    }

    /// <inheritdoc />
    public async Task SaveAsync<T>(string key, T value, StorageScope scope, ulong? groupId = null, ulong? userId = null)
    {
        using var serviceScope = scopeFactory.CreateScope();
        var db = serviceScope.ServiceProvider.GetRequiredService<BotDbContext>();

        var eneityScope = MapToEntityScope(scope);

        var entity = await db.PluginStates
                             .FirstOrDefaultAsync(p =>
                                                      p.PluginId == pluginId &&
                                                      p.Scope == eneityScope &&
                                                      p.GroupId == groupId &&
                                                      p.UserId == userId)
                             .ConfigureAwait(false);
        Dictionary<string, object> dateDict;

        if (entity == null)
        {
            dateDict = [];
            entity = new PluginState
            {
                PluginId = pluginId,
                Scope = eneityScope,
                GroupId = groupId,
                UserId = userId,
                StateDataJson = "{}"
            };
            db.PluginStates.Add(entity);
        }
        else
        {
            dateDict = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.StateDataJson) ?? [];
        }

        dateDict[key] = value!;

        entity.StateDataJson = JsonSerializer.Serialize(dateDict);
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private static ScopeType MapToEntityScope(StorageScope sdkScope) => sdkScope switch
    {
        StorageScope.Global => ScopeType.Global,
        StorageScope.Group => ScopeType.Group,
        StorageScope.User => ScopeType.User,
        StorageScope.GroupUser => ScopeType.GroupUser,
        _ => ScopeType.Global
    };
}
