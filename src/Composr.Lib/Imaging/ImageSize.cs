using System.IO;

namespace Composr.Lib.Imaging
{
    public class ImageSize
    {
        public string Name { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public override bool Equals(object size)
        {
            if (size == null || size.GetType() == typeof(ImageSize)) return false;

            ImageSize s = (ImageSize)size;
            return s.Height == Height && s.Width == Width && s.Name == Name;
        }
    }

    public class ImageInfo
    {
        public string SourceFile { get; set; }
        public string DestinationFile { get; set; }
        public ImageSize ImageSize { get; set; }
    }
}
