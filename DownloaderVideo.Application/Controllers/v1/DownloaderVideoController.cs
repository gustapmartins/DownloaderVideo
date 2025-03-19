using DownloaderVideo.Application.AppServices.v1.Interfaces;
using DownloaderVideo.Application.Static;
using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Interface.Services.v1;
using DownloaderVideo.Domain.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    ///   Baixar um v�deo do YouTube
    /// </summary>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso seja feita com sucesso</response>
    /// <response code="204">Caso n�o passe nenhuma url</response>
    [HttpPost("downloadVideo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(Summary = "Baixa um v�deo na qualidade selecionada.")]
    public IActionResult DownloadVideo([FromQuery] string url, [FromQuery] string quality)
    {
        if (string.IsNullOrWhiteSpace(url) || !VideoUrlValidator.IsValidYouTubeUrl(url))
        {
            return BadRequest("A URL fornecida n�o � v�lida.");
        }

        OperationResult<Stream> result = _generateTemplateAppService.DownloadVideo(url, quality);

        if (HasNotifications())
            return ResponseResult(result);
        if (result.Content is not null)
        {
            string fileName = $"video_{DateTime.Now:yyyyMMddHHmmss}.mp4";
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            Response.Headers.Add("Content-Type", "video/mp4");

            return new FileStreamResult(result.Content, "video/mp4");
        }
        else
            return NoContent();
    }

    /// <summary>
    ///   Baixar um v�deo do YouTube
    /// </summary>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso seja feita com sucesso</response>
    /// <response code="204">Caso n�o passe nenhuma url</response>
    [HttpGet("available-qualities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(Summary = "Baixa um v�deo na qualidade selecionada.")]
    public async Task<IActionResult> GetAvailableQualitiesAsync([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url) || !VideoUrlValidator.IsValidYouTubeUrl(url))
        {
            return BadRequest("A URL fornecida n�o � v�lida.");
        }

        OperationResult<List<DownloaderVideoEntity>> result = await _generateTemplateAppService.GetAvailableQualitiesAsync(url);

        if (HasNotifications())
            return ResponseResult(result);
        if (result.Content is not null)
            return Ok(result);
        else
            return NoContent();
    }
}
