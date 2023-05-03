using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowInTheNight
{
    partial class GameForm : Form
    {
        public GameForm()
        {
            Instance = this;
            InitializeComponent();
            testLabel.Font = Drawer.gameFontMain; 
            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;
        }
        private void GameForm_Activated(object sender, EventArgs e)
        {
            Cursor.Hide();
#if DEBUG
            return;
#endif
            TopMost = true;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            Meta.StartMainLoop(this);
            Meta.GoFullscreen(this);
        }
        private void GameForm_Deactivate(object sender, EventArgs e)
        {
            Cursor.Show();
            TopMost = false;
        }

        void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            Meta.InputController.OnKeyUp(key);
        }

        void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            Meta.InputController.OnKeyDown(key);
#if DEBUG
            if(key == Keys.F3)
            {
                GameState.ElfMode = !GameState.ElfMode;
            }
            if (key == Keys.F4)
            {
                GameState.ShamanMode = !GameState.ShamanMode;
            }
            if(key == Keys.F5)
            {
                Core.Teleport();
            }
            if (key == Keys.F6)
            {
                Core.RushDarkDoctor();
            }
            if(key == Keys.F7)
            {
                Core.GiveAllItems();
            }
            if (key == Keys.F8)
            {
                TextWorks.Save();
            }
#endif
        }

        public static int FitWidth = 400;
        public IntPoint GetImageSize()
        {
            if (gameScreen.Width <= 0 || gameScreen.Height <= 0)
                return new IntPoint(1, 1);

            //var size = (IntPoint)gameScreen.Size;
            var bounds = Screen.PrimaryScreen.Bounds;
            var h = TryFitAll(bounds.Height);
            var w = h < 320 ? 320 : h;
            return new IntPoint(w, h);

        }

        private int TryFitAll(int screenSize)
        {
            int topOffsetBig;
            int topOffsetSmall;
            int bottomOffsetBig;
            int bottomOffsetSmall;
            int sizeBig = TryFitAll(screenSize, true, out topOffsetBig, out bottomOffsetBig);
            int sizeSmall = TryFitAll(screenSize, false, out topOffsetSmall, out bottomOffsetSmall);

            this.topFraming.Height = (int)(topOffsetSmall + (topOffsetBig - topOffsetSmall) * Core.multiplier);
            this.bottomFraming.Height = (int)(bottomOffsetSmall + (bottomOffsetBig - bottomOffsetSmall) * Core.multiplier);

            return Core.BigMode ? sizeBig : sizeSmall;
        }
        
        private int TryFitAll(int screenSize, bool bigMode, out int topOffset, out int bottomOffset)
        {
            var maxWantedSize = (int)(600);
            var minWantedSize = (int)(screenSize > 1200? 540 : 450);

            var maxMultiplier = (int)Math.Ceiling((screenSize + 0.0) / maxWantedSize);
            var size = screenSize / maxMultiplier;
            if (screenSize / maxMultiplier < minWantedSize)
            {
                maxMultiplier -= 1;
                size = maxWantedSize;
            }

            if (!bigMode)
            {
                size /= 3;
                size *= 2;
                if (maxMultiplier % 2 == 0)
                {
                    maxMultiplier /= 2;
                    maxMultiplier *=3;
                }
            }

            var offset = screenSize - size * maxMultiplier;
            topOffset = offset / 2;
            bottomOffset = offset - topOffset;

            if (bigMode)
            {
                size /= 3;
                size *= 2;
            }
            return size;
        }

        public void UpdateImage(Image image)
        {
            var previousImage = this.gameScreen.Image;
            this.gameScreen.Image = image;
            if (previousImage != null)
                previousImage.Dispose();
        }

        internal void UpdateFpsLabel(string text)
        {
            this.fpsLabel.Visible = true;
            this.fpsLabel.Text = text;
        }

        public bool WasClosed = false;
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SoundWorks.Close();
            WasClosed = true;
        }
        private static GameForm Instance;

        private delegate IntPoint MeasureStringDelegate(String text, Font font, int maxWidth);
        public IntPoint _MeasureString(String text, Font font, int maxWidth)
        {
            testLabel.Text = text;
            testLabel.MaximumSize = new System.Drawing.Size(maxWidth, 0);
            var width = testLabel.Size.Width;
            var height = testLabel.Size.Height;
            return new IntPoint(width, height);
        }
        public static IntPoint MeasureStringViaLabel (String text, Font font, int maxWidth)
        {
            var delegatedMeasureString = new MeasureStringDelegate(Instance._MeasureString);
            var result = Instance.Invoke(delegatedMeasureString, text, font, maxWidth);
            return (IntPoint)result;
        }

        public static void CloseForm()
        {
            Instance.Invoke(new Action(Instance.Close));
        }
    }
}
