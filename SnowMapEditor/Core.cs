using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SnowMapEditor
{
    static class Core
    {
        private static double cumulativeDeltaTime = 0;
        private static double tickTime = 0.04;
        public static int Ticks = 0;
        public static void Update(double deltaTime)
        {
            if (editorState.SelectedItem != null)
            {
                editorState.SelectedItem.Comment = Meta.panelData.Comment;
            }

            cumulativeDeltaTime +=deltaTime;
            if(cumulativeDeltaTime > tickTime)
            {
                Ticks = (int)(cumulativeDeltaTime / tickTime);
                cumulativeDeltaTime %= tickTime;
            }

            editorState.Update(mapData, deltaTime);

            CameraPosition = editorState.CameraPosition;

        }
        public static MapData mapData = new MapData();
        public static EditorState editorState = new EditorState();
        public static RealPoint CameraPosition = RealPoint.Zero;
        private static IntPoint lastSize = IntPoint.Zero;
        private static float Zoom = 2;
        public static Image Redraw(IntPoint size)
        {
            lastSize = size;
            var image = new Bitmap(size.X, size.Y);
            var g = Graphics.FromImage(image);
            var pixelCameraPosition = CameraPosition.Round();
            g.TranslateTransform(size.X / 2, size.Y / 2);

            g.ScaleTransform(Zoom, Zoom);

            g.TranslateTransform(- pixelCameraPosition.X,- pixelCameraPosition.Y);

            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            Drawer.Redraw(g, mapData, editorState);

            g.Dispose();

            return image;
        }
        public static IntPoint ScreenToMap(IntPoint cursor)
        {
            return (IntPoint)(((RealPoint)(cursor - lastSize / 2) / Zoom + CameraPosition) / Meta.panelData.GridStep).Round() * Meta.panelData.GridStep;
        }
    }
}
