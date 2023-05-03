using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SnowInTheNight
{
    [Serializable]
    class TextBubble
    {
        public TextBubble (RealPoint position, String text)
        {
            this.Position = position;
            this.Text = text;
        }
        public RealPoint Position;
        public int TextIndex;
        public String Text
        {
            get
            {
                return TextWorks.GetText(TextIndex);
            }
            set
            {
                TextIndex = TextWorks.GetTextIndex(value);
            }
        }

        public int SymbolsVisible = int.MaxValue;
        public RealPoint Alignment = new RealPoint(0.5,0.5);
        public static String SilentSymbol = "#";
        public static double printingSpeed = 20;
        public int MaxWidth = 280;
        private Rectangle FixForDraw (Rectangle rectangleForFill)
        {
            return new Rectangle(rectangleForFill.X, rectangleForFill.Y, rectangleForFill.Width - 1, rectangleForFill.Height - 1);
        }
        public void DrawMarker(Graphics g)
        {
            var text = "Х";

            var size = SizeAction;
            var shift = new IntPoint((int)(size.X * Alignment.X),(int)(size.Y * Alignment.Y));
            var position = Position.Round() - shift;
            var offset = IntPoint.Zero;
            offset -= new IntPoint(1, 0);
            var b = new SolidBrush(Color.FromArgb(192, Color.White));

            var r1 = new Rectangle((Point)position, (Size)size);
            var r2 = new Rectangle((Point)(position + offset), (Size)size);

            g.FillRectangle(b, r1);
            g.DrawString(text, Drawer.gameFontMain, Brushes.Black, r2);
            g.DrawRectangle(Pens.Black, FixForDraw(r1));
        }
        public void DrawTextBubble(Graphics g)
        {
            var text = Text;
            var leftover = "";
            if (text.Length > SymbolsVisible)
            {
                leftover = text.Substring(SymbolsVisible);
                text = text.Remove(SymbolsVisible);
            }
            text = text.Replace(SilentSymbol, "");
            leftover = leftover.Replace(SilentSymbol, "");

            leftover = Regex.Replace(leftover, @"[^\s]", "_");
            text += leftover;

            // dirty hack:
            // measure string by creating a label with text in the main form and getting its size
            // this requires invokation on main thread, so it slows everything down, but the result is correct
            var size = GameForm.MeasureStringViaLabel(text, Drawer.gameFontMain, MaxWidth);
            size -= new IntPoint(0, 7);
            size += new IntPoint(0, SizeAction.Y / 2);
            var shift = new IntPoint((int)(size.X * Alignment.X), (int)(size.Y * Alignment.Y));
            var position = Position.Round() - shift;
            var offset = IntPoint.Zero;
            offset -= new IntPoint(1, 0);
            var b = new SolidBrush(Color.FromArgb(192, Color.White));

            var r1 = new Rectangle((Point)position, (Size)size);
            var r2 = new Rectangle((Point)(position + offset), (Size)size);

            g.FillRectangle(b, r1);
            g.DrawString(text, Drawer.gameFontMain, Brushes.Black, r2);
        }
        public void DrawActiveTextBubble(Graphics g)
        {
            var text = Text;
            var leftover = "";
            if (text.Length > SymbolsVisible)
            {
                leftover = text.Substring(SymbolsVisible);
                text = text.Remove(SymbolsVisible);
            }
            text = text.Replace(SilentSymbol, "");
            leftover = leftover.Replace(SilentSymbol, "");

            leftover = Regex.Replace(leftover, @"[^\s]", "_");
            text += leftover;

            // dirty hack:
            // measure string by creating a label with text in the main form and getting its size
            // this requires invokation on main thread, so it slows everything down, but the result is correct
            var size = GameForm.MeasureStringViaLabel(text, Drawer.gameFontMain, MaxWidth);
            size -= new IntPoint(0, 7);
            size += new IntPoint(0, SizeAction.Y / 2);
            var shift = new IntPoint((int)(size.X * Alignment.X), (int)(size.Y * Alignment.Y));
            var position = Position.Round() - shift;
            var offset = IntPoint.Zero;
            offset -= new IntPoint(1, 0);
            var b = new SolidBrush(Color.FromArgb(192, Color.White));

            var r1 = new Rectangle((Point)position, (Size)size);
            var r2 = new Rectangle((Point)(position + offset), (Size)size);

            g.FillRectangle(b, r1);
            g.DrawString(text, Drawer.gameFontMain, Brushes.Black, r2);
            g.DrawRectangle(Pens.Black, FixForDraw(r1));
        }
        private static IntPoint SizeAction = new IntPoint(12,14);
        public void DrawInteractiveTextBubble(Graphics g)
        {
            var text = Text;
            var leftover = "";
            if (text.Length > SymbolsVisible)
            {
                leftover = text.Substring(SymbolsVisible);
                text = text.Remove(SymbolsVisible);
            }
            text = text.Replace(SilentSymbol, "");
            leftover = leftover.Replace(SilentSymbol, "");

            leftover = Regex.Replace(leftover, @"[^\s]", "_");
            text += leftover;

            // dirty hack:
            // measure string by creating a label with text in the main form and getting its size
            // this requires invokation on main thread, so it slows everything down, but the result is correct
            var size = GameForm.MeasureStringViaLabel(text, Drawer.gameFontMain, MaxWidth);
            size -= new IntPoint(0, 7);
            size += new IntPoint(0, SizeAction.Y / 2);
            var shift = new IntPoint((int)(size.X * Alignment.X), (int)(size.Y * Alignment.Y));
            var position = Position.Round() - shift;
            size -= new IntPoint(0, SizeAction.Y / 2);
            var offset = IntPoint.Zero;
            offset -= new IntPoint(1, 0);
            var b = new SolidBrush(Color.FromArgb(192, Color.White));

            var positionActionX = position.X + (size.X - SizeAction.X) * 9 / 10;
            var positionActionY = position.Y + size.Y;
            var positionAction = new IntPoint(positionActionX, positionActionY);

            var halfSizeAction = new IntPoint(SizeAction.X, SizeAction.Y / 2);
            var halfPositionAction = new IntPoint(positionAction.X, positionAction.Y + SizeAction.Y - halfSizeAction.Y);
            size = new IntPoint(size.X, size.Y + SizeAction.Y - halfSizeAction.Y);

            var ra1 = new Rectangle((Point)halfPositionAction, (Size)halfSizeAction);
            var ra2 = new Rectangle((Point)(positionAction + offset), (Size)SizeAction);
            var ra3 = new Rectangle((Point)positionAction, (Size)SizeAction);


            var r1 = new Rectangle((Point)position, (Size)size);
            var r2 = new Rectangle((Point)(position + offset), (Size)size);

            g.FillRectangle(b, r1);
            g.DrawString(text, Drawer.gameFontMain, Brushes.Black, r2);

            g.FillRectangle(b, ra1);
            g.DrawString("Х", Drawer.gameFontMain, Brushes.Black, ra2);
            g.DrawRectangle(Pens.Black, FixForDraw(ra3));
            r1 = FixForDraw(r1);
            var pts = new Point[]{
                new Point(ra1.Left,r1.Bottom),
                new Point(r1.Left,r1.Bottom),
                new Point(r1.Left,r1.Top),
                new Point(r1.Right,r1.Top),
                new Point(r1.Right,r1.Bottom),
                new Point(ra1.Right,r1.Bottom)
            };
            g.DrawLines(Pens.Black, pts);
        }
        public void DrawInventoryTextBubble(Graphics g)
        {
            var text = Text;

            // dirty hack:
            // measure string by creating a label with text in the main form and getting its size
            // this requires invokation on main thread, so it slows everything down, but the result is correct
            var size = GameForm.MeasureStringViaLabel(text, Drawer.gameFontMain, 310);
            size -= new IntPoint(0, 5);
            var shift = new IntPoint((int)(size.X * Alignment.X), (int)(size.Y * Alignment.Y));
            var position = Position.Round() - shift;
            var offset = IntPoint.Zero;
            offset -= new IntPoint(1, 0);
            

            var r1 = new Rectangle((Point)position, (Size)size);
            var r2 = new Rectangle((Point)(position + offset), (Size)size);

            g.FillRectangle(Brushes.White, r1);
            g.DrawString(text, Drawer.gameFontMain, Brushes.Black, r2);
            g.DrawRectangle(Pens.Black, FixForDraw(r1));
        }
    }
}
