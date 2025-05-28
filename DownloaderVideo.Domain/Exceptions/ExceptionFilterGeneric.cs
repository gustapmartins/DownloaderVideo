namespace DownloaderVideo.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

public class ExceptionFilterGeneric : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionFilterGeneric> _logger;

    public ExceptionFilterGeneric(ILogger<ExceptionFilterGeneric> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {

        var result = new ObjectResult(new
        {
            context.Exception.Message,
            context.Exception.Source,
            ExceptionType = context.Exception.GetType().FullName,
            Path = context.HttpContext.Request.Path,
        })
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        _logger.LogError("Unhandled exception ocurred while executing request: {ex}", context.Exception);

        context.Result = result;
    }
}
