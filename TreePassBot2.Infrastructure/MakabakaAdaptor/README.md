# Makabaka-Adaptor

Makabaka-Adaptor是一个为TreePassBot2项目设计的适配器层，用于将TreePassBot2的消息模型转换为Makabaka框架的消息模型，实现与QQ通信的解耦。

## 功能特性

- 提供独立的消息模型（TreePassBot2.Infrastructure.Messages命名空间）
- 实现消息模型转换机制，将TreePassBot2消息转换为Makabaka消息
- 提供通信服务接口（ITreePassBotCommunicationService），实现与QQ通信的解耦
- 支持多种消息类型（文本、@、表情、图片、回复、戳一戳等）
- 内置消息构建器，方便构建复杂消息
- 支持依赖注入
- 完善的异常处理

## 项目结构

- **TreePassBot2.Infrastructure.Messages**：独立的消息模型
  - MessageSegment：消息段基类
  - TextSegment、AtSegment、FaceSegment等：各种类型的消息段
  - Message：消息容器
  - MessageBuilder：消息构建器
- **TreePassBot2.Infrastructure**：通信服务接口和实现
  - ITreePassBotCommunicationService：通信服务接口
  - MakabakaService：基于Makabaka的实现
- **Makabaka.Adaptor.Converters**：消息转换器
  - MessageConverter：将TreePassBot2消息转换为Makabaka消息

## 安装方法

将Makabaka-Adaptor项目添加到您的解决方案中，并添加对它的引用。

## 使用方法

### 1. 配置依赖注入

```csharp
// 在Startup.cs或Program.cs中
using Makabaka.Adaptor;

// 添加TreePassBot Makabaka服务
builder.Services.AddTreePassBotMakabakaService();
```

### 2. 发送简单消息

```csharp
using TreePassBot2.Infrastructure;
using TreePassBot2.Infrastructure.Messages;

public class YourService
{
    private readonly ITreePassBotCommunicationService _communicationService;

    public YourService(ITreePassBotCommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    public async Task SendMessageAsync()
    {
        // 创建消息构建器
        var messageBuilder = new MessageBuilder();
        messageBuilder.AddText("你好，这是一条测试消息");
        
        // 发送群消息
        await _communicationService.SendGroupMessageAsync(123456789, messageBuilder);
        
        // 发送私聊消息
        await _communicationService.SendPrivateMessageAsync(1234567890, messageBuilder);
    }
}
```

### 3. 发送复杂消息

```csharp
public async Task SendComplexMessageAsync()
{
    // 创建消息构建器
    var messageBuilder = new MessageBuilder();
    
    // 添加多个消息段
    messageBuilder.AddText("大家好，")
                 .AddAt(1234567890) // @用户
                 .AddText(" 请看这个表情 ")
                 .AddFace(123) // 表情
                 .AddText(" 和这个图片")
                 .AddImage("https://example.com/image.jpg"); // 图片
    
    // 发送群消息
    await _communicationService.SendGroupMessageAsync(123456789, messageBuilder);
}
```

### 4. 发送回复消息

```csharp
public async Task SendReplyMessageAsync()
{
    // 创建消息构建器
    var messageBuilder = new MessageBuilder();
    
    // 添加回复和文本内容
    messageBuilder.AddReply(12345678901234) // 回复消息ID
                 .AddText("这是回复的内容");
    
    // 发送群消息
    await _communicationService.SendGroupMessageAsync(123456789, messageBuilder);
}
```

## 支持的消息类型

- 文本（Text）
- @用户（At）
- QQ表情（Face）
- 图片（Image）
- 回复消息（Reply）
- 戳一戳（Poke）

## 解耦设计说明

本适配器实现了以下解耦设计：

1. **独立的消息模型**：所有消息相关的类都位于TreePassBot2.Infrastructure.Messages命名空间，与具体的通信实现无关
2. **通信服务接口**：ITreePassBotCommunicationService定义了通信操作，不依赖于具体实现
3. **适配器模式**：MakabakaService作为适配器，将TreePassBot2的操作转换为Makabaka的操作
4. **消息转换机制**：通过MessageConverter实现消息模型之间的转换

这种设计使得将来可以轻松切换到其他通信方式，只需实现新的ITreePassBotCommunicationService即可，而不需要修改业务代码。

## 注意事项

- 请确保已正确配置Makabaka框架
- 发送消息时请遵守相关法律法规和平台规定
- 图片消息支持本地文件路径和网络URL

## 兼容性

- 支持.NET 6.0及以上版本
- 兼容Makabaka框架的所有版本

## 许可证

本项目采用MIT许可证。