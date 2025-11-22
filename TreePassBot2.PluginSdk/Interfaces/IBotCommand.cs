using TreePassBot2.PluginSdk.Entities;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface IBotCommand
{
    string Trigger { get; }
    string[] Aliases { get; }

    CommandScope Scope { get; }
    UserRole MinRole { get; }

    Task ExecuteAsync(ICommandContext ctx);
}