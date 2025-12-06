using BackManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable ComplexConditionExpression

namespace BackManager.Server.Controllers
{
    /// <summary>
    /// 审核记录控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuditsController : ControllerBase
    {
        /// <summary>
        /// 审核记录模型
        /// </summary>
        private record AuditRecord
        {
            public required string Id { get; set; }
            public required string UserId { get; set; }
            public required string Username { get; set; }
            public required string GroupId { get; set; }
            public required string GroupName { get; set; }
            public required string Status { get; set; }
            public string? VerificationCode { get; set; }
            public bool EnteredGroup { get; set; }
            public required string CreatedAt { get; set; }
            public string? ProcessedAt { get; set; }
            public string? ProcessedBy { get; set; }
        }

        /// <summary>
        /// 获取审核记录列表
        /// </summary>
        /// <returns>审核记录列表</returns>
        [HttpGet]
        public IActionResult GetAuditRecords()
        {
            // 模拟审核记录数据
            var audits = new List<AuditRecord>
            {
                new()
                {
                    Id = "1",
                    UserId = "user1",
                    Username = "用户1",
                    GroupId = "group1",
                    GroupName = "测试群组1",
                    Status = "Pending",
                    VerificationCode = null,
                    EnteredGroup = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-2).ToString("o"),
                    ProcessedAt = null,
                    ProcessedBy = null
                },
                new()
                {
                    Id = "2",
                    UserId = "user2",
                    Username = "用户2",
                    GroupId = "group2",
                    GroupName = "测试群组2",
                    Status = "Approved",
                    VerificationCode = "123456",
                    EnteredGroup = true,
                    CreatedAt = DateTime.UtcNow.AddHours(-4).ToString("o"),
                    ProcessedAt = DateTime.UtcNow.AddHours(-3).ToString("o"),
                    ProcessedBy = "admin1"
                },
                new()
                {
                    Id = "3",
                    UserId = "user3",
                    Username = "用户3",
                    GroupId = "group1",
                    GroupName = "测试群组1",
                    Status = "Rejected",
                    VerificationCode = null,
                    EnteredGroup = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-6).ToString("o"),
                    ProcessedAt = DateTime.UtcNow.AddHours(-5).ToString("o"),
                    ProcessedBy = "admin2"
                }
            };

            return Ok(ApiResponse<List<AuditRecord>>.Ok(audits, "获取审核记录成功"));
        }

        /// <summary>
        /// 批准审核
        /// </summary>
        /// <param name="auditId">审核ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{auditId}/approve")]
        public IActionResult ApproveAudit(string auditId)
        {
            // 模拟批准审核操作
            return Ok(ApiResponse<object>.Ok(null, $"审核 {auditId} 已批准"));
        }

        /// <summary>
        /// 拒绝审核
        /// </summary>
        /// <param name="auditId">审核ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{auditId}/reject")]
        public IActionResult RejectAudit(string auditId)
        {
            // 模拟拒绝审核操作
            return Ok(ApiResponse<object>.Ok(null, $"审核 {auditId} 已拒绝"));
        }
    }
}