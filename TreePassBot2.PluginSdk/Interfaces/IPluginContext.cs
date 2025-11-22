namespace TreePassBot2.PluginSdk.Interfaces;

public interface IPluginContext
{
    void RegisterCommand(IBotCommand command);

    IBotApi BotApi { get; }
    IPluginStateStorage State { get; }
}