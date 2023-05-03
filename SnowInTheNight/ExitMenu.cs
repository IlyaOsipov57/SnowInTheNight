using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    class ExitMenu
    {
        public void Draw(Graphics g, IntPoint center)
        {
            if (!open)
                return;

            var borderOffset = new IntPoint(5, 5);
            var textOffset = new IntPoint(0, 16);
            var widthSize = new IntPoint(144, 0);

            var size = borderOffset * 2 + textOffset * 3 + widthSize;

            var text = "Выйти из игры?\r\nX - подтвердить\r\nZ - вернуться";

            var shift = new IntPoint((int)(size.X * 0.5), (int)(size.Y * 0.5));
            var position = center - shift;

            var b2 = new SolidBrush(Color.FromArgb(192, 192, 192));

            var r1 = new Rectangle((Point)position, (Size)size);

            g.FillRectangle(b2, r1);
            g.DrawString(TextWorks.GetText(text), Drawer.gameFontMain, Brushes.Black, (Point)(position + borderOffset));

            g.DrawRectangle(Pens.Black, FixForDraw(r1));
        }
        private Rectangle FixForDraw(Rectangle rectangleForFill)
        {
            return new Rectangle(rectangleForFill.X, rectangleForFill.Y, rectangleForFill.Width - 1, rectangleForFill.Height - 1);
        }
        bool open = false;
        public bool IsOpen()
        {
            return open;
        }
        public void Open()
        {
            open = true;
        }
        public void Update(double deltaTime)
        {
            if (!open)
                return;
            if (Meta.InputController.InventoryButton.Read())
            {
                open = false;
                return;
            }
            if (Meta.InputController.InteractionButton.Read())
            {
                if (GameSaveManager.IsClosing())
                    return;
                GameSaveManager.Enqueue(new GameSaveManager.AutosaveAction());
                GameSaveManager.Close();
                return;
            }
        }

        internal void Close()
        {
            open = false;
        }
    }
}
