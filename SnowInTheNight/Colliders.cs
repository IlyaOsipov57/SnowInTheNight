using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SnowInTheNight
{
    interface Collider
    {
        RealPoint Collide(RealPoint point);
        RealPoint AntiCollide(RealPoint point);
    }
    interface DrawableCollider: Collider
    {
        void DrawAntiCollider(Graphics g);
        void DrawPavement(Graphics g);
        bool Visible(RealPoint camera, double radiusSquared);
    }
    static class ColliderWorks
    {
        public static IEnumerable<Collider> Reorder(IEnumerable<Collider> colliders)
        {
            return colliders.Where(c => c is SegmentCollider).Concat(colliders.Where(c => c is CircleCollider));
        }
        public static IEnumerable<DrawableCollider> Reorder(IEnumerable<DrawableCollider> colliders)
        {
            return colliders.Where(c => c is SegmentCollider).Concat(colliders.Where(c => c is CircleCollider));
        }
        public static IEnumerable<Decoration> Reorder(IEnumerable<Decoration> decorations)
        {
            return decorations.OrderBy(d => d.GetYforSorting());
        }
        public static IEnumerable<DrawableCollider> GeneratePolygon(params RealPoint[] polygon)
        {
            if(polygon.Length <= 0)
                return new DrawableCollider[0];
            var colliders = new List<DrawableCollider>();
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
        public static IEnumerable<DrawableCollider> GeneratePolyline(double r, params RealPoint[] polyline)
        {
            if (polyline.Length <= 0)
                return new DrawableCollider[0];
            var colliders = new List<DrawableCollider>();
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
        public static IEnumerable<DrawableCollider> GeneratePolyline(params RealPoint[] polyline)
        {
            return GeneratePolyline(0, polyline);
        }
        public static RealPoint Collide (IEnumerable<Collider> colliders, RealPoint point)
        {
            var p = point;
            var y = p.Y;
            // colliders = Reorder(colliders);
            foreach (var collider in colliders)
            {
                var oldp = p;
                p += collider.Collide(p);

            }
            return p - point;
        }
        public static RealPoint AntiCollide (IEnumerable<Collider> colliders, RealPoint point)
        {
            var first = colliders.FirstOrDefault();
            if(first == null)
                return RealPoint.Zero;
            var BEST = first.AntiCollide(point);
            var best = GetSafeSquaredLength(BEST);
            foreach(var collider in colliders)
            {
                var TEST = collider.AntiCollide(point);
                var test = GetSafeSquaredLength(TEST);
                if(test < best)
                {
                    best = test;
                    BEST = TEST;
                    if (BEST == RealPoint.Zero)
                        return BEST;
                }
            }
            return BEST;
        }
        private static double GetSafeSquaredLength (RealPoint vector)
        {
            if(vector == RealPoint.Infinity)
                return double.MaxValue;
            return vector.SquaredLength;
        }

        internal static Interaction FindBestInteraction(List<Interaction> interactions, RealPoint playerPosition, RealPoint playerDirection)
        {
            var best = double.MaxValue;
            var BEST = (Interaction)null;
            foreach (var TEST in interactions)
            {
                var test = TEST.GetInteractiveDistance(playerPosition, playerDirection);
                if (test < best)
                {
                    best = test;
                    BEST = TEST;
                }
            }
            return BEST;
        }
    }
    [Serializable]
    class CircleCollider : DrawableCollider, Collider
    {
        public CircleCollider(RealPoint position, double radius)
        {
            this.Position = position;
            this.Radius = radius;
        }
        public RealPoint Position;
        public double Radius;
        public RealPoint Collide (RealPoint point)
        {
            var delta = point - Position;
            var radius = Radius + MapState.LegLength;
            var length = delta.Length;
            if (radius <= length)
                return RealPoint.Zero;
            return delta.Normalize() * (radius - length);
        }
        public RealPoint AntiCollide (RealPoint point)
        {
            var delta = point - Position;
            var radius = Radius + MapState.LegLength;
            var length = delta.Length;
            if (radius >= length)
                return RealPoint.Zero;
            return delta.Normalize() * (radius - length);
        }
        public void DrawAntiCollider(Graphics g)
        {
            var b = new SolidBrush(Color.FromArgb(140, 140, 140));
            var radius = this.Radius +100;
            g.FillEllipse(b, (float)(Position.X - radius), (float)(Position.Y - radius), (float)(2 * radius), (float)(2 * radius));
        }
        public void DrawPavement(Graphics g)
        {
            var b = Drawer.PavementBrush;
            var radius = this.Radius;
            g.FillEllipse(b, (float)(Position.X - radius), (float)(Position.Y - radius), (float)(2 * radius), (float)(2 * radius));
        }
        public bool Visible(RealPoint camera, double radiusSquared)
        {
            return (Position - camera).SquaredLength < radiusSquared;
        }
    }
    [Serializable]
    class SegmentCollider : DrawableCollider,Collider
    {
        public RealPoint A;
        public RealPoint B;
        public double Radius = 0;
        public SegmentCollider (RealPoint A, RealPoint B)
        {
            this.A = A;
            this.B = B;
        }
        public RealPoint Collide(RealPoint point)
        {
            if (!Geometry.ABCisSharp(point, A, B))
                return RealPoint.Zero;
            if (!Geometry.ABCisSharp(point, B, A))
                return RealPoint.Zero;
            var line = new Line(A, B);
            var projection = line.GetProjection(point);
            var delta = point - projection;
            var radius = Radius + MapState.LegLength;
            var length = delta.Length;
            if (radius <= length)
                return RealPoint.Zero;
            return delta.Normalize() * (radius - length);
        }
        public RealPoint AntiCollide(RealPoint point)
        {
            if (!Geometry.ABCisSharp(point, A, B))
                return RealPoint.Infinity;
            if (!Geometry.ABCisSharp(point, B, A))
                return RealPoint.Infinity;
            var line = new Line(A, B);
            var projection = line.GetProjection(point);
            var delta = point - projection;
            var radius = Radius + MapState.LegLength;
            var length = delta.Length;
            if (radius >= length)
                return RealPoint.Zero;
            return delta.Normalize() * (radius - length);
        }

        public void DrawAntiCollider(Graphics g)
        {
            var b = new SolidBrush(Color.FromArgb(140, 140, 140));
            var radius = Radius + 100;
            var perp = (B - A).Rotate90().Normalize() * radius;
            var p1 = A + perp;
            var p2 = B + perp;
            var p3 = B - perp;
            var p4 = A - perp;

            g.FillPolygon(b, new RealPoint[] { p1, p2, p3, p4 }.Select(p => (Point)(p.Round())).ToArray());
            g.FillEllipse(b, (float)(A.X - radius), (float)(A.Y - radius), (float)(2 * radius), (float)(2 * radius));
            g.FillEllipse(b, (float)(B.X - radius), (float)(B.Y - radius), (float)(2 * radius), (float)(2 * radius));
        }
        public void DrawPavement(Graphics g)
        {
            var b = Drawer.PavementBrush;
            var radius = Radius;
            var perp = (B - A).Rotate90().Normalize() * radius;
            var p1 = A + perp;
            var p2 = B + perp;
            var p3 = B - perp;
            var p4 = A - perp;

            g.FillPolygon(b, new RealPoint[] { p1, p2, p3, p4 }.Select(p => (Point)(p.Round())).ToArray());
            //g.FillEllipse(b, (float)(A.X - radius), (float)(A.Y - radius), (float)(2 * radius), (float)(2 * radius));
            //g.FillEllipse(b, (float)(B.X - radius), (float)(B.Y - radius), (float)(2 * radius), (float)(2 * radius));
        }
        public bool Visible(RealPoint camera, double radiusSquared)
        {
            if (!Geometry.ABCisSharp(camera, A, B))
                return (A - camera).SquaredLength < radiusSquared;
            if (!Geometry.ABCisSharp(camera, B, A))
                return (B - camera).SquaredLength < radiusSquared;
            var line = new Line(A, B);
            return line.GetDistance(camera) < radiusSquared;
        }
    }

    interface Interactor
    {
        double GetInteractiveDistance(RealPoint point, RealPoint direction);
    }

    [Serializable]
    class CircleInteractor : Interactor
    {
        public CircleInteractor(RealPoint position, double radius)
        {
            this.Position = position;
            this.Radius = radius;
        }
        public RealPoint Position;
        public double Radius;
        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {
            var delta = Position - point;
            var radius = Radius;
            var length = delta.Length;

            if (radius < length)
                return double.MaxValue;

            return length;
        }
    }
    [Serializable]
    class DirectedInteractor : Interactor
    {
        public DirectedInteractor(RealPoint position, double radius)
        {
            this.Position = position;
            this.Radius = radius;
        }
        public RealPoint Position;
        public double Radius;
        public double Threshold = 0.5;
        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {

            var delta = Position - point;
            var radius = Radius;
            var length = delta.Length;
            
            if (radius < length)
                return double.MaxValue;
            
            delta = delta.Normalize();
            direction = direction.Normalize();
            
            if (delta * direction <= Threshold)
                return double.MaxValue;

            return length;
        }
    }
    [Serializable]
    class TargetedInteractor : Interactor
    {
        public TargetedInteractor(RealPoint position, double radius, RealPoint targetDirection)
        {
            this.Position = position;
            this.Radius = radius;
            this.TargetDirection = targetDirection;
        }
        public RealPoint TargetDirection;
        public RealPoint Position;
        public double Radius;
        public double Threshold = 0.5;
        public double GetInteractiveDistance(RealPoint point, RealPoint direction)
        {

            var delta = Position - point;
            var radius = Radius;
            var length = delta.Length;

            if (radius < length)
                return double.MaxValue;

            delta = delta.Normalize();
            direction = direction.Normalize();

            if (delta * direction <= Threshold)
                return double.MaxValue;

            var targetDirection = TargetDirection.Normalize();

            if (targetDirection * direction <= Threshold)
                return double.MaxValue;

            return length;
        }
    }
}
