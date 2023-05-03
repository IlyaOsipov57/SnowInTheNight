using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowMapEditor
{
    static class Drawer
    {
        internal static void Redraw(System.Drawing.Graphics g, MapData mapData, EditorState editorState)
        {
            g.Clear(Color.Gray);

            foreach (var data in mapData.Data.Where(d => d is Road))
            {
                (data as Road).PreDraw1(g);
            }
            foreach (var data in mapData.Data.Where(d => d is Road))
            {
                (data as Road).PreDraw2(g);
            }
            foreach (var data in mapData.Data)
            {
                data.Draw(g);
            }

            if (editorState.SelectedItem != null && editorState.SelectedItem.Polyline.Count != 0)
                editorState.SelectedItem.Draw(g, true);

            g.FillEllipse(Brushes.Blue, editorState.CursorMarker.X - 2, editorState.CursorMarker.Y - 2, 3, 3);
            SnowInTheNight.Hook.DrawPlayer(g, new SnowInTheNight.RealPoint(0, 0));
        }
    }
}
