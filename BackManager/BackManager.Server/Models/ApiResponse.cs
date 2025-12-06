// g:/TreePassBot2/BackManager/BackManager.Server/Models/ApiResponse.cs

namespace BackManager.Server.Models
{
    /// <summary>
    /// 统一API响应模型
    /// </summary>
    /// <typeparam name="T">响应数据类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 响应数据
        /// </summary>
        public T Data { get; set; } = default!;

        /// <summary>
        /// 响应状态码
        /// </summary>
        public int Code { get; set; } = 200;

        /// <summary>
        /// 成功响应
        /// </summary>
        /// <param name="data">响应数据</param>
        /// <param name="message">响应消息</param>
        /// <returns>API响应对象</returns>
        public static ApiResponse<T?> Ok(T? data, string message = "请求成功")
        {
            return new ApiResponse<T?>
            {
                Success = true,
                Message = message,
                Data = data,
                Code = 200
            };
        }

        /// <summary>
        /// 错误响应
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="code">错误状态码</param>
        /// <returns>API响应对象</returns>
        public static ApiResponse<T> Error(string message = "请求失败", int code = 500)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default!,
                Code = code
            };
        }
    }
}