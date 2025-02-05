using System.ComponentModel.DataAnnotations;

namespace DownloaderVideo.Application.Dto.v1;

public class DownloaderVideoRequest
{
    /// <summary>
    /// Nome do objeto
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
}
