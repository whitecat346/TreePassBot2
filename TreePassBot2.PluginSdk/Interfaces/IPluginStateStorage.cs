using TreePassBot2.PluginSdk.Entities.Enums;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface IPluginStateStorage
{
    Task<T?> GetAsync<T>(string key, StorageScope scope, ulong? groupId = null, ulong? userId = null);

    Task SaveAsync<T>(string key, T value, StorageScope scope, ulong? groupId = null, ulong? userId = null);
}
