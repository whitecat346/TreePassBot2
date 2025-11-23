using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.PluginSdk.Interfaces;

public interface IPluginContext
{
    void RegisterCommand(IBotCommand command);

    ITreePassBotCommunicationService BotApi { get; }
    IPluginStateStorage State { get; }
}