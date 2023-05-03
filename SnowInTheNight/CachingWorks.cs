using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    static class CachingWorks
    {
        public static Size TreeSize = new Size(69, 89);
        private static Bitmap[] treeImages = null;
        public static Bitmap GetTree(int frame)
        {
            if (treeImages == null)
            {
                var size = TreeSize;
                var image = SnowInTheNight.Properties.Resources.tree;
                var count = image.Width / size.Width;
                treeImages = new Bitmap[count];
                for (int i = 0; i < count; i++)
                {
                    var sourceArea = new Rectangle(new Point(size.Width * i, 0), size);
                    var croppedImage = image.Clone(sourceArea, image.PixelFormat);
                    treeImages[i] = croppedImage;
                }
            }
            frame %= treeImages.Length;
            return treeImages[frame];
        }
        public static Size StumpSize = new Size(28, 15);
        private static Bitmap[] stumpImages = null;
        internal static Bitmap GetStump(int frame)
        {
            if (stumpImages == null)
            {
                var size = StumpSize;
                var image = SnowInTheNight.Properties.Resources.stump;
                var count = image.Width / size.Width;
                stumpImages = new Bitmap[count];
                for (int i = 0; i < count; i++)
                {
                    var sourceArea = new Rectangle(new Point(size.Width * i, 0), size);
                    var croppedImage = image.Clone(sourceArea, image.PixelFormat);
                    stumpImages[i] = croppedImage;
                }
            }
            frame %= stumpImages.Length;
            return stumpImages[frame];
        }
        public static Size GraveSize = new Size(10, 12);
        private static Bitmap[] graveImages = null;
        internal static Bitmap GetGraveStone(int frame)
        {
            if (graveImages == null)
            {
                var size = GraveSize;
                var image = SnowInTheNight.Properties.Resources.grave;
                var count = image.Width / size.Width;
                graveImages = new Bitmap[count];
                for (int i = 0; i < count; i++)
                {
                    var sourceArea = new Rectangle(new Point(size.Width * i, 0), size);
                    var croppedImage = image.Clone(sourceArea, image.PixelFormat);
                    graveImages[i] = croppedImage;
                }
            }
            frame %= graveImages.Length;
            return graveImages[frame];
        }
    }
}
