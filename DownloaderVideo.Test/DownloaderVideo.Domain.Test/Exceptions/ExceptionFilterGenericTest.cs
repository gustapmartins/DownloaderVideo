using DownloaderVideo.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderVideo.Domain.Test.Exceptions;

public class ExceptionFilterGenericTest
{
    private readonly Mock<ILogger<ExceptionFilterGeneric>> _loggerMock;
    private readonly ExceptionFilterGeneric _exceptionFilter;

    public ExceptionFilterGenericTest()
    {
        _loggerMock = new Mock<ILogger<ExceptionFilterGeneric>>();
        _exceptionFilter = new ExceptionFilterGeneric(_loggerMock.Object);
    }

    [Fact]
    public void OnException_SetsContextResultWithInternalServerError()
    {
        // Arrange
        var context = new ExceptionContext(
            new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            ),
            new List<IFilterMetadata>()
        )
        {
            Exception = new Exception("Test exception")
        };
        context.HttpContext.Request.Path = "/test-path";

        // Act
        _exceptionFilter.OnException(context);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        Assert.Equal("Test exception", objectResult.Value.GetType().GetProperty("Message").GetValue(objectResult.Value, null));
        Assert.Equal("System.Exception", objectResult.Value.GetType().GetProperty("ExceptionType").GetValue(objectResult.Value, null));
        Assert.Equal("/test-path", ((PathString)objectResult.Value.GetType().GetProperty("Path").GetValue(objectResult.Value, null)).ToString());
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled exception ocurred while executing request:")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }
}
