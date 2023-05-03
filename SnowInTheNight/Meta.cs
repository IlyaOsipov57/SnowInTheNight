using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowInTheNight
{
    static class Meta
    {
        public static Controller InputController = new Controller();

        private static Thread MainLoop;
        private static bool Running = true;
        private static GameForm Invoker;
        private static bool started = false;
        public static void StartMainLoop(GameForm gameForm)
        {
            if (started)
                return;
            started = true;
            Invoker = gameForm;
            MainLoop = new Thread(Run);
            MainLoop.Start();
        }
        private static void Run()
        {
            try
            {
                LoadMenuData data;
                if (GameSaveManager.GetSavesMenu(out data))
                {
                    Core.LoadGameMenu.Open(data);
                }
                var lastTime = DateTime.Now;
                while (Running)
                {
                    var deltaTime = (DateTime.Now - lastTime).TotalMilliseconds / 1000;
                    lastTime = DateTime.Now;
                    RunCycle(deltaTime);
                }
            }
            catch (Exception e)
            {
                try
                {
                    if (!(Invoker as GameForm).WasClosed)
                    {
                        Program.ErrorLog(e);
                        Thread.Sleep(1000);
                        Invoker.Invoke(new Action(Invoker.Close));
                    }
                }
                catch { }
            }
        }
        private delegate void UpdateImageDelegate(Image image);
        private delegate void UpdateLabelDelegate(String text);
        private delegate IntPoint GetImageSizeDelegate();

        private static void RunCycle(double deltaTime)
        {
            Core.Update(deltaTime);

            var delegatedGetImageSize = new GetImageSizeDelegate(Invoker.GetImageSize);
            var size = (IntPoint)Invoker.Invoke(delegatedGetImageSize);

            var image = Core.Redraw(size);

            var delegatedUpdateImage = new UpdateImageDelegate(Invoker.UpdateImage);
            Invoker.Invoke(delegatedUpdateImage, image);
#if DEBUG
            var delegatedUpdateLabel = new UpdateLabelDelegate(Invoker.UpdateFpsLabel);
            Invoker.Invoke(delegatedUpdateLabel, Core.GetTimeAndFPS());
#endif
        }
        public static void GoFullscreen(Form form)
        {
            form.WindowState = FormWindowState.Normal;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Bounds = Screen.PrimaryScreen.Bounds;
        }

        public class Controller
        {
            internal static object InputLock = new object();
            public UAxisForgetful Direction = new UAxisForgetful(Keys.Left, Keys.Right, Keys.Up, Keys.Down);
            public UClick InteractionButton = new UClick(Keys.X);
            public UClick InventoryButton = new UClick(Keys.Z);
            public UClick TeleportButton = new UClick(Keys.C);
            public UClick InputsButton = new UClick(Keys.F2);
            public UClick LanguageButton = new UClick(Keys.F1);
            public UItemAxis ItemAxis = new UItemAxis(Keys.Left, Keys.Right);
            public UItemAxis LoadMenuAxis = new UItemAxis(Keys.Up, Keys.Down);
            public UMapAxis MapAxis = new UMapAxis(Keys.Left,Keys.Right,Keys.Up,Keys.Down);

            public UButtonView ViewUp = new UButtonView(Keys.Up);
            public UButtonView ViewLeft = new UButtonView(Keys.Left);
            public UButtonView ViewDown = new UButtonView(Keys.Down);
            public UButtonView ViewRight = new UButtonView(Keys.Right);

            public void OnKeyDown(Keys key)
            {
                lock (InputLock)
                {
                    Direction.KeyDown(key);
                    InteractionButton.KeyDown(key);
                    InventoryButton.KeyDown(key);
                    TeleportButton.KeyDown(key);
                    ItemAxis.KeyDown(key);
                    LoadMenuAxis.KeyDown(key);
                    MapAxis.KeyDown(key);
                    InputsButton.KeyDown(key);
                    LanguageButton.KeyDown(key);

                    ViewUp.KeyDown(key);
                    ViewLeft.KeyDown(key);
                    ViewDown.KeyDown(key);
                    ViewRight.KeyDown(key);
                }
            }
            public void OnKeyUp(Keys key)
            {
                lock (InputLock)
                {
                    Direction.KeyUp(key);
                    InteractionButton.KeyUp(key);
                    InventoryButton.KeyUp(key);
                    TeleportButton.KeyUp(key);
                    ItemAxis.KeyUp(key);
                    LoadMenuAxis.KeyUp(key);
                    MapAxis.KeyUp(key);
                    InputsButton.KeyUp(key);
                    LanguageButton.KeyUp(key);

                    ViewUp.KeyUp(key);
                    ViewLeft.KeyUp(key);
                    ViewDown.KeyUp(key);
                    ViewRight.KeyUp(key);
                }
            }
            public void Accept (Keys key)
            {
                ViewUp.Accept(key);
                ViewLeft.Accept(key);
                ViewDown.Accept(key);
                ViewRight.Accept(key);
            }
            public void Update(double deltaTime)
            {
                lock (InputLock)
                {
                    Direction.Update(deltaTime);
                    InteractionButton.Update(deltaTime);
                    InventoryButton.Update(deltaTime);
                    TeleportButton.Update(deltaTime);
                    ItemAxis.Update(deltaTime);
                    LoadMenuAxis.Update(deltaTime);
                    MapAxis.Update(deltaTime);
                    InputsButton.Update(deltaTime);
                    LanguageButton.Update(deltaTime);

                    ViewUp.Update(deltaTime);
                    ViewLeft.Update(deltaTime);
                    ViewDown.Update(deltaTime);
                    ViewRight.Update(deltaTime);
                }
            }
        }
    }
}
