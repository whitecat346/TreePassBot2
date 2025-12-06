
using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers
{
    /// <summary>
    /// 成员管理控制器
    /// </summary>
    [ApiController]
    [Route("api/groups/{groupId}/members")]
    public class MembersController : ControllerBase
    {
        /// <summary>
        /// 成员模型
        /// </summary>
        private record GroupMember
        {
            public required string Id { get; set; }
            public required string GroupId { get; set; }
            public required string UserId { get; set; }
            public required string Username { get; set; }
            public string? Nickname { get; set; }
            public string? Avatar { get; set; }
            public required string Role { get; set; }
            public required string JoinedAt { get; set; }
        }

        /// <summary>
        /// 获取群组成员列表
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns>成员列表</returns>
        [HttpGet]
        public IActionResult GetGroupMembers(string groupId)
        {
            // 模拟群组成员数据
            List<GroupMember> members =
            [
                new()
                {
                    Id = "1",
                    GroupId = groupId,
                    UserId = "user1",
                    Username = "用户1",
                    Nickname = "昵称1",
                    Avatar = null,
                    Role = "Owner",
                    JoinedAt = DateTime.UtcNow.AddMonths(-2).ToString("o")
                },

                new()
                {
                    Id = "2",
                    GroupId = groupId,
                    UserId = "user2",
                    Username = "用户2",
                    Nickname = "昵称2",
                    Avatar = null,
                    Role = "Admin",
                    JoinedAt = DateTime.UtcNow.AddMonths(-1).ToString("o")
                },

                new()
                {
                    Id = "3",
                    GroupId = groupId,
                    UserId = "user3",
                    Username = "用户3",
                    Nickname = "昵称3",
                    Avatar = null,
                    Role = "Member",
                    JoinedAt = DateTime.UtcNow.AddDays(-10).ToString("o")
                },

                new()
                {
                    Id = "4",
                    GroupId = groupId,
                    UserId = "bot1",
                    Username = "机器人",
                    Nickname = "机器人昵称",
                    Avatar = null,
                    Role = "Bot",
                    JoinedAt = DateTime.UtcNow.AddDays(-5).ToString("o")
                }
            ];

            return Ok(ApiResponse<List<GroupMember>>.Ok(members, "获取群组成员列表成功"));
        }
    }
}