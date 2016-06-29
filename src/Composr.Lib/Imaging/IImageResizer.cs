using System.IO;

namespace Composr.Lib.Imaging
{
    public interface IImageResizer
    {
        void Resize(ImageInfo info);
    }
}