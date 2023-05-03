using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SnowInTheNight
{
    interface Decoration
    {
        double GetYforSorting();
        bool IsOutOfScreen(double left, double right, double top, double bottom);
        void Draw(Graphics g, double yMin, double yMax);
        void Update(double deltaTime, MapState mapState, GameState gameState);
    }
    [Serializable]
    class Circle : Decoration
    {
        public RealPoint Position;
        public double Radius;
        public Circle(RealPoint position, double radius)
        {
            this.Position = position;
            this.Radius = radius;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return false;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            g.DrawEllipse(Pens.Black, (int)(Position.X - Radius), (int)(Position.Y - Radius), (int)Radius * 2, (int)Radius * 2);
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    [Serializable]
    class Shout : Decoration
    {
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return false;
        }
        public Shout (RealPoint position, double radius, double despawnRadius, String text)
        {
            SpawnInteractor = new CircleInteractor(position, radius);
            DespawnInteractor = new CircleInteractor(position, despawnRadius);
            ShoutTextBubble = new TextBubble(position, text);
        }
        public Interactor SpawnInteractor;
        public Interactor DespawnInteractor;
        public TextBubble ShoutTextBubble;
        public bool Reviveable = false;
        public double ExtraSeconds = 2;
        private bool alive = false;
        private bool dead = false;
        private double symbolsVisible = 0;

        public void Despawn()
        {
            dead = true;
        }
        public double GetYforSorting()
        {
            return double.MaxValue;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (yMax < double.MaxValue)
                return;
            if (dead)
                return;
            if (!alive)
                return;
            if (Core.MapMode)
                return;
            ShoutTextBubble.DrawTextBubble(g);
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (dead)
                return;
            if (Core.MapMode)
                return;
            if(!alive)
            {
                if(SpawnInteractor.GetInteractiveDistance(mapState.PlayerPosition, mapState.PlayerDirection) < double.MaxValue)
                {
                    alive = true;
                    symbolsVisible = 0;
                }
            }
            if(alive)
            {
                if(DespawnInteractor.GetInteractiveDistance(mapState.PlayerPosition, mapState.PlayerDirection) == double.MaxValue)
                {
                    alive = false;
                    if(!Reviveable)
                        dead = true;
                }
            }
            if(alive)
            {
                symbolsVisible += deltaTime * TextBubble.printingSpeed;
                ShoutTextBubble.SymbolsVisible = (int)symbolsVisible;
                if (symbolsVisible > ShoutTextBubble.Text.Length + ExtraSeconds * TextBubble.printingSpeed)
                {
                    alive = false;
                    if (!Reviveable)
                        dead = true;
                }
            }
        }
    }
    
    [Serializable]
    class Fence : Decoration
    {
        RealPoint[] polyline;
        static double step = 16;
        static double height = 10;
        static double extra = 4;
        public int Seed = 0;
        
        public static Pen pen = new Pen(Color.Black, 2);

        private double maxX;
        private double maxY;
        private double minX;
        private double minY;
        public Fence (params RealPoint[] polyline)
        {
            this.polyline = polyline;
            maxX = polyline.Max(p => p.X);
            maxY = polyline.Max(p => p.Y);
            minX = polyline.Min(p => p.X);
            minY = polyline.Min(p => p.Y);
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return maxX < left || minX > right || maxY < top || minY > bottom;
        }
        public double GetYforSorting()
        {
            return double.MinValue;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            var precalc = GetPrecalc(Seed);
            for (int i = 0; i < polyline.Length - 1; i++)
            {
                DrawPrecalc(precalc[i], g, yMin, yMax);
            }
            return;
            for (int i = 1; i < polyline.Length; i++)
            {
                var p1 = polyline[i - 1];
                var p2 = polyline[i];
                DrawSegment(g, p1, p2, Seed + i, yMin, yMax);
            }
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
        [NonSerialized]
        RealPoint[][][] precalc;
        RealPoint[][][] GetPrecalc(int seed)
        {
            if (precalc == null)
            {
                precalc = new RealPoint[polyline.Length - 1][][];
                for (int j = 0; j < polyline.Length-1; j++)
                {
                    var A = polyline[j];
                    var B = polyline[j+1];

                    var R = new Random(seed);
                    var delta = B - A;
                    var length = delta.Length;
                    var count = (int)Math.Ceiling(length / step);

                    precalc[j] = new RealPoint[count + 2][];
                    var fixedStep = length / (count);
                    delta = delta.Normalize() * fixedStep;
                    {
                        var h = height + R.NextDouble() * extra;
                        var b = A;
                        var t = b + new RealPoint(0, -h);

                        precalc[j][0] = new RealPoint[] { b, t };
                    }
                    for (int i = 1; i < count; i++)
                    {
                        var h = height + R.NextDouble() * extra;
                        var s = (R.NextDouble() * 2 - 1) * extra;
                        var b = A + delta * i;
                        var t = b + new RealPoint(s, -h);

                        precalc[j][i] = new RealPoint[] { b, t };
                    }
                    {
                        var h = height + R.NextDouble() * extra;
                        var b = B;
                        var t = b + new RealPoint(0, -h);

                        precalc[j][count] = new RealPoint[] { b, t };
                    }
                    {
                        var h = height - R.NextDouble() * extra;
                        //var a = A + new RPoint(0,-h1);
                        //var b = B + new RPoint(0,-h2);

                        var a = A + new RealPoint(0, -h);
                        var b = B + new RealPoint(0, -h);

                        precalc[j][count+1] = new RealPoint[] { A, B, a, b };
                    }
                }
            }
            return precalc;
        }

        private void DrawPrecalc (RealPoint[][] precalc, Graphics g, double yMin, double yMax)
        {
            for (int i = 0; i < precalc.Length - 1; i++)
            {
                var b = precalc[i][0];
                var t = precalc[i][1];
                if (b.Y >= yMin && b.Y <= yMax)
                    g.DrawLine(pen, (Point)b.Round(), (Point)t.Round());
            }
            {
                var A = precalc[precalc.Length - 1][0];
                var B = precalc[precalc.Length - 1][1];
                var a = precalc[precalc.Length - 1][2];
                var b = precalc[precalc.Length - 1][3];

                var oldClip = g.Clip;

                var minX = (float)Math.Min(A.X, B.X) - 10;
                var maxX = (float)Math.Max(A.X, B.X) + 10;
                var minY = (float)Math.Max(Math.Min(A.Y, B.Y) - 10, yMin);
                var maxY = (float)Math.Min(Math.Max(A.Y, B.Y) + 10, yMax);
                g.SetClip(new RectangleF(minX, minY, maxX - minX, maxY - minY));

                g.DrawLine(pen, (Point)a.Round(), (Point)b.Round());

                g.Clip = oldClip;
            }
        }

        private void DrawSegment (Graphics g, RealPoint A, RealPoint B, int seed, double yMin, double yMax)
        {
            var R = new Random(seed);
            var delta = B-A;
            var length = delta.Length;
            var count = (int)Math.Ceiling(length / step);
            var fixedStep = length / (count);
            delta = delta.Normalize() * fixedStep;
            {
                var h = height + R.NextDouble() * extra;
                var b = A;
                var t = b + new RealPoint(0, -h);
                if (b.Y >= yMin && b.Y <= yMax)
                    g.DrawLine(pen, (Point)b.Round(), (Point)t.Round());
            }
            for(int i =1; i< count; i++)
            {
                var h = height + R.NextDouble() * extra;
                var s = (R.NextDouble() * 2 - 1) * extra;
                var b = A + delta*i;
                var t = b + new RealPoint(s,-h);
                if(b.Y >= yMin && b.Y <= yMax)
                    g.DrawLine(pen, (Point)b.Round(), (Point)t.Round());
            }
            {
                var h = height + R.NextDouble() * extra;
                var b = B;
                var t = b + new RealPoint(0, -h);
                if (b.Y >= yMin && b.Y <= yMax)
                    g.DrawLine(pen, (Point)b.Round(), (Point)t.Round());
            }
            {
                var oldClip = g.Clip;


                var minX = (float)Math.Min(A.X, B.X) - 10;
                var maxX = (float)Math.Max(A.X, B.X) + 10;
                var minY = (float)Math.Max(Math.Min(A.Y, B.Y) - 10, yMin);
                var maxY = (float)Math.Min(Math.Max(A.Y, B.Y) + 10, yMax);
                g.SetClip(new RectangleF(minX, minY, maxX - minX, maxY - minY));

                var h = height - R.NextDouble() * extra;
                //var a = A + new RPoint(0,-h1);
                //var b = B + new RPoint(0,-h2);

                var a = A + new RealPoint(0, -h);
                var b = B + new RealPoint(0, -h);
                g.DrawLine(pen, (Point)a.Round(), (Point)b.Round());

                g.Clip = oldClip;
                return;

                //if (A.Y > B.Y)
                //{
                //    var c = B;
                //    B = A;
                //    A = c;
                //}
                //if (A.Y == B.Y)
                //{
                //    if (A.Y < yMin || A.Y > yMax)
                //        return;
                //    var a = A + new RPoint(0, -h);
                //    var b = B + new RPoint(0, -h);
                //    g.DrawLine(pen, (Point)a.Round(), (Point)b.Round());
                //    return;
                //}
                //{
                //    var line = new RLine(A, B);
                //    var minLine = new RLine(yMin);
                //    var maxLine = new RLine(yMax);

                //    var aPrime = minLine & line;
                //    var bPrime = maxLine & line;

                //    if (A.Y < aPrime.Y)
                //        A = aPrime;
                //    if (B.Y > bPrime.Y)
                //        B = bPrime;

                //    if (A.Y > B.Y)
                //        return;

                //    var a = A + new RPoint(0, -h);
                //    var b = B + new RPoint(0, -h);
                //    g.DrawLine(pen, (Point)a.Round(), (Point)b.Round());
                //}
            }
        }
    }

    [Serializable]
    class GraveWood : Decoration
    {
        public RealPoint Position;
        public int Seed;
        public GraveWood(RealPoint postion, int seed)
        {
            this.Position = postion;
            this.Seed = seed;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;
            var R = new Random(Seed);
            g.DrawLine(Fence.pen, (int)Position.X, (int)Position.Y, (int)Position.X + R.Next(-1, 2), (int)Position.Y - 12 + R.Next(-1, 2));
            g.DrawLine(Fence.pen, (int)Position.X - 4 + R.Next(-1, 2), (int)Position.Y - 8 + R.Next(-1, 2), (int)Position.X + 4 + R.Next(-1, 2), (int)Position.Y - 8 + R.Next(-1, 2));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }

    [Serializable]
    class Stump : Decoration
    {
        public RealPoint Position;
        public int frame;
        public Stump(RealPoint postion, int frame)
        {
            this.Position = postion;
            this.frame = frame;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = CachingWorks.StumpSize;
            var croppedImage = CachingWorks.GetStump(frame);

            var offset = new RealPoint(0, 5);

            g.DrawImageUnscaled(croppedImage, (Point)((Position + offset).Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    
    [Serializable]
    partial class FirePlace : Decoration
    {
        public RealPoint Position;
        public bool isFueled;
        public bool isVisited;
        static Pen pen = new Pen(Color.Black, 2);
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if(Position.Y < yMin || Position.Y > yMax)
                return;

            if (!isFueled)
            {
                g.DrawLine(pen, (Point)(Position.Round() + new IntPoint(-5, -5)), (Point)(Position.Round() + new IntPoint(10, 10)));
                g.DrawLine(pen, (Point)(Position.Round() + new IntPoint(0, -6)), (Point)(Position.Round() + new IntPoint(0, 14)));
                g.DrawLine(pen, (Point)(Position.Round() + new IntPoint(7, -4)), (Point)(Position.Round() + new IntPoint(-10, 10)));
            }
            else
            {
                g.DrawLine(pen, (Point)(Position.Round() + new IntPoint(0, 0)), (Point)(Position.Round() + new IntPoint(10, 10)));
                g.DrawLine(pen, (Point)(Position.Round() + new IntPoint(0, 0)), (Point)(Position.Round() + new IntPoint(0, 14)));
                g.DrawLine(pen, (Point)(Position.Round() + new IntPoint(0, 2)), (Point)(Position.Round() + new IntPoint(-10, 10)));
                var size = new Size(15,15);

                var sourceArea = new Rectangle(new Point(size.Width * danceFrame, 0), size);
                var image = SnowInTheNight.Properties.Resources.fire;

                var croppedImage = image.Clone(sourceArea, image.PixelFormat);

                g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-7, -6)));
            }
        }
        int danceFrame = 0;
        double nextFrameTimer = 0;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (Core.MapMode)
                return;
            if (!isFueled)
                return;
            nextFrameTimer -= deltaTime;
            if(nextFrameTimer <=0)
            {
                var R = new Random();
                nextFrameTimer = R.NextDouble() * 0.3 + 0.2;
                var r = R.NextDouble();
                if (r > 0.85)
                    danceFrame = 3;
                else if (r > 0.7)
                    danceFrame = 1;
                else
                {
                    if (danceFrame == 0)
                        danceFrame = 2;
                    else if (danceFrame == 2)
                        danceFrame = 0;
                    else if (r > 0.35)
                        danceFrame = 2;
                    else
                        danceFrame = 0;
                }
                
            }
        }
    }

    [Serializable]
    class SledDecoration : Decoration
    {
        public RealPoint Position;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left - 60 || Position.X > right + 60 || Position.Y < top || Position.Y > bottom;
        }
        public SledDecoration(RealPoint position)
        {
            this.Position = position;
        }
        public RealPoint[] GetBorder()
        {
            return new RealPoint[]
            {
                Position + new RealPoint(-55,0),
                Position + new RealPoint(-55,-12),
                Position + new RealPoint(0,-12),
                Position + new RealPoint(62,0)
            };
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var image = SnowInTheNight.Properties.Resources.sled;
            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-61,-35)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
        }
    }

    partial class Maid : Decoration, Collider
    {
        public RealPoint Position;
        int frame = 0;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public Maid(RealPoint position)
        {
            this.DecoratedCollider = new CircleCollider(position + new RealPoint(0, -5), 15);
            this.Position = position;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(36, 46);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.maid;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-18, -46)));

            croppedImage.Dispose();
            image.Dispose();
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if(frame != 0)
            {
                var sign = mapState.PlayerPosition.X > Position.X ? 0 : -1;
                if (frame <= 2)
                {
                    frame = 2 + sign;
                }
                else if(frame <= 4)
                {
                    frame = 4 + sign;
                }
            }
        }


        public Collider DecoratedCollider;
        public RealPoint Collide(RealPoint point)
        {
            return DecoratedCollider.Collide(point);
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            return RealPoint.Zero;
        }
        public void DrawAntiCollider(Graphics g)
        {
        }
    }

    [Serializable]
    partial class BellGate : Decoration
    {
        public RealPoint Position;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public RealPoint[] GetBorder()
        {
            return new RealPoint[]
            {
                Position + new RealPoint(-7, -2),
                Position + new RealPoint(7, 2)
            };
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(14, 20);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.bellGate;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-7, -18)));
        }
        private int frame = 0;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
        }
    }

    [Serializable]
    partial class Beggar : Decoration, Collider
    {
        public RealPoint Position;
        public bool dead;
        public Beggar(RealPoint position)
        {
            this.Position = position;
            this.DecoratedCollider = new CircleCollider(position + new RealPoint(5, -3), 15);
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (dead)
                return;
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(45, 27);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.beggar;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-size.Width/2, -size.Height)));
            
        }
        private int frame = 0;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (dead)
                return;
            var b = (ItemBucket)mapState.Items.FirstOrDefault(i => i is ItemBucket);
            if (b != null && b.Uses > 0)
            {
                dead = true;
                mapState.AddBeggarPath();
            }
        }

        public Collider DecoratedCollider;
        public RealPoint Collide(RealPoint point)
        {
            if (dead)
                return RealPoint.Zero;
            return DecoratedCollider.Collide(point);
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            return RealPoint.Zero;
        }
        public void DrawAntiCollider(Graphics g)
        {
        }
    }

    [Serializable]
    class Tree : Decoration
    {
        public RealPoint Position;
        public Tree(RealPoint position, int frame)
        {
            this.Position = position;
            this.frame = frame;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom + 100;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            //var size = new Size(69, 91);

            //var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            //var image = SnowWalkingTest.Properties.Resources.tree;

            //var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            var size = CachingWorks.TreeSize;
            var croppedImage = CachingWorks.GetTree(frame);

            var offset = new RealPoint(0, 5);

            if (frame == 2)
                offset += new RealPoint(-5, 3);
            if (frame == 3)
                offset += new RealPoint(15, 0);
            if (frame == 9)
                offset += new RealPoint(-15, 0);
            if (frame == 0)
                offset += new RealPoint(-5, 0);

            g.DrawImageUnscaled(croppedImage, (Point)((Position + offset).Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));

        }
        private int frame = 0;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
        }
    }

    [Serializable]
    partial class LostBucket : Decoration, Collider
    {
        public RealPoint Position;
        public bool Found;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Found)
                return;
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(13, 8);

            var image = SnowInTheNight.Properties.Resources.lostBucket;

            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
        }


        public Collider DecoratedCollider;
        public RealPoint Collide(RealPoint point)
        {
            if (Found)
                return RealPoint.Zero;
            return DecoratedCollider.Collide(point);
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            return RealPoint.Zero;
        }
        public void DrawAntiCollider(Graphics g)
        {
        }
    }

    [Serializable]
    partial class NastyWoman : Decoration
    {
        public RealPoint Position;
        int frame = 2;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public NastyWoman(RealPoint position)
        {
            this.Position = position;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(21, 40);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.nastyWoman;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-18, -46)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (frame != 0)
            {
                var sign = mapState.PlayerPosition.X > Position.X ? 0 : -1;
                frame = 2 + sign;
            }
        }
    }

    [Serializable]
    partial class DeadBeggar : Decoration, Collider
    {
        public RealPoint Position;
        public bool Exists;
        public bool Found;
        public DeadBeggar(RealPoint position)
        {
            this.Position = position;
            DecoratedCollider = new CircleCollider(position, 20);
            decoratedInteraction = new StaticInteraction(position, 50, "У него осталась старая карта.\r\nСудя по пометкам на ней, он шёл в замок.") { DisableOnUse = true };
            decoratedInteraction.DespawnInteractor = new CircleInteractor(position, 150);
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (!Exists)
                return;
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(41, 20);

            var image = SnowInTheNight.Properties.Resources.deadBeggar;

            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height/2)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (!Exists)
            {
                var b = (ItemBucket)mapState.Items.FirstOrDefault(i => i is ItemBucket);
                if (b != null && b.Uses > 0)
                {
                    Exists = true;
                    mapState.AddBeggarPath();
                }
            }
        }


        public Collider DecoratedCollider;
        public RealPoint Collide(RealPoint point)
        {
            if (!Exists)
                return RealPoint.Zero;
            return DecoratedCollider.Collide(point);
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            return RealPoint.Zero;
        }
        public void DrawAntiCollider(Graphics g)
        {
        }
    }
    [Serializable]
    partial class Elder : Decoration
    {
        public RealPoint Position;
        public int frame;
        public Elder(RealPoint position)
        {
            this.Position = position;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(45, 49);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.elder;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    partial class PostSign : Decoration
    {
        public RealPoint Position;
        public int frame = 0;
        public PostSign(RealPoint position)
        {
            this.Position = position;
            this.R = new Random();
            decoratedInteraction = new StaticInteraction(position, 70, "");
            decoratedInteraction.ReadingTextBubble.Position = position + new RealPoint(0, -70);
            decoratedInteraction.DespawnInteractor = new CircleInteractor(position, 100);
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(34, 56);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);

            if (Core.MapMode)
            {
                sourceArea = new Rectangle(new Point(0, 0), size);
            }
            var image = SnowInTheNight.Properties.Resources.post;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        Random R;
        int animationTick = 0;
        bool flipped = false;
        int animationIndex = 0;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (gameState.IsStormy && !Core.MapMode)
            {
                var oldTick = animationTick;
                for (int i = 0; i < Core.Ticks; i++)
                {
                    animationTick--;
                    if (animationTick <= 0)
                    {
                        if (R.NextDouble() > 0.99)
                        {
                            animationTick = 50;
                            oldTick = 50;
                            var r = R.NextDouble();
                            if (r > 0.7)
                            {
                                animationIndex = 0;
                            }
                            else if (r > 0.4)
                            {
                                animationIndex = 1;
                            }
                            else
                            {
                                animationIndex = 2;
                            }
                        }
                    }
                }
                if (animationIndex == 0)
                {
                    if (animationTick <= 0)
                    {
                        frame = 0;
                    }
                    else if (20 <= animationTick && animationTick <= 40)
                    {
                        frame = 1;
                    }
                    else
                    {
                        frame = 2;
                    }
                }
                else if (animationIndex == 1)
                {
                    if (animationTick <= 0)
                    {
                        frame = 0;
                    }
                    else if (20 <= animationTick && animationTick <= 30)
                    {
                        frame = 2;
                    }
                    else
                    {
                        frame = 1;
                    }
                }
                else if (animationIndex == 2)
                {
                    if(animationTick <= -10)
                    {
                        frame = 0;
                        animationTick = 50;
                        var r = R.NextDouble();
                        if (r > 0.6)
                        {
                            animationIndex = 0;
                        }
                        else
                        {
                            animationIndex = 1;
                        }
                    }
                    else if (animationTick <= 0)
                    {
                        frame = 0;
                    }
                    else if (animationTick <= 10)
                    {
                        frame = 5;
                    }
                    else if (animationTick <= 20)
                    {
                        frame = 4;
                    }
                    else if (animationTick <= 30)
                    {
                        if (oldTick > 30)
                        {
                            flipped = !flipped;
                        }
                        frame = 3;
                    }
                    else if (animationTick <= 40)
                    {
                        frame = 2;
                    }
                    else
                    {
                        frame = 1;
                    }
                }
            }
        }
    }
    [Serializable]
    class Cart : Decoration
    {
        public RealPoint Position;
        public Cart(RealPoint postion)
        {
            this.Position = postion;
        }
        internal RealPoint[] GetBorder()
        {
            return new RealPoint[]{
                Position + new RealPoint(-30,-5),
                Position + new RealPoint(-25,-15),
                Position + new RealPoint(35,-15)
            };
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var image = SnowInTheNight.Properties.Resources.cart;
            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-37, -34)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    [Serializable]
    class DogHouse : Decoration
    {
        public RealPoint Position;
        public DogHouse(RealPoint postion)
        {
            this.Position = postion;
        }
        internal RealPoint[] GetBorder()
        {
            return new RealPoint[]{
                Position + new RealPoint(-10,-8),
                Position + new RealPoint(-8,-20),
                Position + new RealPoint(10,-20),
                Position + new RealPoint(10,-8)
            };
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var image = SnowInTheNight.Properties.Resources.doghouse;
            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-14, -28)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }

    [Serializable]
    class Stable : Decoration
    {
        public RealPoint Position;
        public Stable(RealPoint postion)
        {
            this.Position = postion;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left- 90 || Position.X > right + 90 || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var image = SnowInTheNight.Properties.Resources.stable;
            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-90, -28)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }

    [Serializable]
    class Tomb : Decoration
    {
        public RealPoint Position;
        public Tomb(RealPoint postion)
        {
            this.Position = postion;
        }

        internal RealPoint[] GetBorder()
        {
            return new RealPoint[]{
                Position + new RealPoint(-42,-15),
                Position + new RealPoint(-51,-5),
                Position + new RealPoint(-20, 0),
                Position + new RealPoint(47,-10),
                Position + new RealPoint(20,-30)
            };
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left -10 || Position.X > right +10 || Position.Y < top || Position.Y > bottom + 90;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var image = SnowInTheNight.Properties.Resources.crypt;
            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-51, -90)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    [Serializable]
    class CastleDoor : Decoration
    {
        public RealPoint Position;
        public CastleDoor(RealPoint postion)
        {
            this.Position = postion;
        }

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var image = SnowInTheNight.Properties.Resources.door;
            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-19, -47)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }

    [Serializable]
    class Well : Decoration
    {
        public RealPoint Position;
        public Well (RealPoint postion)
        {
            this.Position = postion;
        }

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var image = SnowInTheNight.Properties.Resources.well;
            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-12, -35)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }

    [Serializable]
    class Wall : Decoration
    {
        RealPoint[] polyline;
        static double step = 32;
        static double height = 64;
        static double toothHeight = 6;
        public int Seed = 0;

        private double maxX;
        private double maxY;
        private double minX;
        private double minY;
        public Wall(params RealPoint[] polyline)
        {
            this.polyline = polyline;
            maxX = polyline.Max(p => p.X);
            maxY = polyline.Max(p => p.Y);
            minX = polyline.Min(p => p.X);
            minY = polyline.Min(p => p.Y);
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return maxX < left || minX > right || maxY < top || minY > bottom - height;
        }
        public double GetYforSorting()
        {
            return double.MinValue;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (yMin != double.MinValue)
                return;
            for (int i = 1; i < polyline.Length; i++)
            {
                var p1 = polyline[i - 1];
                var p2 = polyline[i];
                DrawSegment(g, p1, p2, Seed + i);
            }
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
        private void DrawSegment(Graphics g, RealPoint A, RealPoint B, int seed)
        {
            var R = new Random(seed);
            var delta = B - A;
            var length = delta.Length;
            var count = (int)Math.Ceiling(length / step);
            var fixedStep = length / (count);
            delta = delta.Normalize();

            var pr = Math.Sqrt(0.75) / 2;
            var projecter = new Drawer.Project3D()
            {
                O = A,
                E1 = delta,
                E2 = new RealPoint(delta.Y / pr, - delta.X * pr).Normalize(),
                E3 = new RealPoint(0, -1)
            };

            var b = Brushes.Black;

            for(int i = 0; i<count; i++)
            {
                var x0 = i*fixedStep + fixedStep/2;
                var xs = R.Next(-1, 2);
                var x1 = x0 - toothHeight + xs + R.Next(-1, 2);
                var x2 = x0 + toothHeight + xs + R.Next(-1, 2);
                var y1 = 0;
                var y2 = -toothHeight;
                var z1 = height - 2;
                var z2 = height + toothHeight + R.Next(-1, 2);


                var A1 = new Drawer.Project3D.R3Point(x1, y1, z1);
                var A2 = new Drawer.Project3D.R3Point(x2, y1, z1);
                var B1 = new Drawer.Project3D.R3Point(x1, y1, z2);
                var B2 = new Drawer.Project3D.R3Point(x2, y1, z2);
                var C1 = new Drawer.Project3D.R3Point(x1, y2, z2);
                var C2 = new Drawer.Project3D.R3Point(x2, y2, z2);
                var D1 = new Drawer.Project3D.R3Point(x1, y2, z1);
                var D2 = new Drawer.Project3D.R3Point(x2, y2, z1);


                projecter.FillPolygon(g, b, A1, B1, B2, A2);
                projecter.FillPolygon(g, b, C1, B1, B2, C2);
                projecter.FillPolygon(g, b, C1, D1, D2, C2);
            }
            {
                projecter = new Drawer.Project3D()
                {
                    O = A,
                    E1 = delta,
                    E2 = delta.Rotate90().Normalize(),
                    E3 = new RealPoint(0, -1)
                };
                var x1 = 0;
                var x2 = length;
                var y1 = 0;
                var y2 = -toothHeight;
                var z1 = 0;
                var z2 = height;

                var A1 = new Drawer.Project3D.R3Point(x1, y1, z1);
                var A2 = new Drawer.Project3D.R3Point(x2, y1, z1);
                var B1 = new Drawer.Project3D.R3Point(x1, y1, z2);
                var B2 = new Drawer.Project3D.R3Point(x2, y1, z2);
                var C1 = new Drawer.Project3D.R3Point(x1, y2, z1);
                var C2 = new Drawer.Project3D.R3Point(x2, y2, z1);

                projecter.FillPolygon(g, b, A1, B1, B2, A2);
                projecter.FillPolygon(g, b, A1, C1, C2, A2);
                g.FillEllipse(b, new Rectangle((Point)(A + new RealPoint(-toothHeight, -toothHeight)).Round(), new Size((int)(2 * toothHeight), (int)(Math.Floor(2 * toothHeight)))));
            }
        }
    }

    [Serializable]
    class GraveStone : Decoration
    {
        public RealPoint Position;
        public int frame;
        public GraveStone (RealPoint postion, int frame)
        {
            this.Position = postion;
            this.frame = frame;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = CachingWorks.GraveSize;
            var croppedImage = CachingWorks.GetGraveStone(frame);

            var offset = new RealPoint(0, 3);

            g.DrawImageUnscaled(croppedImage, (Point)((Position + offset).Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    partial class Madman : Decoration, Collider
    {
        public RealPoint Position;
        int frame = 0;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom;
        }
        public Madman(RealPoint position)
        {
            this.DecoratedCollider = new CircleCollider(position + new RealPoint(0, -10), 20);
            this.Position = position;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(48, 48);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.madman;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
        }


        public Collider DecoratedCollider;
        public RealPoint Collide(RealPoint point)
        {
            return DecoratedCollider.Collide(point);
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            return RealPoint.Zero;
        }
        public void DrawAntiCollider(Graphics g)
        {
        }
    }


    partial class Ghost : Decoration, Collider
    {
        public RealPoint Position;
        int frame = 0;
        int frameTimer = 0;
        bool isInDialogue = false;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom + 50;
        }
        public Ghost(RealPoint position)
        {
            this.DecoratedCollider = new CircleCollider(position + new RealPoint(0, 0), 10);
            this.Position = position;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(23, 58);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.duke;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            if (isInDialogue)
            {
                frameTimer = 0;
            }
            else
            {
                frameTimer -= Core.Ticks;
                {
                    if (frameTimer > 40)
                    {
                        frame = 1;
                    }
                    else if (frameTimer > 20)
                    {
                        frame = 2;
                    }
                    else if (frameTimer > 10)
                    {
                        frame = 1;
                    }
                    else if (frameTimer > 0)
                    {
                        frame = 0;
                    }
                    else
                    {
                        frameTimer = 0;
                    }
                }
            }
        }

        public Collider DecoratedCollider;
        public RealPoint Collide(RealPoint point)
        {
            return DecoratedCollider.Collide(point);
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            return RealPoint.Zero;
        }
        public void DrawAntiCollider(Graphics g)
        {
        }
    }

    [Serializable]
    class Mill : Decoration
    {
        public RealPoint Position;
        public Mill(RealPoint postion)
        {
            this.Position = postion;
        }

        internal RealPoint[] GetBorder()
        {
            return new RealPoint[]{
                Position + new RealPoint(-35, 0),
                Position + new RealPoint(-20,-40),
                Position + new RealPoint(30, -40),
                Position + new RealPoint(45, 0),
                Position + new RealPoint(-10,10),
                Position + new RealPoint(20,10)
            };
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left - 10 || Position.X > right + 100 || Position.Y < top || Position.Y > bottom + 90;
        }
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var image = SnowInTheNight.Properties.Resources.mill;
            g.DrawImageUnscaled(image, (Point)(Position.Round() + new IntPoint(-100, -140)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    
    partial class Guard : Decoration, Collider
    {
        public RealPoint Position;
        int frame = 0;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom + 50;
        }
        public Guard(RealPoint position)
        {
            this.DecoratedCollider = new CircleCollider(position + new RealPoint(0, 0), 10);
            DecoratedCollider2 = new SegmentCollider(position + new RealPoint(-20, -30), position + new RealPoint(50, -30));
            this.Position = position;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(39, 58);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.guard;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            blockPath = gameState.EndingPhase == 0;
        }

        bool blockPath;
        public Collider DecoratedCollider;
        public Collider DecoratedCollider2;
        public RealPoint Collide(RealPoint point)
        {
            if(blockPath)
            {
                var p = point;
                p+=DecoratedCollider.Collide(p);
                p+=DecoratedCollider2.Collide(p);
                return p - point;
            }
            return DecoratedCollider.Collide(point);
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            return RealPoint.Zero;
        }
        public void DrawAntiCollider(Graphics g)
        {
        }
    }
    
    partial class Nomad : Decoration, Collider
    {
        public RealPoint Position;
        int frame = 0;
        public double GetYforSorting()
        {
            return Position.Y;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return Position.X < left || Position.X > right || Position.Y < top || Position.Y > bottom + 50;
        }
        public Nomad(RealPoint position)
        {
            this.DecoratedCollider1 = new CircleCollider(position + new RealPoint(-25,-15), 10);
            this.DecoratedCollider2 = new CircleCollider(position + new RealPoint(15, -10), 8);
            this.DecoratedCollider3 = new CircleCollider(position + new RealPoint(-10, -5), 10);
            this.Position = position;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (Position.Y < yMin || Position.Y > yMax)
                return;

            var size = new Size(71, 47);

            var sourceArea = new Rectangle(new Point(size.Width * frame, 0), size);
            var image = SnowInTheNight.Properties.Resources.nomad;

            var croppedImage = image.Clone(sourceArea, image.PixelFormat);

            g.DrawImageUnscaled(croppedImage, (Point)(Position.Round() + new IntPoint(-(size.Width + 1) / 2, -size.Height)));
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
     
        }

        public Collider DecoratedCollider1;
        public Collider DecoratedCollider2;
        public Collider DecoratedCollider3;
        public RealPoint Collide(RealPoint point)
        {
            var p = point;
            p += DecoratedCollider1.Collide(p);
            p+= DecoratedCollider2.Collide(p);
            if(frame == 1)
            {
                p += DecoratedCollider3.Collide(p);
            }
            return p - point;
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            return RealPoint.Zero;
        }
        public void DrawAntiCollider(Graphics g)
        {
        }
    }

    [Serializable]
    class MapText : Decoration
    {
        public RealPoint Position;
        public int TextIndex;
        int frame = 0;
        public double GetYforSorting()
        {
            return double.MaxValue;
        }
        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return false;
        }
        public MapText(RealPoint position, int textIndex)
        {
            this.Position = position;
            this.TextIndex = textIndex;
        }
        public void Draw(Graphics g, double yMin, double yMax)
        {
            if (!Core.MapMode)
                return;
            Draw(g);
        }
        public void Draw(Graphics g)
        {
            var position = Position;

            var text = "";
            if (TextIndex >=10)
            {
                var location = Location.FromId(TextIndex - 10);
                if (location.IsVisited())
                {
                    text = location.Name;

                    position -= new RealPoint(8 * text.Length , 0);
                }
            }
            else
            {
                if (TextIndex == 1)
                    text = "    Путь к замку:\r\n           Свернуть у знака\r\n       Обойти мельницу\r\nПройти по кладбищу";
                if (TextIndex == 2)
                    text = "Злюка";
                text = TextWorks.GetText(text);
            }

            g.DrawString(text, Drawer.gameFontSloppyMap, Brushes.White, (Point)position.Round());
        }
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
     
        }
    }
}

