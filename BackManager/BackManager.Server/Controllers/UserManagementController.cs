using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UserManagementController : ControllerBase
    {
        /// <summary>
        /// 用户模型
        /// </summary>
        private record UserInfo
        {
            public required string Id { get; set; }
            public required string Username { get; set; }
            public required string Role { get; set; }
            public required string Avatar { get; set; }
            public required string Status { get; set; }
            public required string LastActive { get; set; }
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        [HttpGet]
        public IActionResult GetUsers()
        {
            // 模拟用户数据
            List<UserInfo> users =
            [
                new()
                {
                    Id = "1",
                    Username = "管理员",
                    Role = "Owner",
                    Avatar = "https://picsum.photos/id/1/100/100",
                    Status = "Online",
                    LastActive = DateTime.UtcNow.ToString("o")
                },

                new()
                {
                    Id = "2",
                    Username = "审核员",
                    Role = "Admin",
                    Avatar = "https://picsum.photos/id/2/100/100",
                    Status = "Online",
                    LastActive = DateTime.UtcNow.AddMinutes(-10).ToString("o")
                },

                new()
                {
                    Id = "3",
                    Username = "普通成员",
                    Role = "Member",
                    Avatar = "https://picsum.photos/id/3/100/100",
                    Status = "Offline",
                    LastActive = DateTime.UtcNow.AddHours(-2).ToString("o")
                },

                new()
                {
                    Id = "4",
                    Username = "机器人助手",
                    Role = "Bot",
                    Avatar = "https://picsum.photos/id/4/100/100",
                    Status = "Online",
                    LastActive = DateTime.UtcNow.ToString("o")
                }
            ];

            return Ok(ApiResponse<List<UserInfo>>.Ok(users, "获取用户列表成功"));
        }

        /// <summary>
        /// 角色更新请求模型
        /// </summary>
        public class UpdateRoleRequest
        {
            public string Role { get; set; } = string.Empty;
        }

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleData">角色更新数据</param>
        /// <returns>操作结果</returns>
        [HttpPut("{userId}/role")]
        public IActionResult UpdateUserRole(string userId, [FromBody] UpdateRoleRequest roleData)
        {
            // 模拟更新用户角色操作
            return Ok(ApiResponse<object>.Ok(null, $"用户 {userId} 角色已更新为 {roleData.Role}"));
        }

        /// <summary>
        /// 禁用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{userId}/ban")]
        public IActionResult BanUser(string userId)
        {
            // 模拟禁用用户操作
            return Ok(ApiResponse<object>.Ok(null, $"用户 {userId} 已禁用"));
        }

        /// <summary>
        /// 解禁用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{userId}/unban")]
        public IActionResult UnbanUser(string userId)
        {
            // 模拟解禁用户操作
            return Ok(ApiResponse<object>.Ok(null, $"用户 {userId} 已解禁"));
        }
    }
}