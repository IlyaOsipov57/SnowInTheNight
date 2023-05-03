using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    [Serializable]
    class KeepersHouse : Decoration
    {
        double left;
        double right;
        double top;
        double bottom;
        public int Seed;

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return this.right < left || this.left > right || this.bottom < top || this.top > bottom + BlockHeight;
        }
        public double GetYforSorting()
        {
            return bottom;
        }
        public KeepersHouse(double x, double y, int seed)
        {
            this.left = x - 50;
            this.right = x + 50;
            this.top = y - BlockHeight;
            this.bottom = y;

            this.Seed = seed;
        }
        public RealPoint[] GetBorder()
        {
            var bottomLeft = new RealPoint(left, bottom);
            var bottomRight = new RealPoint(right, bottom);
            var topLeft = new RealPoint(left + 10, top);
            var topRight = new RealPoint(right - 10, top);
            return new RealPoint[]
            {
                bottomLeft,
                topLeft,
                topRight,
                bottomRight
            };
        }

        public static int BlockHeight = 60;
        public static int BasementHeight = 10;

        public void Draw(Graphics g, double yMin, double yMax)
        {
            var centerY = (this.top + this.bottom) / 2;
            if (centerY < yMin || centerY > yMax)
                return;

            var left = (int)this.left;
            var right = (int)this.right;
            var bottom = (int)this.bottom;
            var top = (int)this.top;

            var basementLine = bottom - BasementHeight;
            var roofLine = basementLine - BlockHeight;
            var roofTop = roofLine - BlockHeight / 3;

            var f1 = new Point(left, bottom);
            var f2 = new Point(left + 2, basementLine + 2);
            var f3 = new Point(left + 4, basementLine);
            var f4 = new Point(left + 8, roofLine + 2);
            var f5 = new Point(left, roofLine);
            var f6 = new Point(left + 20, roofTop);

            var f7 = new Point(right - 30, roofTop + 3);
            var f8 = new Point(right - 28, roofTop - 7);
            var f9 = new Point(right - 24, roofTop - 7);
            var f10 = new Point(right - 26, roofTop + 3);
            var f11 = new Point(right - 26, roofTop + 2);

            var f12 = new Point(right - 20, roofTop + 4);
            var f13 = new Point(right, roofLine + 4);
            var f14 = new Point(right - 8, roofLine + 6);
            var f15 = new Point(right - 4, basementLine);
            var f16 = new Point(right - 2, basementLine + 2);
            var f17 = new Point(right, bottom);
            var f18 = new Point(left + 50, bottom + 16);

            var poly = new Point[] { f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18 };
            var b = Brushes.Black;
            g.FillPolygon(b, poly);
            /*{
                var d1 = new PointF(left + 16, basementLine);
                var d2 = new PointF(left + 18, basementLine - 40);
                var d3 = new PointF(left + 32, basementLine - 38);
                var d4 = new PointF(left + 30, basementLine + 2);

                poly = new PointF[] { d1, d2, d3, d4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }*/
            {
                var w1 = new Point(right - 16, basementLine - 22);
                var w2 = new Point(right - 16, basementLine - 40);
                var w3 = new Point(right - 30, basementLine - 37);
                var w4 = new Point(right - 30, basementLine - 19);

                poly = new Point[] { w1, w2, w3, w4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }

        }

        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }

    [Serializable]
    class SmallHouse : Decoration
    {
        double left;
        double right;
        double top;
        double bottom;
        public int Seed;

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return this.right < left || this.left > right || this.bottom < top || this.top > bottom;
        }
        public double GetYforSorting()
        {
            return bottom;
        }
        public SmallHouse(double x, double y, int seed)
        {
            this.left = x - 50;
            this.right = x + 50;
            this.top = y - 40;
            this.bottom = y;

            this.Seed = seed;
        }
        public RealPoint[] GetBorder()
        {
            var bottomLeft = new RealPoint(left, bottom);
            var bottomRight = new RealPoint(right, bottom);
            var topLeft = new RealPoint(left + 10, top);
            var topRight = new RealPoint(right - 10, top);
            var f18 = new RealPoint(left + 50, bottom + 6);
            return new RealPoint[]
            {
                bottomLeft,
                topLeft,
                topRight,
                bottomRight,
                f18
            };
        }

        public static int BlockHeight = 60;
        public static int BasementHeight = 10;

        public void Draw(Graphics g, double yMin, double yMax)
        {
            var centerY = (this.top + this.bottom) / 2;
            if (centerY < yMin || centerY > yMax)
                return;

            var left = (int)this.left;
            var right = (int)this.right;
            var bottom = (int)this.bottom;
            var top = (int)this.top;

            var basementLine = bottom - BasementHeight;
            var roofLine = basementLine - BlockHeight;
            var roofTop = roofLine - BlockHeight / 3;

            var f1 = new Point(left, bottom);
            var f2 = new Point(left + 2, basementLine + 2);
            var f3 = new Point(left + 4, basementLine);
            var f4 = new Point(left + 8, roofLine + 2);
            var f5 = new Point(left, roofLine);
            var f6 = new Point(left + 20, roofTop);

            var f7 = new Point(left + 26, roofTop + 2);
            var f8 = new Point(left + 26, roofTop - 7);
            var f9 = new Point(left + 30, roofTop - 7);
            var f10 = new Point(left + 30, roofTop + 2);

            var f12 = new Point(right - 20, roofTop + 4);
            var f13 = new Point(right, roofLine + 4);
            var f14 = new Point(right - 8, roofLine + 6);
            var f15 = new Point(right - 4, basementLine);
            var f16 = new Point(right - 2, basementLine + 2);
            var f17 = new Point(right, bottom);
            var f18 = new Point(left + 50, bottom + 6);

            var poly = new Point[] { f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f12, f13, f14, f15, f16, f17, f18 };
            var b = Brushes.Black;
            g.FillPolygon(b, poly);
            //{
            //    var d1 = new PointF(right - 16, basementLine);
            //    var d2 = new PointF(right - 18, basementLine - 40);
            //    var d3 = new PointF(right - 32, basementLine - 38);
            //    var d4 = new PointF(right - 30, basementLine + 2);

            //    poly = new PointF[] { d1, d2, d3, d4 };
            //    b = Brushes.Gray;
            //    g.FillPolygon(b, poly);
            //}
            //{
            //    var w1 = new PointF(left + 16, basementLine - 22);
            //    var w2 = new PointF(left + 16, basementLine - 40);
            //    var w3 = new PointF(left + 30, basementLine - 38);
            //    var w4 = new PointF(left + 30, basementLine - 20);

            //    poly = new PointF[] { w1, w2, w3, w4 };
            //    b = Brushes.Gray;
            //    g.FillPolygon(b, poly);
            //}

        }

        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }

    [Serializable]
    class BigHouse : Decoration
    {
        double left;
        double right;
        double top;
        double bottom;
        public int Seed;

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return this.right < left || this.left > right || this.bottom < top || this.top > bottom +BlockHeight;
        }
        public double GetYforSorting()
        {
            return bottom;
        }
        public BigHouse(double x, double y, int seed)
        {
            this.left = x - 50;
            this.right = x + 50;
            this.top = y - BlockHeight;
            this.bottom = y;

            this.Seed = seed;
        }
        public RealPoint[] GetBorder()
        {
            var bottomLeft = new RealPoint(left, bottom);
            var bottomRight = new RealPoint(right, bottom);
            var topLeft = new RealPoint(left + 10, top);
            var topRight = new RealPoint(right - 10, top);
            var f18 = new RealPoint(left + 50, bottom + 12);

            return new RealPoint[]
            {
                bottomLeft,
                topLeft,
                topRight,
                bottomRight,
                f18
            };
        }

        public static int BlockHeight = 60;
        public static int BasementHeight = 10;

        public void Draw(Graphics g, double yMin, double yMax)
        {
            var centerY = (this.top + this.bottom) / 2;
            if (centerY < yMin || centerY > yMax)
                return;

            var left = (int)this.left;
            var right = (int)this.right;
            var bottom = (int)this.bottom;
            var top = (int)this.top;

            var basementLine = bottom - BasementHeight;
            var roofLine = basementLine - BlockHeight * 2;
            var roofTop = roofLine - BlockHeight / 3;

            var f1 = new Point(left, bottom);
            var f2 = new Point(left + 2, basementLine + 2);
            var f3 = new Point(left + 4, basementLine);
            var f4 = new Point(left + 8, roofLine + 2);
            var f5 = new Point(left, roofLine);
            var f6 = new Point(left + 20, roofTop);

            var middle = (right + left) / 2;
            var f7 = new Point(middle - 2, roofTop + 2);
            var f8 = new Point(middle - 2, roofTop - 7);
            var f9 = new Point(middle + 2, roofTop - 9);
            var f10 = new Point(middle + 2, roofTop + 1);
            var f11 = new Point(middle + 2, roofTop + 2);

            var f12 = new Point(right - 20, roofTop + 4);
            var f13 = new Point(right, roofLine + 4);
            var f14 = new Point(right - 8, roofLine + 6);
            var f15 = new Point(right - 4, basementLine);
            var f16 = new Point(right - 2, basementLine + 2);
            var f17 = new Point(right, bottom);
            var f18 = new Point(left + 50, bottom + 12);

            var poly = new Point[] { f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18 };
            var b = Brushes.Black;
            g.FillPolygon(b, poly);
        }

        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    [Serializable]
    class LongHouse : Decoration
    {
        double left;
        double right;
        double top;
        double bottom;
        public int Seed;

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return this.right < left || this.left > right || this.bottom < top || this.top > bottom;
        }
        public double GetYforSorting()
        {
            return bottom;
        }
        public LongHouse(double x, double y, int seed)
        {
            this.left = x - 100;
            this.right = x + 100;
            this.top = y - BlockHeight;
            this.bottom = y;

            this.Seed = seed;
        }
        public RealPoint[] GetBorder()
        {
            var bottomLeft = new RealPoint(left, bottom);
            var bottomRight = new RealPoint(right, bottom);
            var topLeft = new RealPoint(left + 10, top);
            var topRight = new RealPoint(right - 10, top);
            var f18 = new RealPoint(left + 50, bottom + 6);

            return new RealPoint[]
            {
                bottomLeft,
                topLeft,
                topRight,
                bottomRight,
                f18
            };
        }

        public static int BlockHeight = 60;
        public static int BasementHeight = 10;

        public void Draw(Graphics g, double yMin, double yMax)
        {
            var centerY = (this.top + this.bottom) / 2;
            if (centerY < yMin || centerY > yMax)
                return;

            var left = (int)this.left;
            var right = (int)this.right;
            var bottom = (int)this.bottom;
            var top = (int)this.top;

            var basementLine = bottom - BasementHeight;
            var roofLine = basementLine - BlockHeight;
            var roofTop = roofLine - BlockHeight / 3;

            var f1 = new Point(left, bottom);
            var f2 = new Point(left + 2, basementLine + 2);
            var f3 = new Point(left + 4, basementLine);
            var f4 = new Point(left + 8, roofLine + 2);
            var f5 = new Point(left, roofLine);
            var f6 = new Point(left + 20, roofTop);

            var f7 = new Point(right - 30, roofTop + 3);
            var f8 = new Point(right - 28, roofTop - 7);
            var f9 = new Point(right - 24, roofTop - 7);
            var f10 = new Point(right - 26, roofTop + 3);
            var f11 = new Point(right - 26, roofTop + 2);

            var f12 = new Point(right - 20, roofTop + 4);
            var f13 = new Point(right, roofLine + 4);
            var f14 = new Point(right - 8, roofLine + 6);
            var f15 = new Point(right - 4, basementLine);
            var f16 = new Point(right - 2, basementLine + 2);
            var f17 = new Point(right, bottom);
            var f18 = new Point(left + 50, bottom + 6);

            var poly = new Point[] { f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18 };
            var b = Brushes.Black;
            g.FillPolygon(b, poly);
        }

        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    [Serializable]
    class ChurchHouse : Decoration
    {
        double left;
        double middle;
        double right;
        double top;
        double bottom;

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return this.right < left || this.left > right || this.bottom < top || this.top > bottom + BlockHeight*2;
        }
        public double GetYforSorting()
        {
            return bottom;
        }
        public ChurchHouse(double x, double y)
        {
            this.left = x - 50;
            this.middle = x - 5;
            this.right = x + 50;
            this.top = y - BlockHeight;
            this.bottom = y;
        }
        public RealPoint[] GetBorder()
        {
            var bottomLeft = new RealPoint(left, bottom);
            var bottomRight = new RealPoint(right, bottom);
            var topLeft = new RealPoint(left + 10, top);
            var topRight = new RealPoint(right - 10, top);
            var f16 = new RealPoint(middle + 10, bottom + 15);
            var f17 = new RealPoint(middle - 20, bottom + 15);

            return new RealPoint[]
            {
                bottomLeft,
                topLeft,
                topRight,
                bottomRight,
                f16,
                f17
            };
        }

        public static int BlockHeight = 60;
        public static int BasementHeight = 10;

        public void Draw(Graphics g, double yMin, double yMax)
        {
            var centerY = (this.top + this.bottom) / 2;
            if (centerY < yMin || centerY > yMax)
                return;

            var left = (int)this.left;
            var middle = (int)this.middle;
            var right = (int)this.right;
            var bottom = (int)this.bottom;
            var top = (int)this.top;

            var basementLine = bottom - BasementHeight;
            var secondFloor = basementLine - BlockHeight;
            var roofLine = secondFloor - BlockHeight;
            var roofTop = roofLine - BlockHeight;

            var f1 = new Point(left, bottom);
            var f2 = new Point(left + 2, basementLine + 2);
            var f3 = new Point(left + 4, basementLine);
            var f4 = new Point(left + 4, roofLine + 2);
            var f5 = new Point(left, roofLine);
            var f6 = new Point(left + 20, roofTop);

            var f7 = new Point(middle - 20, roofTop);
            var f8 = new Point(middle, roofLine);
            var f9 = new Point(middle - 4, roofLine + 2);
            var f10 = new Point(middle, secondFloor - 15);
            var f11 = new Point(right, secondFloor);
            var f12 = new Point(right - 8, secondFloor + 2);


            var f13 = new Point(right - 4, basementLine);
            var f14 = new Point(right - 2, basementLine + 2);
            var f15 = new Point(right, bottom);
            var f16 = new Point(middle+10, bottom + 15);
            var f17 = new Point(middle-20, bottom + 15);

            var poly = new Point[] { f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17 };

            var b = Brushes.Black;

            g.FillPolygon(b, poly);

            g.DrawLine(Fence.pen, (left + middle) / 2 - 4, roofTop - 8, (left + middle) / 2 + 4, roofTop - 8);
            g.DrawLine(Fence.pen, (left + middle) / 2, roofTop, (left + middle) / 2, roofTop - 12);


            var d1 = new Point(left + 14, basementLine);
            var d2 = new Point(left + 14, basementLine - 40);
            var d3 = new Point(left + 22, basementLine - 36);
            var d4 = new Point(left + 22, basementLine + 5);

            poly = new Point[] { d1, d2, d3, d4 };
            b = Brushes.Gray;

            g.FillPolygon(b, poly);

            return;
        }

        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    [Serializable]
    class DoctorsHouse : Decoration
    {
        double left;
        double middle;
        double right;
        double top;
        double bottom;

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return this.right < left || this.left > right || this.bottom < top || this.top > bottom + 2*BlockHeight;
        }
        public double GetYforSorting()
        {
            return bottom;
        }
        public DoctorsHouse(double x, double y)
        {
            this.left = x - 75;
            this.middle = x + 20;
            this.right = x + 75;
            this.top = y - BlockHeight;
            this.bottom = y;
        }
        public RealPoint[] GetBorder()
        {
            var bottomLeft = new RealPoint(left, bottom);
            var bottomRight = new RealPoint(right, bottom);
            var topLeft = new RealPoint(left + 10, top);
            var topRight = new RealPoint(right - 10, top);
            var f20 = new RealPoint(middle, bottom + 15);

            return new RealPoint[]
            {
                bottomLeft,
                topLeft,
                topRight,
                bottomRight,
                f20
            };
        }

        public static int BlockHeight = 60;
        public static int BasementHeight = 10;

        public void Draw(Graphics g, double yMin, double yMax)
        {
            var centerY = (this.top + this.bottom) / 2;
            if (centerY < yMin || centerY > yMax)
                return;

            var left = (int)this.left;
            var middle = (int)this.middle;
            var right = (int)this.right;
            var bottom = (int)this.bottom;
            var top = (int)this.top;

            var basementLine = bottom - BasementHeight;
            var secondFloor = basementLine - BlockHeight;
            var roofLine = secondFloor - BlockHeight;
            var roofTop = roofLine - BlockHeight;

            var f1 = new Point(left, bottom);
            var f2 = new Point(left + 2, basementLine + 2);
            var f3 = new Point(left + 4, basementLine);
            var f4 = new Point(left + 4, roofLine + 2);
            var f5 = new Point(left, roofLine);
            var f6 = new Point(left + 20, roofTop);

            var f7 = new Point(middle - 30, roofTop);
            var f8 = new Point(middle - 30, roofTop - 10);
            var f9 = new Point(middle - 24, roofTop - 10);
            var f10 = new Point(middle - 24, roofTop);


            var f11 = new Point(middle - 20, roofTop);
            var f12 = new Point(middle, roofLine);
            var f13 = new Point(middle - 4, roofLine + 2);
            var f14 = new Point(middle, secondFloor - 15);
            var f15 = new Point(right, secondFloor);
            var f16 = new Point(right - 8, secondFloor + 2);


            var f17 = new Point(right - 4, basementLine);
            var f18 = new Point(right - 2, basementLine + 2);
            var f19 = new Point(right, bottom);
            var f20 = new Point(middle, bottom + 15);

            var poly = new Point[] { f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, f19, f20 };
            var b = Brushes.Black;
            g.FillPolygon(b, poly);

            {
                var d1 = new Point(left + 16, basementLine);
                var d2 = new Point(left + 16, basementLine - 40);
                var d3 = new Point(left + 30, basementLine - 38);
                var d4 = new Point(left + 30, basementLine + 2);

                poly = new Point[] { d1, d2, d3, d4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }
            {
                var w1 = new Point(middle - 16, secondFloor - 32);
                var w2 = new Point(middle - 16, secondFloor - 52);
                var w3 = new Point(middle - 30, secondFloor - 50);
                var w4 = new Point(middle - 30, secondFloor - 30);

                poly = new Point[] { w1, w2, w3, w4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }
            {
                var w1 = new Point(right - 16, basementLine - 22);
                var w2 = new Point(right - 16, basementLine - 40);
                var w3 = new Point(right - 30, basementLine - 36);
                var w4 = new Point(right - 30, basementLine - 18);

                poly = new Point[] { w1, w2, w3, w4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }

        }

        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }
    [Serializable]
    class CoalHouse : Decoration
    {
        double left;
        double right;
        double top;
        double bottom;
        public int Seed;

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return this.right < left || this.left > right || this.bottom < top || this.top > bottom + 2*BlockHeight;
        }
        public double GetYforSorting()
        {
            return bottom;
        }
        public CoalHouse(double x, double y, int seed)
        {
            this.left = x - 100;
            this.right = x + 100;
            this.top = y - BlockHeight;
            this.bottom = y;

            this.Seed = seed;
        }
        public RealPoint[] GetBorder()
        {
            var bottomLeft = new RealPoint(left, bottom);
            var bottomRight = new RealPoint(right, bottom);
            var topLeft = new RealPoint(left + 10, top);
            var topRight = new RealPoint(right - 10, top);
            var f18 = new RealPoint(left + 50, bottom + 20);

            return new RealPoint[]
            {
                bottomLeft,
                topLeft,
                topRight,
                bottomRight,
                f18
            };
        }

        public static int BlockHeight = 60;
        public static int BasementHeight = 10;

        public void Draw(Graphics g, double yMin, double yMax)
        {
            var centerY = (this.top + this.bottom) / 2;
            if (centerY < yMin || centerY > yMax)
                return;

            var left = (int)this.left;
            var right = (int)this.right;
            var bottom = (int)this.bottom;
            var top = (int)this.top;

            var basementLine = bottom - BasementHeight;
            var secondFloorLine = basementLine - BlockHeight;
            var roofLine = secondFloorLine - BlockHeight;
            var roofTop = roofLine - BlockHeight / 3;

            var f1 = new Point(left, bottom);
            var f2 = new Point(left + 2, basementLine + 2);
            var f3 = new Point(left + 4, basementLine);
            var f4 = new Point(left + 8, secondFloorLine + 2);
            var f5 = new Point(left, secondFloorLine);
            var f6 = new Point(left + 20, secondFloorLine - 10);

            var ff4 = new Point(left + 24, roofLine + 2);
            var ff5 = new Point(left + 16, roofLine);
            var ff6 = new Point(left + 40, roofTop);

            var ff7 = new Point(left + 46, roofTop);
            var ff8 = new Point(left + 46, roofTop - 7);
            var ff9 = new Point(left + 50, roofTop - 7);
            var ff10 = new Point(left + 50, roofTop);

            var middle = (right + left) / 2;
            var f7 = new Point(middle - 2, roofTop + 2);
            var f8 = new Point(middle - 2, roofTop - 7);
            var f9 = new Point(middle + 2, roofTop - 8);
            var f10 = new Point(middle + 2, roofTop + 1);
            var f11 = new Point(middle + 2, roofTop + 2);

            var ff12 = new Point(right - 40, roofTop + 4);
            var ff13 = new Point(right -20, roofLine + 4);
            var ff14 = new Point(right - 28, roofLine + 6);

            var f12 = new Point(right - 25, secondFloorLine - 10);
            var f13 = new Point(right, secondFloorLine + 4);
            var f14 = new Point(right - 8, secondFloorLine + 6);
            var f15 = new Point(right - 4, basementLine);
            var f16 = new Point(right - 2, basementLine + 2);
            var f17 = new Point(right, bottom);
            var f18 = new Point(left + 50, bottom + 20);

            var poly = new Point[] { f1, f2, f3, f4, f5, f6, ff4, ff5, ff6, ff7, ff8, ff9, ff10, f7, f8, f9, f10, f11, ff12, ff13, ff14, f12, f13, f14, f15, f16, f17, f18 };
            var b = Brushes.Black;
            g.FillPolygon(b, poly);
            {
                var d1 = new Point(right - 36, basementLine);
                var d2 = new Point(right - 36, basementLine - 40);
                var d3 = new Point(right - 52, basementLine - 38);
                var d4 = new Point(right - 52, basementLine + 2);

                poly = new Point[] { d1, d2, d3, d4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }
            {
                var w1 = new Point(right - 56, basementLine - BlockHeight - 22);
                var w2 = new Point(right - 56, basementLine - BlockHeight - 40);
                var w3 = new Point(right - 70, basementLine - BlockHeight - 39);
                var w4 = new Point(right - 70, basementLine - BlockHeight - 21);

                poly = new Point[] { w1, w2, w3, w4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }
            {
                var w1 = new Point(right - 86, basementLine - BlockHeight - 21);
                var w2 = new Point(right - 86, basementLine - BlockHeight - 39);
                var w3 = new Point(right - 100, basementLine - BlockHeight - 38);
                var w4 = new Point(right - 100, basementLine - BlockHeight - 20);

                poly = new Point[] { w1, w2, w3, w4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }
            {
                var w1 = new Point(left + 36, basementLine - BlockHeight - 22);
                var w2 = new Point(left + 37, basementLine - BlockHeight - 40);
                var w3 = new Point(left + 47, basementLine - BlockHeight - 38);
                var w4 = new Point(left + 46, basementLine - BlockHeight - 20);

                poly = new Point[] { w1, w2, w3, w4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }
        }

        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {

        }
    }

    [Serializable]
    class RatHouse : Decoration
    {
        double left;
        double middleLeft;
        double middleRight;
        double right;
        double top;
        double bottom;

        public bool IsOutOfScreen(double left, double right, double top, double bottom)
        {
            return this.right < left || this.left > right || this.bottom < top || this.top > bottom + BlockHeight*2;
        }
        public double GetYforSorting()
        {
            return bottom;
        }
        public RatHouse(double x, double y)
        {
            this.left = x - 80;
            this.middleLeft = x - 35;
            this.middleRight = x + 35;
            this.right = x + 80;
            this.top = y - BlockHeight;
            this.bottom = y;
        }
        public RealPoint[] GetBorder()
        {
            var bottomLeft = new RealPoint(left, bottom);
            var bottomRight = new RealPoint(right, bottom);
            var topLeft = new RealPoint(left + 10, top);
            var topRight = new RealPoint(right - 10, top);
            var f16 = new RealPoint(middleLeft + 10, bottom + 15);
            var f17 = new RealPoint(middleRight - 10, bottom + 15);

            return new RealPoint[]
            {
                bottomLeft,
                topLeft,
                topRight,
                bottomRight,
                f16,
                f17
            };
        }

        public static int BlockHeight = 60;
        public static int BasementHeight = 10;

        public void Draw(Graphics g, double yMin, double yMax)
        {
            var centerY = (this.top + this.bottom) / 2;
            if (centerY < yMin || centerY > yMax)
                return;

            var left = (int)this.left;
            var middleLeft = (int)this.middleLeft;
            var middleRight = (int)this.middleRight;
            var right = (int)this.right;
            var bottom = (int)this.bottom;
            var top = (int)this.top;

            var basementLine = bottom - BasementHeight;
            var secondFloor = basementLine - BlockHeight;
            var roofLine = secondFloor - BlockHeight;
            var roofTop = roofLine - BlockHeight;

            var f1 = new Point(left, bottom);
            var f2 = new Point(left + 2, basementLine + 2);
            var f3 = new Point(left + 4, basementLine);
            var f4 = new Point(left + 4, roofLine + 2);
            var f5 = new Point(left, roofLine);
            var f6 = new Point(left + 20, roofTop);

            var f7 = new Point(middleLeft - 20, roofTop);
            var f8 = new Point(middleLeft, roofLine);
            var f9 = new Point(middleLeft - 4, roofLine + 2);
            var f10 = new Point(middleLeft, secondFloor - 15);


            var f11 = new Point(middleRight, secondFloor - 15);
            var f12 = new Point(middleRight + 4, roofLine + 2);
            var f13 = new Point(middleRight, roofLine);
            var f14 = new Point(middleRight + 20, roofTop);

            var f15 = new Point(right - 20, roofTop);
            var f16 = new Point(right, roofLine);
            var f17 = new Point(right - 4, roofLine + 2);
            var f18 = new Point(right - 4, basementLine);
            var f19 = new Point(right - 2, basementLine + 2);
            var f20 = new Point(right, bottom);


            var f21 = new Point(middleRight + 20, bottom + 15);
            var f22 = new Point(middleLeft - 20, bottom + 15);

            var poly = new Point[] { f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, f19, f20, f21, f22 };

            var b = Brushes.Black;

            g.FillPolygon(b, poly);

            {
                var w1 = new Point(right - 8, basementLine - BlockHeight - 22);
                var w2 = new Point(right - 8, basementLine - BlockHeight - 40);
                var w3 = new Point(right - 14, basementLine - BlockHeight - 38);
                var w4 = new Point(right - 14, basementLine - BlockHeight - 20);

                poly = new Point[] { w1, w2, w3, w4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }
            {
                var w1 = new Point(left + 8, basementLine - BlockHeight - 22);
                var w2 = new Point(left + 8, basementLine - BlockHeight - 40);
                var w3 = new Point(left + 14, basementLine - BlockHeight - 38);
                var w4 = new Point(left + 14, basementLine - BlockHeight - 20);

                poly = new Point[] { w1, w2, w3, w4 };
                b = Brushes.Gray;
                g.FillPolygon(b, poly);
            }

            if(hasDoor)
            {
                var d1 = new Point(middleLeft + 28, basementLine + 15);
                var d2 = new Point(middleLeft + 28, basementLine - 20);
                var d3 = new Point(middleLeft + 30, basementLine - 23);
                var d4 = new Point(middleLeft + 32, basementLine - 24);
                var d5 = new Point(middleRight - 32, basementLine - 24);
                var d6 = new Point(middleRight - 30, basementLine - 23);
                var d7 = new Point(middleRight - 28, basementLine - 20);
                var d8 = new Point(middleRight - 28, basementLine + 15);

                poly = new Point[] { d1, d2, d3, d4, d5, d6, d7, d8 };
                b = Brushes.Gray;

                g.FillPolygon(b, poly);
            }
            return;
        }
        bool hasDoor;
        public void Update(double deltaTime, MapState mapState, GameState gameState)
        {
            hasDoor = !gameState.GameEnds;
        }
    }
}
