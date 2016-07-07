using Composr.Lib.Util;
using Microsoft.Extensions.Logging;
using System.IO;
using Composr.Lib.Imaging;
using System.Collections.Generic;

namespace Composr.CLI
{
    public class ImageClient
    {
        private ILogger logger = Logging.CreateLogger<ImageClient>();
        private ImageResizer resizer = new ImageResizer();

        private List<ImageSize> sizes = new List<ImageSize>
        {
            //new ImageSize { Name = "md", Width = 1120, Height = 840 },
            new ImageSize { Name = "sm", Width = 800, Height = 600 },
            new ImageSize { Name = "xs", Width = 400, Height = 300 }
            //new ImageSize { Name = "th", Width = 80, Height = 60 },
        };

        public ImageClient()
        {
            
        }

        public void ProcessImages()
        {
            logger.LogInformation("starting image processing");
            CleanDirectory(Settings.ImageSourceDirectory);
            foreach (var file in Directory.GetFiles(Settings.ImageSourceDirectory, "*.jpg"))
                foreach (var size in sizes)
                    ResizeImage(file.ToLowerInvariant(), size);
        }

        private void ResizeImage(string file, ImageSize size)
        {
            logger.LogInformation($"resizing image: {file} to dimensions: {size.Name} - {size.Width}x{size.Height}");
            ImageInfo info = new ImageInfo()
            {
                ImageSize = size,
                SourceFile = file,
                DestinationFile = file.Replace(".jpg", $"-{size.Name}.jpg").Replace("original\\", "")
            };
            resizer.Resize(info);
        }

        private void CleanDirectory(string imageSourceDirectory)
        {
            logger.LogInformation("cleaning destination directory");
            foreach (string file in Directory.GetFiles(Settings.ImageDestinationDirectory))
                File.Delete(file);
        }
    }
}
