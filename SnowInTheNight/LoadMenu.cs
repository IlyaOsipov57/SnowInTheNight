using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    public class LoadMenuData
    {
        public class LoadMenuItem
        {
            public Image Image;

            public String Name;

            public Action OnChosen;
        }

        public LoadMenuItem[] Items;
        public String Hint;
        public bool CanExit;
        public Action OnExit;
    }

    class LoadMenu
    {
        private bool IsOpened;
        private LoadMenuData Data;

        private int selected = 0;
        private int selectionShift = 0;
        public void Draw(Graphics g, IntPoint center)
        {
            if (!IsOpened)
                return;

            var itemsVisible = Math.Min(Data.Items.Length, 3);
            var borderOffset = new IntPoint(5, 5);
            var textOffset = new IntPoint(0, 16);
            var itemSize = new IntPoint(144, 48);
            var itemPadding = new IntPoint(2, 2);
            var itemSpace = new IntPoint(0, 6);
            var itemsArea = new IntPoint(
                (itemSize + 2 * itemPadding).X,
                ((itemSize + 2 * itemPadding + textOffset + itemSpace) * itemsVisible).Y);

            var size = itemsArea + borderOffset * 2 + textOffset * 2;

            var text = Data.Hint;

            var shift = new IntPoint((int)(size.X * 0.5), (int)(size.Y * 0.5));
            var position = center - shift;

            var b1 = new SolidBrush(Color.FromArgb(125, Color.White));
            var b2 = new SolidBrush(Color.FromArgb(192, 192, 192));

            var r1 = new Rectangle((Point)position, (Size)size);

            g.FillRectangle(b1, r1);

            g.FillRectangle(b2, new Rectangle((Point)(position + borderOffset + new IntPoint(0, itemsArea.Y) - new IntPoint(0, itemPadding.Y)), new Size(itemSize.X + 2 * itemPadding.X, textOffset.Y * 2 + 2 * itemPadding.X)));
            g.DrawString(TextWorks.GetText(text), Drawer.gameFontMain, Brushes.Black, (Point)(position + borderOffset + new IntPoint(0, itemsArea.Y) - new IntPoint(0, itemPadding.Y)));

            for (int i = 0; i < itemsVisible; i++)
            {
                var itemPosition = position + borderOffset + itemPadding + new IntPoint(0, (itemSpace + itemPadding * 2 + textOffset + itemSize).Y) * i;

                g.FillRectangle(b2, new Rectangle((Point)(itemPosition - itemPadding), (Size)(itemSize + 2 * itemPadding + textOffset)));
                g.DrawImageUnscaled(Data.Items[selectionShift + i].Image, (Point)itemPosition);
                g.DrawString(Data.Items[selectionShift + i].Name, Drawer.gameFontMain, Brushes.Black, (Point)(itemPosition + new IntPoint(0, itemSize.Y)));
            }

            if (itemsVisible > 0)
            {
                var selectedItemPosition = position + borderOffset + new IntPoint(0, (itemSpace + itemPadding * 2 + textOffset + itemSize).Y) * selected;
                var r2 = new Rectangle((Point)selectedItemPosition, (Size)(itemSize + textOffset + 2 * itemPadding));
                g.DrawRectangle(Pens.Black, FixForDraw(r2));
            }

            g.DrawRectangle(Pens.Black, FixForDraw(r1));
        }
        private Rectangle FixForDraw(Rectangle rectangleForFill)
        {
            return new Rectangle(rectangleForFill.X, rectangleForFill.Y, rectangleForFill.Width - 1, rectangleForFill.Height - 1);
        }

        public bool IsOpen()
        {
            return IsOpened;
        }
        public void Open(LoadMenuData data)
        {
            IsOpened = true;
            selected = 0;
            selectionShift = 0;
            Data = data;
        }
        private void Close()
        {
            Data = null;
            IsOpened = false;
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (!IsOpened)
                return;

            if (Meta.InputController.InventoryButton.Read())
            {
                if (Data.OnExit != null)
                    Data.OnExit();
                if (TryClose())
                    return;
            }
            if (Meta.InputController.InteractionButton.Read())
            {
                var selectedItem = selected + selectionShift;
                if (TryChoose(selectedItem))
                    return;
            }
            var shift = Meta.InputController.LoadMenuAxis.Read();
            UpdateSelectionPosition(shift);
        }

        private bool TryClose()
        {
            if (!Data.CanExit)
                return false;
            Close();
            return true;
        }

        private bool TryChoose(int selectedItem)
        {
            if (selectedItem < 0)
                return false;
            if (selectedItem >= Data.Items.Length)
                return false;
            if (Data.Items[selectedItem].OnChosen != null)
                Data.Items[selectedItem].OnChosen();
            Close();
            return true;
        }

        void UpdateSelectionPosition(int shift)
        {
            if (shift == 0)
                return;
            if (shift == 1)
            {
                if (selectionShift + selected + 1 == Data.Items.Length)
                    return;
                if (selected < 2)
                {
                    selected++;
                    return;
                }
                selectionShift++;
                return;
            }
            if (shift == -1)
            {
                if (selected + selectionShift == 0)
                    return;
                if (selected > 0)
                {
                    selected--;
                    return;
                }
                selectionShift--;
                return;
            }
        }
    }
}
