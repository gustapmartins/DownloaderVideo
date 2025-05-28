using System.Diagnostics.CodeAnalysis;

namespace DownloaderVideo.Domain.Entity;

[ExcludeFromCodeCoverage]
public class OperationResult<T>
{
    public OperationResult()
    {
        Content = default!;
    }

    public OperationResult(T content, string message, int statusCode, bool status)
    {
        Content = content;
        Message = message;
        StatusCode = statusCode;
        Status = status;
    }

    public string Message { get; set; } = string.Empty;

    public T Content { get; set; }

    public int StatusCode { get; set; }

    public bool Status { get; set; }
}
