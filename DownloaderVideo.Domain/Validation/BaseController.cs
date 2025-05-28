using DownloaderVideo.Domain.Interface.Services.v1;
using Microsoft.AspNetCore.Mvc;

namespace DownloaderVideo.Domain.Validation;

public abstract class BaseController(INotificationBase _notificationBase) : ControllerBase
{
    protected bool HasNotifications()
    {
        return _notificationBase.HasNotifications();
    }

    protected ActionResult ResponseResult(Object Result)
    {
        return UnprocessableEntity(new
        {
            Success = false,
            Errors = _notificationBase.GetNotifications(),
            Data = Result
        });
    }
}
