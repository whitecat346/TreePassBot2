using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers
{
    /// <summary>
    /// 群组管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        /// <summary>
        /// 群组模型
        /// </summary>
        public class Group
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int MemberCount { get; set; }
            public string OwnerId { get; set; }
            public string CreatedAt { get; set; }
        }

        /// <summary>
        /// 获取群组列表
        /// </summary>
        /// <returns>群组列表</returns>
        [HttpGet]
        public IActionResult GetGroups()
        {
            // 模拟群组数据
            List<Group> groups =
            [
                new()
                {
                    Id = "1",
                    Name = "测试群组1",
                    MemberCount = 100,
                    OwnerId = "owner1",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2).ToString("o")
                },

                new()
                {
                    Id = "2",
                    Name = "测试群组2",
                    MemberCount = 200,
                    OwnerId = "owner2",
                    CreatedAt = DateTime.UtcNow.AddMonths(-1).ToString("o")
                },

                new()
                {
                    Id = "3",
                    Name = "测试群组3",
                    MemberCount = 50,
                    OwnerId = "owner3",
                    CreatedAt = DateTime.UtcNow.AddDays(-10).ToString("o")
                }
            ];

            return Ok(ApiResponse<List<Group>>.Ok(groups, "获取群组列表成功"));
        }
    }
}
