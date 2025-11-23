namespace TreePassBot2.Infrastructure.MakabakaAdaptor;

/// <summary>
/// 适配器使用示例
/// </summary>
public class ExampleUsage
{
    private readonly IMakabakaAdapter _adapter;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="adapter">Makabaka适配器</param>
    public ExampleUsage(IMakabakaAdapter adapter)
    {
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
    }

    /// <summary>
    /// 发送简单文本消息
    /// </summary>
    public async Task SendSimpleMessageAsync()
    {
        // 发送群消息
        var groupMessageBuilder = new MessageBuilder().AddText("这是一条简单的群消息");
        await _adapter.SendGroupMessageAsync(123456789, groupMessageBuilder);

        // 发送私聊消息
        var privateMessageBuilder = new MessageBuilder().AddText("这是一条简单的私聊消息");
        await _adapter.SendPrivateMessageAsync(987654321, privateMessageBuilder);
    }

    /// <summary>
    /// 发送复杂消息
    /// </summary>
    public async Task SendComplexMessageAsync()
    {
        // 构建复杂的群消息
        var groupMessageBuilder = new MessageBuilder()
                                 .AddText("大家好！")
                                 .AddAt(123456789) // @特定用户
                                 .AddAt(0)         // @全体成员
                                 .AddText("\n这是一张图片:")
                                 .AddImage("https://example.com/image.jpg")
                                 .AddText("\n这是一个表情:")
                                 .AddFace(1); // 表情ID

        await _adapter.SendGroupMessageAsync(123456789, groupMessageBuilder);

        // 构建复杂的私聊消息
        var privateMessageBuilder = new MessageBuilder()
                                   .AddText("你好！")
                                   .AddText("\n这是一条带表情的消息:")
                                   .AddFace(2); // 表情ID

        await _adapter.SendPrivateMessageAsync(987654321, privateMessageBuilder);
    }

    /// <summary>
    /// 发送回复消息
    /// </summary>
    public async Task SendReplyMessageAsync(long messageId)
    {
        var messageBuilder = new MessageBuilder()
                            .AddReply(messageId) // 回复指定消息
                            .AddText("这是一条回复消息");

        await _adapter.SendGroupMessageAsync(123456789, messageBuilder);
    }
}