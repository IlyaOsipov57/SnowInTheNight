using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowMapEditor
{
    interface Editable
    {
        List<RealPoint> Polyline
        {
            get;
            set;
        }
        String ObjectTypeName
        {
            get;
        }
        String Comment
        {
            get;
            set;
        }
        void Draw(Graphics g, bool selected = false);
    }
    class Fence : Editable
    {
        public List<RealPoint> Polyline
        {
            get;
            set;
        }
        public String Comment
        {
            get;
            set;
        }
        public String ObjectTypeName
        {
            get
            {
                return "Fence";
            }
        }
        public Fence(List<RealPoint> polyline)
        {
            this.Polyline = polyline;
            this.Comment = "";
        }
        public void Draw(Graphics g, bool selected = false)
        {
            if(Polyline.Count > 0)
            {
                foreach (var pt in Polyline)
                {
                    var p = pt.Round() - new IntPoint(2,2);
                    g.FillEllipse(Brushes.Black, p.X, p.Y, 3, 3);
                }
            }
            if (Polyline.Count > 1)
            {
                if (selected || !Meta.panelData.useHQ)
                    g.DrawLines(Pens.Black, Polyline.Select(p => (Point)(p.Round())).ToArray());
                if (Meta.panelData.useHQ)
                {
                    SnowInTheNight.Hook.DrawFence(g, Polyline.Select(p => new SnowInTheNight.RealPoint(p.X, p.Y)).ToArray(), Comment);
                }
            }
        }
    }
    class Wall : Editable
    {
        public List<RealPoint> Polyline
        {
            get;
            set;
        }
        public String Comment
        {
            get;
            set;
        }
        public String ObjectTypeName
        {
            get
            {
                return "Wall";
            }
        }
        public Wall(List<RealPoint> polyline)
        {
            this.Polyline = polyline;
            this.Comment = "";
        }
        static Pen pen = new Pen(Color.Black);
        public void Draw(Graphics g, bool selected = false)
        {
            if (Polyline.Count > 0)
            {
                foreach (var pt in Polyline)
                {
                    var p = pt.Round() - new IntPoint(2, 2);
                    g.FillEllipse(Brushes.Black, p.X, p.Y, 3, 3);
                }
            }
            if (Polyline.Count > 1)
            {
                if (selected || !Meta.panelData.useHQ)
                    g.DrawLines(pen, Polyline.Select(p => (Point)(p.Round())).ToArray());
                if (Meta.panelData.useHQ)
                {
                    SnowInTheNight.Hook.DrawWall(g, Polyline.Select(p => new SnowInTheNight.RealPoint(p.X, p.Y)).ToArray(), Comment);
                }
            }
        }
    }
    class Trail : Editable
    {
        public List<RealPoint> Polyline
        {
            get;
            set;
        }
        public String ObjectTypeName
        {
            get
            {
                return "Trail";
            }
        }
        public String Comment
        {
            get;
            set;
        }
        public Trail(List<RealPoint> polyline)
        {
            this.Polyline = polyline;
            this.Comment = "";
        }
        static Pen pen;
        static Trail ()
        {
            pen = new Pen(Color.Black);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }
        public void Draw(Graphics g, bool selected = false)
        {
            if(Polyline.Count > 0)
            {
                foreach (var pt in Polyline)
                {
                    var p = pt.Round() - new IntPoint(2, 2);
                    g.FillEllipse(Brushes.Black, p.X, p.Y, 3, 3);
                }
                {
                    var pt = Polyline.Last();
                    var p = pt.Round() - new IntPoint(3, 3);
                    g.FillEllipse(Brushes.Black, p.X, p.Y, 5, 5);
                }
            }
            if (Polyline.Count > 1)
            {
                if (selected || !Meta.panelData.useHQ)
                    g.DrawLines(pen, Polyline.Select(p => (Point)(p.Round())).ToArray());
                if (Meta.panelData.useHQ)
                {
                    SnowInTheNight.Hook.DrawSteps(g, Polyline.Select(p => new SnowInTheNight.RealPoint(p.X, p.Y)).ToArray(), Comment);
                }
            }
        }
    }
    class Road : Editable
    {
        public List<RealPoint> Polyline
        {
            get;
            set;
        }
        public String ObjectTypeName
        {
            get
            {
                return "Road";
            }
        }
        public String Comment
        {
            get;
            set;
        }
        public Road(List<RealPoint> polyline)
        {
            this.Polyline = polyline;
            this.Comment = "";
        }
        static Road()
        {
            pen = new Pen(Color.Black);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        }
        static Pen pen;
        public void PreDraw1(Graphics g)
        {
            var spl = Comment.Split(':');
            if (Comment.ToUpper() != "FOLLOWAREA" && Comment.ToUpper() != "PAVED" && spl.Length == 1)
            {
                int seed = 0;
                if (!int.TryParse(spl[0], out seed))
                    seed = 0;
                var colliders = ColliderWorks.GeneratePolyline(seed, Polyline.ToArray());
                foreach (var collider in colliders)
                {
                    collider.DrawAntiCollider(g,false);
                }
            }
        }
        public void PreDraw2(Graphics g)
        {
            if (Meta.panelData.useHQ)
            {
                var spl = Comment.Split(':');

                if (Comment.ToUpper() == "FOLLOWAREA")
                {
                    var colliders = ColliderWorks.GeneratePolyline(Polyline.ToArray());
                    foreach (var collider in colliders)
                    {
                        collider.DrawAntiCollider(g, true);
                    }
                }
                if (Comment.ToUpper() == "PAVED" || (spl.Length > 1 && spl[1].ToUpper() == "PAVED"))
                {
                    SnowInTheNight.Hook.DrawRoad(g, Polyline.Select(p => new SnowInTheNight.RealPoint(p.X, p.Y)).ToArray(), Comment);
                }
            }
        }
        public void Draw(Graphics g, bool selected = false)
        {
            if(Polyline.Count > 0)
            {
                foreach (var pt in Polyline)
                {
                    var p = pt.Round() - new IntPoint(2, 2);
                    g.FillEllipse(Brushes.Black, p.X, p.Y, 3, 3);
                }
            }
            if (Polyline.Count > 1)
            {
                if (selected)
                    g.DrawLines(pen, Polyline.Select(p => (Point)(p.Round())).ToArray());
            }
        }
    }
    class Singular : Editable
    {
        public List<RealPoint> Polyline
        {
            get;
            set;
        }
        public String ObjectTypeName
        {
            get
            {
                return "Singular";
            }
        }
        public String Comment
        {
            get;
            set;
        }
        public Singular(List<RealPoint> polyline)
        {
            this.Polyline = polyline;
            this.Comment = "";
        }
        public void Draw(Graphics g, bool selected = false)
        {
            if (Meta.panelData.useHQ)
            {
                SnowInTheNight.Hook.Draw(g, new SnowInTheNight.RealPoint(Polyline[0].X, Polyline[0].Y), Comment);
            }
            var pt = Polyline[0];
            var p = pt.Round() - new IntPoint(2, 2);
            g.FillEllipse(Brushes.Black, p.X, p.Y, 3, 3);
            g.DrawEllipse(Pens.Black, p.X-2, p.Y-2, 7, 7);
        }
    }
}
