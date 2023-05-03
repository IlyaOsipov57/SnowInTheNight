using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowInTheNight
{
    static class Core
    {
        private static double cumulativeDeltaTime = 0;
        private static double tickTime = 0.04;
        public static int Ticks = 0;
        public static void Update(double deltaTime)
        {
            if (gameState.GameEndTimer < 11)
                SoundWorks.PlaySomeSnowSound(deltaTime);

            GameSaveManager.TakeActions(ref mapState, gameState);
            
            cumulativeDeltaTime +=deltaTime;
            if(cumulativeDeltaTime > tickTime)
            {
                Ticks = (int)(cumulativeDeltaTime / tickTime);
                cumulativeDeltaTime %= tickTime;
            }

            UpdateFrameMultiplier(deltaTime);

            Meta.InputController.Update(deltaTime);

            gameState.Update(mapState, deltaTime);
         
            CameraPosition = gameState.CameraPosition;

        }
        private static double multiplierUpdateDelayTimer;
        private static double multiplierUpdateDelayValue = 0.7;
        private static void UpdateFrameMultiplier(double deltaTime)
        {
            var targetMultiplier = BigMode ? 1.0 : 0.0;


            if(_multiplier == targetMultiplier)
            {
                multiplierUpdateDelayTimer = multiplierUpdateDelayValue;
            }
            else
            {
                multiplierUpdateDelayTimer -= deltaTime;
            }
            if (multiplierUpdateDelayTimer > 0)
                return;

            for (int i = 0; i < Ticks; i++)
            {
                var delta = targetMultiplier - _multiplier;
                _multiplier += delta * 0.1;
            }
            if (_multiplier >= 0.99)
                _multiplier = 1.0;
            if (_multiplier <= 0.01)
                _multiplier = 0.0;
        }

        public static RealPoint CameraPosition = RealPoint.Zero;
        private static GameState gameState = new GameState();
        private static MapState mapState = new MapState();
        public static bool MapMode = false;
        public static bool BigMode = false;
        public static bool IsBigModeStable ()
        {
            var targetMultiplier = BigMode ? 1.0 : 0.0;
            return multiplier == targetMultiplier;
        }
        public static double multiplier
        {
            get
            {
                if (MapMode)
                    return 0;
                return _multiplier;
            }
            set
            {
                _multiplier = value;
            }
        }
        private static double _multiplier = 0;
        public static Image Redraw(IntPoint size)
        {
            size = ((RealPoint)size * (1+multiplier/2)).Round();

            var image = new Bitmap(size.X, size.Y);
            var g = Graphics.FromImage(image);
            var pixelCameraPosition = CameraPosition.Round();
            g.TranslateTransform(size.X / 2, size.Y / 2);

            if (MapMode)
            {
                var k = (float)(1.0 / mapModeMultiplier);
                g.ScaleTransform(k,k);
            }

            g.TranslateTransform(- pixelCameraPosition.X,- pixelCameraPosition.Y);
            var clipSize = GetClipSize(size);
            g.Clip = new Region(new Rectangle((Point)(pixelCameraPosition - clipSize / 2), (Size)clipSize));


            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            var ig = Graphics.FromImage(image);
            ig.TranslateTransform(0, size.Y);

            ig.SmoothingMode = SmoothingMode.None;
            ig.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            Drawer.Redraw(g, ig, gameState, mapState, size);

            g.Dispose();

            return image;
        }
        public static double mapModeMultiplier = 2;
        private static IntPoint GetClipSize(IntPoint size)
        {
            IntPoint clipSize = (size + new IntPoint(10, 10));
            if (MapMode)
            {
                clipSize = ((RealPoint)size * mapModeMultiplier).Round();
            }
            else
            {
                clipSize = (((RealPoint)clipSize) * (1 + multiplier / 2)).Round();
            }
            return clipSize;
        }
        public static String GetTimeAndFPS()
        {
            var fps = gameState.FPS >= 0 ? "" + gameState.FPS : "??";
            return TimeToString(gameState.SecondsFromStart) + "\r\n" + fps;
        }
        public static String TimeToString (int secondsFromStart)
        {
            var minutes = secondsFromStart / 60;
            var seconds = secondsFromStart % 60;
            return minutes + (seconds > 9 ? ":" : ":0") + seconds;
        }

        public static LoadMenu LoadGameMenu = new LoadMenu();
        public static ExitMenu ExitGameMenu = new ExitMenu();
        
        public static void OnLoad()
        {
            gameState = new GameState();

            foreach(var interaction in mapState.Interactions.Where(i => i is FirePlace))
            {
                (interaction as FirePlace).OnLoad();
            }

            MapMode = false;
            BigMode = mapState.LanternIsOn_SaveValue;
            _multiplier = BigMode ? 1 : 0;
        }
#if DEBUG
        public static void Teleport()
        {
            mapState.PlayerPosition = new RealPoint(300, -4000);
        }

        internal static void RushDarkDoctor()
        {
            mapState.DarkDoctor.SpeedPercentage = 1000;
        }

        internal static void GiveAllItems()
        {
            mapState.AddItem(new ItemMap(), gameState);
            mapState.AddItem(new ItemLantern(), gameState);
            mapState.AddItem(new ItemBell(), gameState);
            mapState.AddItem(new ItemBucket() { Uses = 2 }, gameState);
        }
#endif
    }
}
