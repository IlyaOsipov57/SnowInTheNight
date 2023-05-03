using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnowMapEditor
{
    static class Meta
    {
        public static Controller InputController = new Controller();
        public static PanelData panelData = new PanelData();

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
                var lastTime = DateTime.Now;
                while (Running)
                {
                    var deltaTime = (DateTime.Now - lastTime).TotalMilliseconds / 1000;
                    lastTime = DateTime.Now;
                    RunCycle(deltaTime);
                    GC.Collect();
                }
            }
            catch (Exception e)
            {
                try
                {
                    Invoker.Invoke(new Action(Invoker.Close));
                }
                catch { }
            }
        }
        private delegate void UpdateImageDelegate(Image image);
        private delegate void UpdateLabelDelegate(String text);
        private delegate PanelData ToolPanelUpdateDelegate(PanelData data);
        private delegate IntPoint GetCursorPositionDelegate();
        private delegate IntPoint GetImageSizeDelegate();

        private static void RunCycle(double deltaTime)
        {
            InputController.Update(deltaTime);

            Core.Update(deltaTime);

            var delegatedToolPanelUpdate = new ToolPanelUpdateDelegate(Invoker.UpdateToolPanel);
            panelData = (PanelData)Invoker.Invoke(delegatedToolPanelUpdate, panelData);

            var delegatedGetImageSize = new GetImageSizeDelegate(Invoker.GetImageSize);
            var size = (IntPoint)Invoker.Invoke(delegatedGetImageSize);

            var image = Core.Redraw(size);

            var delegatedUpdateImage = new UpdateImageDelegate(Invoker.UpdateImage);
            Invoker.Invoke(delegatedUpdateImage, image);
        }

        public static IntPoint GetCursorPosition ()
        {
            var delegatedGetCursorPosition = new GetCursorPositionDelegate(Invoker.GetCursorPosition);
            return (IntPoint)Invoker.Invoke(delegatedGetCursorPosition);
        }

        public static void GoFullscreen(Form form)
        {
            form.WindowState = FormWindowState.Normal;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Bounds = Screen.PrimaryScreen.Bounds;
        }

        public class Controller
        {
            public UMouseClick LeftClick = new UMouseClick(MouseButtons.Left);
            public UMouseClick RightClick = new UMouseClick(MouseButtons.Right);
            public UClick InteractionButton = new UClick(Keys.X);
            public UClick InventoryButton = new UClick(Keys.Z);
            public UMapAxis MapAxis = new UMapAxis(Keys.Left,Keys.Right,Keys.Up,Keys.Down);
            public IntPoint CursorPosition = IntPoint.Zero;
            public void OnKeyDown(Keys key)
            {
                InteractionButton.KeyDown(key);
                InventoryButton.KeyDown(key);
                MapAxis.KeyDown(key);
            }
            public void OnKeyDown(MouseButtons key)
            {
                LeftClick.KeyDown(key);
                RightClick.KeyDown(key);
            }
            public void OnKeyUp(Keys key)
            {
                InteractionButton.KeyUp(key);
                InventoryButton.KeyUp(key);
                MapAxis.KeyUp(key);
            }
            public void OnKeyUp(MouseButtons key)
            {
                LeftClick.KeyUp(key);
                RightClick.KeyUp(key);
            }
            public void Update(double deltaTime)
            {
                CursorPosition = Core.ScreenToMap(GetCursorPosition());
                InteractionButton.Update(deltaTime);
                InventoryButton.Update(deltaTime);
                MapAxis.Update(deltaTime);
                LeftClick.Update(deltaTime);
                RightClick.Update(deltaTime);
            }
        }
    }
}
