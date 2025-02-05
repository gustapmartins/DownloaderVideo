using DownloaderVideo.Application.AppServices.v1.Interfaces;
using DownloaderVideo.Application.Dto.v1;
using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using DownloaderVideo.Domain.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics;
using System.Net;
using YoutubeExplode;

namespace DownloaderVideo.Application.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v1/[controller]", Order = 1)]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class DownloaderVideoController : BaseController
{

    private readonly ILogger<DownloaderVideoController> _logger;
    private readonly IDownloaderVideoAppServices _generateTemplateAppService;

    public DownloaderVideoController(
        ILogger<DownloaderVideoController> logger,
        IDownloaderVideoAppServices appService,
        INotificationBase notificationBase) : base(notificationBase)
    {
        _logger = logger;
        _generateTemplateAppService = appService;
    }


    /// <summary>
    ///   Baixar um vídeo do YouTube
    /// </summary>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso seja feita com sucesso</response>
    /// <response code="204">Caso não passe nenhuma url</response>
    [HttpGet("download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(Summary = "Baixa um vídeo na qualidade selecionada.")]
    public async Task<IActionResult> DownloadVideo([FromQuery] string url, [FromQuery] string quality)
    {
        OperationResult<byte[]> result = await _generateTemplateAppService.DownloadVideo(url, quality);

        if (HasNotifications())
            return ResponseResult(result);
        if (result.Content is not null)
            return File(result.Content, "application/octet-stream", result.Message);
        else
            return NoContent();
    }

    /// <summary>
    ///   Baixar um vídeo do YouTube
    /// </summary>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso seja feita com sucesso</response>
    /// <response code="204">Caso não passe nenhuma url</response>
    [HttpGet("available-qualities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(Summary = "Baixa um vídeo na qualidade selecionada.")]
    public IActionResult DownloadVideo([FromQuery] string url)
    {
        OperationResult<List<DownloaderVideoEntity>> result = _generateTemplateAppService.GetAvailableQualities(url);

        if (HasNotifications())
            return ResponseResult(result);
        if (result.Content is not null)
            return Ok(result.Content);
        else
            return NoContent();
    }
}
