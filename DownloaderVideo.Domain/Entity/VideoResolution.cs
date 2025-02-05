using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderVideo.Domain.Entity
{
    public class VideoResolution
    {
        public VideoResolution(string id, string resolution)
        {
            Id = id;
            Resolution = resolution;
        }

        public string Id { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
    }
}
