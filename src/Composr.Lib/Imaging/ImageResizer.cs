using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Composr.Lib.Imaging
{
    public class ImageResizer : IImageResizer
    {
        public void Resize(ImageInfo imageinfo)
        {
            ValidateInput(imageinfo);
            Image image = Image.FromFile(imageinfo.SourceFile);
            ImageSize aspectRatioSize = ComputeAspectRatioDimensions(image, imageinfo.ImageSize);
            Image finalImage = imageinfo.ImageSize.Equals(aspectRatioSize) ? image : GetResizedImage(image, aspectRatioSize);            
            CompressAndSave(finalImage, imageinfo.DestinationFile);
        }

        private void ValidateInput(ImageInfo imageinfo)
        {
            if (imageinfo == null && !string.IsNullOrWhiteSpace(imageinfo.SourceFile) && !File.Exists(imageinfo.SourceFile)) throw new ArgumentNullException("Source image cannot be null");
            if (imageinfo.ImageSize == null) throw new ArgumentNullException("Image size cannot be null");
            if (imageinfo.ImageSize.Width <= 0) throw new ArgumentOutOfRangeException("targetWidth", "The width must be greater than 0");
            if (imageinfo.ImageSize.Height <= 0) throw new ArgumentOutOfRangeException("targetHeight", "The height must be greater than 0");
        }

        private ImageSize ComputeAspectRatioDimensions(Image image, ImageSize size)
        {
            float widthratio = (float) size.Width / (float) image.Width;
            float heightratio = (float) size.Height / (float) image.Height;
            float ratio = (heightratio < widthratio) ? heightratio : widthratio;

            if (ratio >= 1)
                return size; //no image enlargement, keep original

            return new ImageSize
            {
                Name = size.Name,
                Width = (int)Math.Round(image.Width * ratio),
                Height = (int)Math.Round(image.Height * ratio)
            };
        }

        private Image GetResizedImage(Image image, ImageSize aspectRatioSize)
        {
            Bitmap newimg = new Bitmap(aspectRatioSize.Width, aspectRatioSize.Height);
            using (Graphics g = Graphics.FromImage(newimg))
            {
                g.Clear(Color.White);
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.Low;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.DrawImage(image, new Rectangle(0, 0, aspectRatioSize.Width, aspectRatioSize.Height));
            }

            return newimg;
        }

        private void CompressAndSave(Image image, string destinationFileName)
        {
            image.Save(destinationFileName, GetEncoder("image/jpeg"), GetEncodeParameters());
        }

        private ImageCodecInfo GetEncoder(string mime)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.Equals(mime))
                {
                    return codec;
                }
            }
            return null;
        }

        private EncoderParameters GetEncodeParameters()
        {
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Compression;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 90L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            return myEncoderParameters;
        }

    }
}
