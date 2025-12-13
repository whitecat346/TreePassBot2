using BackManager.Server.Models;
using System.Net;
using System.Text.Json;

namespace BackManager.Server.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
/// <remarks>
/// 全局异常处理中间件构造函数
/// </remarks>
/// <param name="next">下一个中间件</param>
/// <param name="logger">日志记录器</param>
public class GlobalExceptionHandler(
    RequestDelegate next,
    ILogger<GlobalExceptionHandler> logger)
{
    /// <summary>
    /// 中间件执行方法
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>任务</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <param name="exception">异常对象</param>
    /// <returns>任务</returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = ApiResponse<object>.Error(
            message: exception.Message,
            code: context.Response.StatusCode
        );

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}
