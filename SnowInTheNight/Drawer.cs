using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace SnowInTheNight
{
    static class Drawer
    {
        private static PrivateFontCollection privateFontCollection1 = new PrivateFontCollection();
        private static PrivateFontCollection privateFontCollection2 = new PrivateFontCollection();
        public static Font gameFontMain;
        public static Font gameFontSloppy;
        public static Font gameFontSloppyMap;
        public static Font gameFontSloppySmall;
        public static TextureBrush PavementBrush; 
        static Drawer ()
        {
            //gameFontMain = MakeFont(privateFontCollection1, SnowWalkingTest.Properties.Resources.PressStart2P_Regular, 6f);

            //gameFontMain = MakeFont(privateFontCollection1, SnowWalkingTest.Properties.Resources.basic33NoUnderscore, 11.65f);
            //gameFontSloppy = MakeFont(privateFontCollection2, SnowWalkingTest.Properties.Resources.Kurochkalapkoi, 48);
            //gameFontSloppyMap = MakeFont(privateFontCollection2, SnowWalkingTest.Properties.Resources.Kurochkalapkoi, 33);
            //gameFontSloppySmall = MakeFont(privateFontCollection2, SnowWalkingTest.Properties.Resources.Kurochkalapkoi, 16);

            gameFontMain = MakeFont(privateFontCollection1, SnowInTheNight.Properties.Resources.basic33NoUnderscore, 15.5333333f);
            gameFontSloppy = MakeFont(privateFontCollection2, SnowInTheNight.Properties.Resources.Kurochkalapkoi, 64);
            gameFontSloppyMap = MakeFont(privateFontCollection2, SnowInTheNight.Properties.Resources.Kurochkalapkoi, 44);
            gameFontSloppySmall = MakeFont(privateFontCollection2, SnowInTheNight.Properties.Resources.Kurochkalapkoi, 21.3333333f);

            PavementBrush = new TextureBrush(SnowInTheNight.Properties.Resources.tile);
        }

        private static Font MakeFont(PrivateFontCollection privateFontCollection, byte[] fontBytes, float size)
        {
            var fontData = Marshal.AllocCoTaskMem(fontBytes.Length);
            Marshal.Copy(fontBytes, 0, fontData, fontBytes.Length);
            privateFontCollection.AddMemoryFont(fontData, fontBytes.Length);

            return new System.Drawing.Font(privateFontCollection.Families[0], size, FontStyle.Regular, GraphicsUnit.Pixel);
        }




        public static void Redraw(Graphics g, Graphics ig, GameState gameState, MapState mapState, IntPoint size, bool renderForSaveGameIcon = false)
        {
            if(GameState.InStartScreen && !renderForSaveGameIcon)
            {
                DrawStartScreen(g, gameState);
                return;
            }

            g.Clear(Color.Gray);

#if DEBUG
            if (GameState.ShamanMode)
            {
                DrawAntiColliders(g, mapState);
            }
#endif

            var playerDrawer = new PlayerDrawer(mapState.LeftLeg, mapState.RightLeg)
            {
                Health = mapState.PlayerHealth,
                IsLeftStep = gameState.IsLeftStep,
                HasLantern = Core.BigMode
            };

            var darkDoctorDrawer = (PlayerDrawer) null;
            if (mapState.DarkDoctor != null)
            {
                darkDoctorDrawer = mapState.DarkDoctor.GetDoctorDrawer();
            }

            var allSteps = mapState.Steps.Concat(playerDrawer.GetExtraSteps());
            if(darkDoctorDrawer != null)
                allSteps = allSteps.Concat(darkDoctorDrawer.GetExtraSteps());

            DrawRoads(g, mapState);

            DrawSteps(g, allSteps);

            DrawPlayersAndDecorations(g, mapState, gameState, size, playerDrawer, darkDoctorDrawer);

            if (!Core.MapMode && !gameState.GameEnds)
            {
                foreach (var snowflake in gameState.Snowflakes)
                {
                    DrawSnowflake(g, snowflake);
                }
            }

            if (!renderForSaveGameIcon)
            {
                DrawInteractions(g, mapState);
            }
            
            if (Core.MapMode)
            {
                DrawFog(g, mapState, gameState);
            }


            if (!renderForSaveGameIcon)
            {
                DrawInventory(ig, mapState, gameState);
                Core.LoadGameMenu.Draw(ig, new IntPoint(size.X / 2, -size.Y / 2));
                Core.ExitGameMenu.Draw(ig, new IntPoint(size.X / 2, -size.Y / 2));
            }

            if (gameState.GameEnds)
            {
                {
                    var op = 64 * gameState.GameEndTimer;
                    op = Math.Min(255,Math.Max(0,op));
                    var b = new SolidBrush(Color.FromArgb((int)op,Color.Black));
                    ig.FillRectangle(b, 0, -size.Y, size.X, size.Y);
                }
                foreach (var snowflake in gameState.Snowflakes)
                {
                    DrawSnowflake(g, snowflake);
                }

                {
                    var op = 64 * (gameState.GameEndTimer - 4);
                    op = Math.Min(255, Math.Max(0, op));
                    var b = new SolidBrush(Color.FromArgb((int)op, Color.White));
                    
                    ig.DrawString("Mekagem 2019", gameFontSloppySmall, b, size.X / 2, -60);
                }
                {
                    var op = 64 * (gameState.GameEndTimer-8);
                    op = Math.Min(255, Math.Max(0, op));
                    var b = new SolidBrush(Color.FromArgb((int)op, Color.Black));
                    ig.FillRectangle(b, 0, -size.Y, size.X, size.Y);
                }
            }

            if (!renderForSaveGameIcon)
            {
                if (GameState.ShowInputs)
                    DrawTimeAndInputs(ig, new IntPoint(0, -size.Y), new IntPoint(size.X, -size.Y), mapState, gameState);
            }
        }

        private static void DrawTimeAndInputs(Graphics g, IntPoint topLeftCorner, IntPoint topRightCorner, MapState mapState, GameState gameState)
        {
            var b = new SolidBrush(Color.FromArgb(192, Color.White));
            g.DrawString(Core.TimeToString(gameState.SecondsFromStart), gameFontMain, b, (Point)(topLeftCorner + new IntPoint(0, 3)));

            if (gameState.GameEnds)
                return;

            var pW = topRightCorner + new IntPoint(-36, 2);
            var pA = topRightCorner + new IntPoint(-54, 20);
            var pS = topRightCorner + new IntPoint(-36, 20);
            var pD = topRightCorner + new IntPoint(-18, 20);

            var s = new Size(16, 16);

            var rW = new Rectangle((Point) pW, s);
            var rA = new Rectangle((Point)pA, s);
            var rS = new Rectangle((Point)pS, s);
            var rD = new Rectangle((Point)pD, s);

            if (GameState.InStartScreen)
            {
                DrawButton(g, rW, MapState.IntroDoctor.IsLeftStep && !MapState.IntroDoctor.needNewInput);
                DrawButton(g, rA, false);
                DrawButton(g, rS, false);
                DrawButton(g, rD, !MapState.IntroDoctor.IsLeftStep && !MapState.IntroDoctor.needNewInput);
            }
            else
            {
                DrawButton(g, rW, Meta.InputController.ViewUp);

                if (mapState != null && gameState.EndingPhase >= 2)
                {
                    DrawButton(g, rA, false);
                    DrawButton(g, rS, false);
                    DrawButton(g, rD, false);
                }
                else
                {
                    DrawButton(g, rA, Meta.InputController.ViewLeft);
                    DrawButton(g, rS, Meta.InputController.ViewDown);
                    DrawButton(g, rD, Meta.InputController.ViewRight);
                }
            }
        }
        private static void DrawButton(Graphics g, Rectangle r, UButtonView view)
        {
            DrawButton(g, r, view.Read() && view.Accepted());
        }
        private static void DrawButton(Graphics g, Rectangle r, bool pressed)
        {
            var b = pressed ? new SolidBrush(Color.FromArgb(192, Color.White)) : new SolidBrush(Color.FromArgb(64, Color.White));

            g.FillRectangle(b, r);
            g.DrawRectangle(Pens.Black, FixForDraw(r));
        }

        private static void DrawRoads(Graphics g, MapState mapState)
        {
            var squaredVisionRadius = GetSquaredVisionRadius();
            foreach(var road in mapState.PavedRoad)
            {
                if (!road.Visible(Core.CameraPosition, squaredVisionRadius))
                    continue;
                road.DrawPavement(g);
            }
        }

        private static void DrawPlayersAndDecorations(Graphics g, MapState mapState, GameState gameState, IntPoint size, PlayerDrawer playerDrawer, PlayerDrawer darkDoctorDrawer)
        {
            if (gameState.GameEnds)
            {
                DrawDecorationsBetween(g, mapState, size, double.MinValue, double.MaxValue);
                return;
            }

            var yp = mapState.PlayerPosition.Y;
            if(darkDoctorDrawer == null)
            {
                DrawDecorationsBetween(g, mapState, size, double.MinValue, yp);
                playerDrawer.DrawPlayer(g);
                DrawDecorationsBetween(g, mapState, size, yp, double.MaxValue);
                return;
            }

            var yd = mapState.DarkDoctor.position.Y;
            if(yp < yd)
            {
                DrawDecorationsBetween(g, mapState, size, double.MinValue, yp);
                playerDrawer.DrawPlayer(g);
                DrawDecorationsBetween(g, mapState, size, yp, yd);
                darkDoctorDrawer.DrawPlayer(g);
                DrawDecorationsBetween(g, mapState, size, yd, double.MaxValue);
            }
            else
            {
                DrawDecorationsBetween(g, mapState, size, double.MinValue, yd);
                darkDoctorDrawer.DrawPlayer(g);
                DrawDecorationsBetween(g, mapState, size, yd, yp);
                playerDrawer.DrawPlayer(g);
                DrawDecorationsBetween(g, mapState, size, yp, double.MaxValue);
            }
        }

        private static void DrawStartScreen(Graphics g, GameState gameState)
        {
            g.Clear(Color.Gray);

            var doctorDrawer = MapState.IntroDoctor.GetDoctorDrawer();
            if (doctorDrawer != null)
            {
                var allSteps = MapState.IntroSteps.Concat(doctorDrawer.GetExtraSteps());
                doctorDrawer.DrawPlayer(g);
                DrawSteps(g, allSteps);

                if (GameState.ShowInputs)
                {
                    DrawTimeAndInputs(g, new IntPoint(-160, -150), new IntPoint(160, -150), null, gameState);
                }
            }

            //g.DrawRectangle(Pens.Black, -160, -160, 320, 320);

            if (GameState.StartScreenPhase == 0)
            {
                g.DrawString(TextWorks.GetText("Ночью снег,"), gameFontSloppy, Brushes.Black, TextWorks.Language == TextWorks.Lang.Russian ? -120 : -180, -110);

                //g.DrawImageUnscaled(SnowInTheNight.Properties.Resources.G, -33, -30);
                {
                    var text = TextWorks.GetText(" F1 - переключить язык") + "\r\n" +
                        TextWorks.GetText(GameState.ShowInputs ? " F2 - скрыть таймер" : " F2 - показать таймер");
                    var size = new IntPoint(216, 30);
                    var Position = new RealPoint(0, 115);
                    var Alignment = new RealPoint(0.5, 1);
                    var shift = new IntPoint((int)(size.X * Alignment.X), (int)(size.Y * Alignment.Y));
                    var position = Position.Round() - shift;
                    var b = new SolidBrush(Color.FromArgb(192, Color.White));
                    var r1 = new Rectangle((Point)position, (Size)size);
                    var r2 = new Rectangle((Point)(position), (Size)size);
                    g.FillRectangle(b, r1);
                    g.DrawString(text, Drawer.gameFontMain, Brushes.Black, r2);
                    g.DrawRectangle(Pens.Black, FixForDraw(r1));
                }
                {
                    var text = " Нажмите X, чтобы начать путь";
                    var actionText = "Х";
                    var Position = new RealPoint(0, 140);
                    var Alignment = new RealPoint(0.5, 1);
                    var SizeAction = new IntPoint(12, 14);
                    var size = new IntPoint(216, 23);// GameForm.MeasureStringViaLabel(text, Drawer.gameFontMain, MaxWidth);
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
                    var r2 = new Rectangle((Point)(position), (Size)size);

                    g.FillRectangle(b, r1);
                    g.DrawString(TextWorks.GetText(text), Drawer.gameFontMain, Brushes.Black, r2);

                    g.FillRectangle(b, ra1);
                    g.DrawString(actionText, Drawer.gameFontMain, Brushes.Black, ra2);
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
            }
            else
            {
                {
                    var size = new IntPoint(300, 100);
                    var Position = new RealPoint(0, 20);
                    var Alignment = new RealPoint(0.5, 0.5);
                    var shift = new IntPoint((int)(size.X * Alignment.X), (int)(size.Y * Alignment.Y));
                    var position = Position.Round() - shift;
                    var r1 = new Rectangle((Point)position, (Size)size);
                    var b = new SolidBrush(Color.FromArgb(Math.Min(192, (int)((6-3*GameState.StartScreenPhase)*128)), Color.Gray));
                    g.FillRectangle(b, r1);
                }
                {
                    var citation =
    @"Ибо мы как срубленные деревья зимой.
Кажется, что они повалились на снег и
готовы покатиться от лёгкого толчка.
Нет, сдвинуть их нельзя,
так крепко они примёрзли к земле.
Но даже и это - лишь видимость.";

                    var author = "Франц Кафка, ''Деревья''";
                    var format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    var b = new SolidBrush(Color.FromArgb(Math.Min(255, (int)((6-3*GameState.StartScreenPhase)*128)), Color.Black));
                    g.DrawString(TextWorks.GetText(citation), gameFontMain, b, 0, -30, format);
                    g.DrawString(TextWorks.GetText(author), gameFontSloppySmall, b, 0, 70);
                }
            }
            foreach (var snowflake in gameState.Snowflakes)
            {
                DrawSnowflake(g, snowflake);
            }
        }
        private static Rectangle FixForDraw(Rectangle rectangleForFill)
        {
            return new Rectangle(rectangleForFill.X, rectangleForFill.Y, rectangleForFill.Width - 1, rectangleForFill.Height - 1);
        }
        private static Point[] ConvertToCross(Rectangle rectangleForFill)
        {
            var rectangle = FixForDraw(rectangleForFill);
            var p1 = new IntPoint(rectangle.Left, rectangle.Top);
            var p2 = new IntPoint(rectangle.Right, rectangle.Bottom);
            var p3 = new IntPoint(rectangle.Left, rectangle.Bottom);
            var p4 = new IntPoint(rectangle.Right, rectangle.Top);
            return new Point[] { (Point)p1, (Point)p2, (Point)p3, (Point)p4 };
        }

        private static void DrawSteps(Graphics g,  IEnumerable<MapState.Step> allSteps)
        {
            var squaredVisionRadius = GetSquaredVisionRadius();
            DrawSteps(g, squaredVisionRadius, allSteps);
        }

        public static void DrawSteps(Graphics g, double squaredVisionRadius, IEnumerable<MapState.Step> allSteps)
        {
            foreach (var step in allSteps)
            {
                if (step.Position == RealPoint.Infinity)
                    continue;
                if (FastDistanceCheck(step.Position, Core.CameraPosition, squaredVisionRadius))
                    continue;
                //if ((step.Position - Core.CameraPosition).SquaredLength > squaredVisionRadius)
                //    continue;
                var stepEnd = step.Position + step.Direction * (MapState.StepLength / step.Direction.Length);
                g.DrawLine(Pens.Black, (Point)step.Position.Round(), (Point)stepEnd.Round());
            }
        }

        private static bool FastDistanceCheck (RealPoint A, RealPoint B, double squaredDistance)
        {
            var x = A.X - B.X;
            var y = A.Y - B.Y;
            return x * x + y * y > squaredDistance;
        }

        private static void DrawAntiColliders(Graphics g, MapState mapState)
        {
            foreach(var anticollider in mapState.Anticolliders)
            {
                anticollider.DrawAntiCollider(g);
            }
        }

        private static void DrawFog(Graphics g, MapState mapState, GameState gameState)
        {
            var center = (gameState.CameraPosition / 40).Round();
            for(int i = -30; i<=30;i++)
            {
                for (int j = -30; j <= 30; j++)
                {
                    var cell = center + new IntPoint(i, j);
                    if(!mapState.Visited.Contains(cell))
                    {
                        g.FillRectangle(Brushes.Black, cell.X * 40 - 20, cell.Y * 40 - 20, 40, 40);
                    }
                }
            }
        }
        private static void DrawInventory(Graphics ig, MapState mapState, GameState gameState)
        {
            if (!Core.MapMode)
            {
                if (gameState.IsInInventoryMode)
                    DrawInventoryOpen(ig, mapState, gameState);
                else
                    DrawInventoryClosed(ig, mapState, gameState);
            }
            else
            {
                var tb = new TextBubble(new RealPoint(3, -3), "Х - закрыть карту");
                tb.Alignment = new RealPoint(0, 1);
                tb.DrawInventoryTextBubble(ig);
            }
        }
        private static void DrawInventoryOpen(Graphics ig, MapState mapState, GameState gameState)
        {
            var itemsNumber = mapState.Items.Count;
            var selectedItem = gameState.SelectedItem;
            var itemSize = 27;
            var width = (itemSize + 1) * itemsNumber + 1;
            var top = -30;
            ig.FillRectangle(Brushes.White, 0, top, width, itemSize+1);
            ig.DrawRectangle(Pens.Black, 0, top, width-1, itemSize+1);
            for (int i = 0; i < itemsNumber; i++)
            {
                var image = mapState.Items[i].Image;
                ig.DrawImageUnscaled(image, (itemSize + 1) * i + 2, top + 2);
                ig.DrawLine(Pens.Black, (itemSize + 1) * (i + 1), top, (itemSize + 1) * (i + 1), top + itemSize + 1);
            }
            ig.DrawImageUnscaled(SnowInTheNight.Properties.Resources.anchor, width -1, top);

            if (selectedItem < 0 || selectedItem >= itemsNumber)
                return;

            ig.DrawRectangle(Pens.Black, (itemSize + 1) * selectedItem + 1, top + 1, itemSize - 1, itemSize - 1);
            var tb = new TextBubble(new RealPoint(3, top - 3), mapState.Items[selectedItem].Description);
            tb.Alignment = new RealPoint(0, 1);
            tb.DrawInventoryTextBubble(ig);
        }

        private static void DrawInventoryClosed(Graphics ig, MapState mapState, GameState gameState)
        {
            var itemSize = 27;
            var width = (itemSize + 1) + 1;
            var top = -30;
            ig.FillRectangle(Brushes.White, 0, top, width, itemSize+1);
            ig.DrawRectangle(Pens.Black, 0, top, width-1, itemSize+1);

            ig.DrawImageUnscaled(SnowInTheNight.Properties.Resources.bag, 2, top + 2);
            ig.DrawLine(Pens.Black, itemSize + 1, top, itemSize + 1, top + itemSize + 1);

            ig.DrawImageUnscaled(SnowInTheNight.Properties.Resources.anchor, width -1, top);
        }

        private static double GetSquaredVisionRadius()
        {
            var visionRadius = GameForm.FitWidth * 0.71;
            if (Core.MapMode)
            {
                visionRadius *= 4;
            }
            else
            {
                if (Core.BigMode || Core.multiplier != 0)
                {
                    visionRadius *= 1.5;
                }
            }
            var squaredVisionRadius = visionRadius * visionRadius;
            return squaredVisionRadius;
        }

        private static void DrawInteractions (Graphics g, MapState mapState)
        {
            if (Core.MapMode)
                return;
            foreach(var interaction in mapState.Interactions)
            {
                interaction.Draw(g);
            }
        }

        class CheckBbounds
        {
            public double left;
            public double right;
            public double top;
            public double bottom;
        }
        private static CheckBbounds GetVisibleBounds (IntPoint _size)
        {
            var size = (RealPoint)(_size) + new RealPoint(100,100);
            if (Core.MapMode)
            {
                size *= Core.mapModeMultiplier;
            }
            else
            {
                if (Core.BigMode)
                {
                    size *= 1.5;
                }
            }

            var topleft = Core.CameraPosition - size / 2;
            var bottomright = Core.CameraPosition + size / 2;

            return new CheckBbounds()
            {
                left = topleft.X,
                top = topleft.Y,
                right = bottomright.X,
                bottom = bottomright.Y
            };
        }

        private static void DrawDecorationsBetween(Graphics g, MapState mapState, IntPoint size, double y1, double y2)
        {
            var b = GetVisibleBounds(size);
            foreach (var decoration in mapState.Decorations.Concat(mapState.DynamicDecorations))
            {
                if (decoration.IsOutOfScreen(b.left, b.right, b.top, b.bottom))
                    continue;
                decoration.Draw(g, y1, y2);
            }
        }

        private static void DrawSnowflake(Graphics g, Snowflake snowflake)
        {
            var position = snowflake.Position.Round();
            var radius = 1.2f;
            g.FillEllipse(Brushes.White, position.X - radius, position.Y - radius, 2 * radius, 2 * radius);
            //g.DrawEllipse(Pens.LightGray, position.X - radius, position.Y - radius, 2 * radius, 2 * radius);
        }
        public class PlayerDrawer
        {
            private Project3D Projecter;
            private RealPoint Perpendicular;
            public bool IsLeftStep;
            public double Health;
            public bool HasLantern;

            public PlayerDrawer(RealPoint leftLeg, RealPoint rightLeg)
            {
                var position = (leftLeg+rightLeg)/2;
                var delta = rightLeg - leftLeg;
                Perpendicular = delta.Rotate90();
                var up = new RealPoint(0, -1) * MapState.LegLength * Math.Sqrt(0.75);
                Projecter = new Project3D() { O = position, E1 = delta, E2 = Perpendicular * 2, E3 = up * 2 };
            }
            public MapState.Step[] GetExtraSteps()
            {
                var innerLeft = new Project3D.R3Point(-0.4, 0, 0);
                var innerRight = new Project3D.R3Point(0.4, 0, 0);
                var shift = IsLeftStep ? -0.05 : 0.05;
                innerLeft.Y += -shift;
                innerRight.Y += shift;

                var left = Projecter.Project(innerLeft);
                var right = Projecter.Project(innerRight);

                var extraSteps = new MapState.Step[] {
                new MapState.Step() { Position = left, Direction = Perpendicular },
                new MapState.Step() { Position = right, Direction = Perpendicular }};
                return extraSteps;
            }

            public void DrawPlayer(Graphics g)
            {
                var left = new Project3D.R3Point(-0.8, -0.2, 0.2);
                var right = new Project3D.R3Point(0.8, -0.2, 0.2);
                var innerLeft = new Project3D.R3Point(-0.5, 0, 0);
                var innerRight = new Project3D.R3Point(0.5, 0, 0);
                var innerForward = new Project3D.R3Point(0, 0.1, 0);
                var innerTail = new Project3D.R3Point(0, -0.2, 0);
                var leftShoulder = new Project3D.R3Point(-0.5, -0.1, 2);
                var rightShoulder = new Project3D.R3Point(0.5, -0.1, 2);
                var shoulderTail = new Project3D.R3Point(0, -0.2, 1.5);
                var head = new Project3D.R3Point(0, 0.2, 3);
                var leftTail = new Project3D.R3Point(-0.6, -0.4, 0.2);
                var rightTail = new Project3D.R3Point(0.6, -0.4, 0.2);
                var faceLeft = new Project3D.R3Point(-0.2, 0.3, 2.5);
                var faceRight = new Project3D.R3Point(0.2, 0.3, 2.5);

                var tail1 = new Project3D.R3Point(-0.6, -0.4, 0.2);
                var tail2 = new Project3D.R3Point(-0.2, -0.5, 0.2);
                var tail3 = new Project3D.R3Point(0.2, -0.5, 0.2);
                var tail4 = new Project3D.R3Point(0.6, -0.4, 0.2);

                var shift = IsLeftStep ? -0.05 : 0.05;
                var fix = 0.2;

                left.Y += fix - shift;
                leftTail.Y += fix - shift;
                tail1.Y += fix - shift;
                tail2.Y += fix - shift;
                right.Y += fix + shift;
                rightTail.Y += fix + shift;
                tail3.Y += fix + shift;
                tail4.Y += fix + shift;

                innerLeft.Y += -shift;
                innerRight.Y += shift;

                var b = Brushes.Black;
                if (!Core.MapMode)
                {
                    if (Health < 1 && Health > 0)
                    {
                        var rgb = (int)((1 - Health) * 255);
                        rgb = Math.Max(0, Math.Min(255, rgb));
                        var c = Color.FromArgb(rgb, rgb, rgb);
                        b = new LinearGradientBrush(new Point(0, (int)Projecter.O.Y + 20), new Point(0, (int)Projecter.O.Y - 50), Color.Black, c);
                    }
                    if (Health <= 0)
                    {
                        b = new LinearGradientBrush(new Point(0, (int)Projecter.O.Y + 20), new Point(0, (int)Projecter.O.Y - 50), Color.Black, Color.White);
                        left.Y -= 0.1;
                        right.Y -= 0.1;
                        if (Health <= -1)
                        {
                            Projecter.E3 = Projecter.E3 * 0.5;
                        }
                        if (Health <= -2)
                        {
                            Projecter.E3 = Projecter.E3 * 0.5;
                            Projecter.E2 = Projecter.E2 * 1.5;
                        }
                    }
                }

                var lantern = new Project3D.R3Point(0.2, 0.2, 1.2);
                var center = new Project3D.R3Point(0, 0, 1.2);
                var lanternProjection = Projecter.Project(lantern);
                var centerProjection = Projecter.Project(center);
                var projectionDelta = lanternProjection - centerProjection;
                var correctedE1 = Projecter.Project(innerLeft) - Projecter.Project(innerRight);

                var line = new Line(RealPoint.Zero, correctedE1);
                var leftFacing = projectionDelta.X < 0;

                if (HasLantern && !line.ISUnder(projectionDelta))
                {
                    DrawLantern(g, lanternProjection.Round(), leftFacing);
                }


                //projecter.FillPolygon(g, b, left, right, rightShoulder, leftShoulder);
                Projecter.FillPolygon(g, b, faceLeft, faceRight, rightShoulder, leftShoulder);
                Projecter.FillPolygon(g, b, leftShoulder, head, faceLeft);
                Projecter.FillPolygon(g, b, head, rightShoulder, faceRight);
                Projecter.FillPolygon(g, b, head, faceLeft, faceRight);
                Projecter.FillPolygon(g, b, head, leftTail, leftShoulder);
                Projecter.FillPolygon(g, b, rightTail, head, rightShoulder);
                Projecter.FillPolygon(g, b, rightTail, right, rightShoulder);
                Projecter.FillPolygon(g, b, leftTail, left, leftShoulder);
                Projecter.FillPolygon(g, b, rightTail, leftTail, leftShoulder, rightShoulder);
                Projecter.FillPolygon(g, b, innerLeft, innerForward, head, shoulderTail);
                Projecter.FillPolygon(g, b, innerRight, innerForward, head, shoulderTail);
                Projecter.FillPolygon(g, b, innerLeft, innerTail, head);
                Projecter.FillPolygon(g, b, innerRight, innerTail, head);
                Projecter.FillPolygon(g, b, tail1, tail2, tail3, tail4);


                if (HasLantern && line.ISUnder(projectionDelta))
                {
                    DrawLantern(g, lanternProjection.Round(), leftFacing);
                }
                
            }

            private static void DrawLantern(Graphics g, IntPoint k, bool leftFacing)
            {
                if (Core.MapMode)
                    return;

                if(leftFacing)
                {
                    g.FillEllipse(Brushes.White, k.X - 2, k.Y - 4, 5, 7);
                    g.DrawEllipse(Pens.Black, k.X - 2, k.Y - 4, 5, 7);
                    g.DrawLine(Pens.Black, k.X - 1, k.Y + 1, k.X + 2, k.Y + 1);
                    g.DrawLine(Pens.Black, k.X - 1, k.Y - 1, k.X + 2, k.Y - 1);
                    g.DrawLine(Pens.Black, k.X+1, k.Y - 5, k.X + 3, k.Y - 5);
                    g.DrawLine(Pens.Black, k.X, k.Y - 6, k.X + 3, k.Y - 6);
                }
                else
                {
                    g.FillEllipse(Brushes.White, k.X - 3, k.Y - 4, 5, 7);
                    g.DrawEllipse(Pens.Black, k.X - 3, k.Y - 4, 5, 7);
                    g.DrawLine(Pens.Black, k.X - 2, k.Y + 1, k.X + 2, k.Y + 1);
                    g.DrawLine(Pens.Black, k.X - 2, k.Y - 1, k.X + 2, k.Y - 1);
                    g.DrawLine(Pens.Black, k.X - 4, k.Y - 5, k.X-1, k.Y - 5);
                    g.DrawLine(Pens.Black, k.X - 4, k.Y - 6, k.X, k.Y - 6);
                }

                //g.FillEllipse(Brushes.White, k.X - 2, k.Y - 3, 4, 5);
                //g.DrawEllipse(Pens.Black, k.X - 2, k.Y - 3, 3.5f, 5);
                //g.DrawLine(Pens.Black, k.X - 1, k.Y, k.X + 1, k.Y);
            }
        }
        public class Project3D
        {
            public class R3Point
            {
                public R3Point(double x, double y, double z)
                {
                    X = x;
                    Y = y;
                    Z = z;
                }
                public double X;
                public double Y;
                public double Z;
            }
            public RealPoint E1;
            public RealPoint E2;
            public RealPoint E3;
            public RealPoint O;

            public RealPoint Project (R3Point point)
            {
                return O + E1 * point.X + E2 * point.Y + E3 * point.Z;
            }

            public void FillPolygon(Graphics g, Brush b, params R3Point[] polygon)
            {
                g.FillPolygon(b, polygon.Select(p=> (Point)Project(p).Round()).ToArray());
            }
        }
    }
}

