using TreePassBot2.PluginSdk.Entities.Enums;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface IBotCommand
{
    string Trigger { get; }
    string[] Aliases { get; }

    CommandScope Scope { get; }
    UserRole MinRole { get; }

    /// <summary>
    /// Execute command logic.
    /// </summary>
    /// <param name="ctx">Command context.</param>
    Task ExecuteAsync(ICommandContext ctx);
}
