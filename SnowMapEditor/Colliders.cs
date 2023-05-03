using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SnowMapEditor
{
    static class MapState
    {
        public static double LegLength = 8;
    }
    interface Collider
    {
        Editable Owner
        {
            get;
            set;
        }
        RealPoint Fixate(RealPoint point);
        void DrawAntiCollider(Graphics g, bool dark);
    }
    static class ColliderWorks
    {
        public static IEnumerable<Collider> Reorder (IEnumerable<Collider> colliders)
        {
            return colliders.Where(c => c is SegmentCollider).Concat(colliders.Where(c => c is CircleCollider));
        }
        public static IEnumerable<Collider> GeneratePolygon(params RealPoint[] polygon)
        {
            if(polygon.Length <= 0)
                return new Collider[0];
            var colliders = new List<Collider>();
            var first = polygon[0];
            var poly = polygon.Concat(new RealPoint[] { first }).ToArray();
            for(int i = 1; i< poly.Length; i++)
            {
                var p1 = poly[i-1];
                var p2 = poly[i];
                colliders.Add(new CircleCollider(p2,0));
                colliders.Add(new SegmentCollider(p1, p2));
            }
            return colliders;
        }
        public static IEnumerable<Collider> GeneratePolyline(double r, params RealPoint[] polyline)
        {
            if (polyline.Length <= 0)
                return new Collider[0];
            var colliders = new List<Collider>();
            var first = polyline[0];
            colliders.Add(new CircleCollider(first, r));
            for (int i = 1; i < polyline.Length; i++)
            {
                var p1 = polyline[i - 1];
                var p2 = polyline[i];
                colliders.Add(new CircleCollider(p2, r));
                colliders.Add(new SegmentCollider(p1, p2) { Radius = r});
            }
            return colliders;
        }
        public static IEnumerable<Collider> GeneratePolyline(params RealPoint[] polyline)
        {
            return GeneratePolyline(0, polyline);
        }
        private static Tuple<Collider,RealPoint> Fixate (IEnumerable<Collider> colliders, RealPoint point)
        {
            var Best = (Collider)null;
            var BEST = point;
            var best = Meta.panelData.FixDistance * Meta.panelData.FixDistance;
            if (Meta.panelData.AllowVertices)
                foreach (var Test in colliders.Where(c => c is CircleCollider))
                {
                    var TEST = Test.Fixate(point);
                    if (TEST != RealPoint.Infinity)
                    {
                        var test = (TEST - point).SquaredLength;
                        if (test < best)
                        {
                            best = test;
                            Best = Test;
                            BEST = TEST;
                        }
                    }
                }

            if (Meta.panelData.AllowEdges)
                foreach (var Test in colliders.Where(c => c is SegmentCollider))
                {
                    var TEST = Test.Fixate(point);
                    if (TEST != RealPoint.Infinity)
                    {
                        var test = (TEST - point).SquaredLength;
                        if (test < best)
                        {
                            best = test;
                            Best = Test;
                            BEST = TEST;
                        }
                    }
                }

            return new Tuple<Collider,RealPoint> (Best, BEST);
        }
        private static double GetSafeSquaredLength (RealPoint vector)
        {
            if(vector == null)
                return double.MaxValue;
            return vector.SquaredLength;
        }
        public static SearchResult Fixate(IEnumerable<Editable> editables, RealPoint click)
        {
            var anticolliders = new List<Collider>();
            foreach (var editable in editables)
            {
                var range = GeneratePolyline(editable.Polyline.ToArray());
                foreach(var c in range)
                {
                    c.Owner = editable;
                }
                anticolliders.AddRange(range);
            }
            var p = Fixate(anticolliders, click);
            var result = new SearchResult(p.Item2);
            if(p.Item1 != null)
            {
                result.owner = p.Item1.Owner;
                if(p.Item1 is CircleCollider)
                {
                    var c = p.Item1 as CircleCollider;
                    result.index = p.Item1.Owner.Polyline.IndexOf(c.Position);
                    result.exists = true;
                }
                if(p.Item1 is SegmentCollider)
                {
                    var s= p.Item1 as SegmentCollider;
                    result.index = p.Item1.Owner.Polyline.IndexOf(s.B);
                }
            }
            return result;
        }
        public class SearchResult
        {
            public Editable owner = null;
            public RealPoint position;
            public int index = -1;
            public bool exists = false;

            public SearchResult(RealPoint p)
            {
                position = p;
            }
        }
    }
    [Serializable]
    class CircleCollider : Collider
    {
        public Editable Owner
        {
            get;
            set;
        }
        public CircleCollider(RealPoint position, double radius)
        {
            this.Position = position;
            this.Radius = radius;
        }
        public RealPoint Position;
        public double Radius;
        public RealPoint Fixate (RealPoint point)
        {
            return Position;
        }
        public void DrawAntiCollider(Graphics g, bool isdark)
        {
            var b = isdark ? new SolidBrush(Color.FromArgb(255, 255, 255)) : new SolidBrush(Color.FromArgb(140, 140, 140));
            var radius = this.Radius +  (isdark ? 50 : 100);
            g.FillEllipse(b, (float)(Position.X - radius), (float)(Position.Y - radius), (float)(2 * radius), (float)(2 * radius));
        }
    }
    [Serializable]
    class SegmentCollider : Collider
    {
        public Editable Owner
        {
            get;
            set;
        }
        public RealPoint A;
        public RealPoint B;
        public double Radius;
        public SegmentCollider (RealPoint A, RealPoint B)
        {
            this.A = A;
            this.B = B;
        }
        public RealPoint Fixate(RealPoint point)
        {
            if (!Geometry.ABCisSharp(point, A, B))
                return RealPoint.Infinity;
            if (!Geometry.ABCisSharp(point, B, A))
                return RealPoint.Infinity;
            var line = new Line(A, B);
            var projection = line.GetProjection(point);
            return projection;
        }

        public void DrawAntiCollider(Graphics g, bool isdark)
        {
            var b = isdark ? new SolidBrush(Color.FromArgb(255, 255, 255)) : new SolidBrush(Color.FromArgb(140, 140, 140));
            var radius = isdark ? 50 : Radius+100;
            var perp = (B - A).Rotate90().Normalize() * radius;
            var p1 = A + perp;
            var p2 = B + perp;
            var p3 = B - perp;
            var p4 = A - perp;

            g.FillPolygon(b, new RealPoint[] { p1, p2, p3, p4 }.Select(p => (Point)(p.Round())).ToArray());
            g.FillEllipse(b, (float)(A.X - radius), (float)(A.Y - radius), (float)(2 * radius), (float)(2 * radius));
            g.FillEllipse(b, (float)(B.X - radius), (float)(B.Y - radius), (float)(2 * radius), (float)(2 * radius));
        }
    }
}
