namespace TreePassBot2.Infrastructure.Exceptions;

/// <summary>
/// Throws if failed to execute API.
/// </summary>
public class FailedToExcuteApiException : Exception
{
    /// <summary>
    /// Throws if failed to execute API.
    /// </summary>
    /// <param name="apiName">API name.</param>
    public FailedToExcuteApiException(string apiName) : base(apiName)
    {
    }

    /// <inheritdoc />
    public FailedToExcuteApiException(string apiName, Exception innerException) : base(apiName, innerException)
    {
    }
}