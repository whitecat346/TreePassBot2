using Makabaka.Messages;
using TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments;
using AtSegment = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.AtSegment;
using FaceSegment = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.FaceSegment;
using ForwardSegment = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.ForwardSegment;
using ImageSegment = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.ImageSegment;
using Message = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.Message;
using PokeSegment = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.PokeSegment;
using ReplySegment = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.ReplySegment;
using TextSegment = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.TextSegment;
using VideoSegment = TreePassBot2.Infrastructure.MakabakaAdaptor.Models.MessageSegments.VideoSegment;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor.Converters;

/// <summary>
/// 消息转换器，用于消息对象的相互转换
/// </summary>
public static class MessageConverter
{
    /// <summary>
    /// 将TreePassBot2的消息转换为Makabaka的消息
    /// </summary>
    /// <param name="message">TreePassBot2的消息对象</param>
    /// <returns>Makabaka的消息对象</returns>
    public static Makabaka.Messages.Message ConvertToMakabakaMessage(Message message)
    {
        var makabakaMessage = new Makabaka.Messages.Message();
        makabakaMessage.AddRange(message.Select(ConvertToMakabakaSegment).OfType<Segment>());

        return makabakaMessage;
    }

    /// <summary>
    /// 将Makabaka的消息转换为TreePassBot2的消息
    /// </summary>
    /// <param name="message">Makabaka的消息对象</param>
    /// <returns>TreePassBot2的消息对象</returns>
    public static Message ConvertToBotMessage(Makabaka.Messages.Message message)
    {
        var treePassBotMessage = new Message();

        foreach (var segment in message)
        {
            var treePassBotSegment = ConvertToBotSegment(segment);
            if (treePassBotSegment != null)
            {
                treePassBotMessage.Add(treePassBotSegment);
            }
        }

        return treePassBotMessage;
    }

    /// <summary>
    /// 将TreePassBot2的消息段转换为Makabaka的消息段
    /// </summary>
    /// <param name="segment">TreePassBot2的消息段</param>
    /// <returns>Makabaka的消息段</returns>
    private static Segment? ConvertToMakabakaSegment(MessageSegment segment) =>
        segment switch
        {
            TextSegment textSegment => new Makabaka.Messages.TextSegment(textSegment.Text),
            AtSegment atSegment => new Makabaka.Messages.AtSegment(atSegment.UserId, atSegment.Text),
            ReplySegment replySegment => new Makabaka.Messages.ReplySegment(replySegment.MessageId),
            FaceSegment faceSegment => new Makabaka.Messages.FaceSegment(faceSegment.FaceId.ToString()),
            ImageSegment imageSegment => new Makabaka.Messages.ImageSegment(imageSegment.File),
            PokeSegment pokeSegment => new Makabaka.Messages.PokeSegment(pokeSegment.PokeType, pokeSegment.PokeId),
            ForwardSegment forwardSegment => new Makabaka.Messages.ForwardSegment(forwardSegment.ForwardId),
            VideoSegment videoSegment => new Makabaka.Messages.VideoSegment(videoSegment.File),
            _ => null
        };

    /// <summary>
    /// 将Makabaka的消息段转换为TreePassBot2的消息段
    /// </summary>
    /// <param name="segment">Makabaka的消息段</param>
    /// <returns>TreePassBot2的消息段</returns>
    private static MessageSegment? ConvertToBotSegment(Segment segment)
    {
        switch (segment)
        {
            case Makabaka.Messages.TextSegment textSegment:
                return new TextSegment(textSegment.Data.Text);

            case Makabaka.Messages.AtSegment atSegment:
                if (atSegment.Data.QQ == "all")
                    return new AtSegment(0, atSegment.Data.Name);

                _ = ulong.TryParse(atSegment.Data.QQ, out var userId);
                return new AtSegment(userId, atSegment.Data.Name);

            case Makabaka.Messages.ReplySegment replySegment:
                return long.TryParse(replySegment.Data.Id, out var messageId)
                    ? new ReplySegment(messageId)
                    : null;
            case Makabaka.Messages.FaceSegment faceSegment:
                return int.TryParse(faceSegment.Data.Id, out var faceId)
                    ? new FaceSegment(faceId)
                    : null;

            case Makabaka.Messages.ImageSegment imageSegment:
                return new ImageSegment(imageSegment.Data.File);

            case Makabaka.Messages.PokeSegment pokeSegment:
                return new PokeSegment(0, pokeSegment.Data.Type, pokeSegment.Data.Id);

            case Makabaka.Messages.ForwardSegment forwardSegment:
                return new ForwardSegment(forwardSegment.Data.Id);

            case Makabaka.Messages.VideoSegment videoSegment:
                return new VideoSegment(videoSegment.Data.File);

            default:
                return null;
        }
    }
}
