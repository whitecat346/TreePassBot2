using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MetaInfo;

namespace TreePassBot2.BotEngine.Interfaces;

public interface IMessageHandler
{
    Task HandleMessageAsync(MessageEventData data);
}
