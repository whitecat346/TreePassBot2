using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackManager.Server.Controllers
{
    /// <summary>
    /// 消息日志控制器
    /// </summary>
    [ApiController]
    [Route("api/messages")]
    public class MessageLogsController : ControllerBase
    {
        /// <summary>
        /// 消息日志模型
        /// </summary>
        private class MessageLog
        {
            public string Id { get; set; }
            public string GroupId { get; set; }
            public string GroupName { get; set; }
            public string UserId { get; set; }
            public string Username { get; set; }
            public string Content { get; set; }
            public string SendTime { get; set; }
            public bool IsRecalled { get; set; }
            public string? RecalledBy { get; set; }
            public string? RecalledAt { get; set; }
        }

        /// <summary>
        /// 获取消息日志列表
        /// </summary>
        /// <returns>消息日志列表</returns>
        [HttpGet]
        public IActionResult GetMessageLogs()
        {
            // 模拟消息日志数据
            List<MessageLog> logs =
            [
                new()
                {
                    Id = "1",
                    GroupId = "group1",
                    GroupName = "测试群组1",
                    UserId = "user1",
                    Username = "用户1",
                    Content = "大家好！",
                    SendTime = DateTime.UtcNow.AddHours(-1).ToString("o"),
                    IsRecalled = false,
                    RecalledBy = null,
                    RecalledAt = null
                },

                new()
                {
                    Id = "2",
                    GroupId = "group1",
                    GroupName = "测试群组1",
                    UserId = "user2",
                    Username = "用户2",
                    Content = "你好！",
                    SendTime = DateTime.UtcNow.AddMinutes(-50).ToString("o"),
                    IsRecalled = false,
                    RecalledBy = null,
                    RecalledAt = null
                },

                new()
                {
                    Id = "3",
                    GroupId = "group2",
                    GroupName = "测试群组2",
                    UserId = "user3",
                    Username = "用户3",
                    Content = "测试消息",
                    SendTime = DateTime.UtcNow.AddHours(-2).ToString("o"),
                    IsRecalled = true,
                    RecalledBy = "user3",
                    RecalledAt = DateTime.UtcNow.AddHours(-1.5).ToString("o")
                }
            ];

            return Ok(ApiResponse<List<MessageLog>>.Ok(logs, "获取消息日志成功"));
        }

        /// <summary>
        /// 搜索消息日志
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="groupId">群组ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>搜索结果</returns>
        [HttpGet("search")]
        public IActionResult SearchMessageLogs([FromQuery] string? keyword, [FromQuery] string? groupId,
                                               [FromQuery] string? startTime, [FromQuery] string? endTime)
        {
            // 模拟搜索功能，返回相同的日志数据
            var logs = new List<MessageLog>
            {
                new()
                {
                    Id = "1",
                    GroupId = "group1",
                    GroupName = "测试群组1",
                    UserId = "user1",
                    Username = "用户1",
                    Content = "大家好！",
                    SendTime = DateTime.UtcNow.AddHours(-1).ToString("o"),
                    IsRecalled = false,
                    RecalledBy = null,
                    RecalledAt = null
                }
            };

            return Ok(ApiResponse<List<MessageLog>>.Ok(logs, "搜索消息日志成功"));
        }

        /// <summary>
        /// 获取消息详情
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns>消息详情</returns>
        [HttpGet("{messageId}")]
        public IActionResult GetMessageDetail(string messageId)
        {
            // 模拟消息详情数据
            var log = new MessageLog
            {
                Id = messageId,
                GroupId = "group1",
                GroupName = "测试群组1",
                UserId = "user1",
                Username = "用户1",
                Content = "大家好！这是一条测试消息。",
                SendTime = DateTime.UtcNow.AddHours(-1).ToString("o"),
                IsRecalled = false,
                RecalledBy = null,
                RecalledAt = null
            };

            return Ok(ApiResponse<MessageLog>.Ok(log, "获取消息详情成功"));
        }
    }
}
