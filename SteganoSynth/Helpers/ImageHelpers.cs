using System;
using AppKit;
using CoreGraphics;

namespace ImageMusic
{
    public static class ImageHelpers
    {
        public static NSImage ResizeImage (NSImage image, CGSize maxSize)
        {
            nfloat ratio = 0;

            var imageWidth = image.Size.Width;
            var imageHeight = image.Size.Height;

            var maxWidth = maxSize.Width;
            var maxHeight = maxSize.Height;

            if (imageWidth > imageHeight)
            {
                ratio = maxWidth / imageWidth;
            }
            else
            {
                ratio = maxHeight / imageHeight;
            }

            var newWidth = imageWidth * ratio;
            var newHeight = imageHeight * ratio;

            var newSize = new CGSize(newWidth, newHeight);

            var imageRect = new CGRect(0, 0, imageWidth, imageHeight);
            var imageRef = image.AsCGImage(ref imageRect, null, null);

            return new NSImage(imageRef, newSize);
        }
    }
}
