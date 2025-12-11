using TreePassBot2.Infrastructure.MakabakaAdaptor.Interfaces;

namespace TreePassBot2.Infrastructure.MakabakaAdaptor;

/// <summary>
/// TreePassBot通信服务使用示例
/// </summary>
public class TreePassBotExample
{
    private readonly ICommunicationService _communicationService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="communicationService">通信服务</param>
    public TreePassBotExample(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    /// <summary>
    /// 发送简单文本消息示例
    /// </summary>
    public async Task SendSimpleTextMessageAsync()
    {
        // 创建消息构建器
        var messageBuilder = new Models.MessageBuilder();

        // 添加文本内容
        messageBuilder.AddText("这是一条简单的文本消息");

        // 发送群消息
        await _communicationService.SendGroupMessageAsync(123456789, messageBuilder).ConfigureAwait(false);

        // 发送私聊消息
        await _communicationService.SendPrivateMessageAsync(1234567890, messageBuilder).ConfigureAwait(false);
    }

    /// <summary>
    /// 发送复杂消息示例
    /// </summary>
    public Task SendComplexMessageAsync()
    {
        // 创建消息构建器
        var messageBuilder = new Models.MessageBuilder();

        // 添加多个消息段
        messageBuilder.AddText("大家好，")
                      .AddAt(1234567890) // @用户
                      .AddText(" 请看这个表情 ")
                      .AddFace(123) // 表情
                      .AddText(" 和这个图片")
                      .AddImage("https://example.com/image.jpg"); // 图片

        // 发送群消息
        return _communicationService.SendGroupMessageAsync(123456789, messageBuilder);
    }

    /// <summary>
    /// 发送回复消息示例
    /// </summary>
    public Task SendReplyMessageAsync()
    {
        // 创建消息构建器
        var messageBuilder = new Models.MessageBuilder();

        // 添加回复和文本内容
        messageBuilder.AddReply(12345678901234) // 回复消息ID
                      .AddText("这是回复的内容");

        // 发送群消息
        return _communicationService.SendGroupMessageAsync(123456789, messageBuilder);
    }
}
