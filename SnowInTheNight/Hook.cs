using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowInTheNight
{
#if DEBUG
    public class Hook
    {
        public static String HookUpMapData()
        {
            return SnowInTheNight.Properties.Resources.MapData;
        }
        public static void Draw (Graphics g, RealPoint position, String decoration)
        {
            //Старая церковь

            var seed = 0;
            var tt = decoration.Split(':');
            var t1 = tt[0].Trim();
            var t2 = tt.Length > 1 ? tt[1] : "0";

            if (!int.TryParse(t2, out seed))
                seed = 0;

            switch (t1.ToUpper())
            {
                case "CIRCLE":
                    g.DrawEllipse(Pens.Black, (int)position.X - seed, (int)position.Y - seed, seed * 2, seed * 2);
                    break;
                case "FIREPLACE":
                    var fire = new FirePlace(position, seed);
                    fire.Draw(g,double.MinValue,double.MaxValue);
                    break;
                case "MAID":
                    var maid = new Maid(position);
                    maid.Draw(g,double.MinValue,double.MaxValue);
                    break;
                case "HOME":
                    var home = new DoctorsHouse(position.X, position.Y);
                    home.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "SLED":
                    var sled = new SledDecoration(position);
                    sled.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "BELL":
                    var bell = new BellGate(position);
                    bell.Draw(g, double.MinValue, double.MaxValue);
                    for (var r = 400; r <= 700; r += 100)
                    {
                        g.DrawEllipse(Pens.Black, (int)position.X - r, (int)position.Y - r, 2 * r, 2 * r);
                    }
                    break;
                case "CHURCH":
                    var church = new ChurchHouse(position.X, position.Y);
                    church.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "GRAVE":
                    var grave = new GraveWood(position, seed);
                    grave.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "GRAVESTONE":
                    var gravestone = new GraveStone(position, seed);
                    gravestone.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "HOUSE1":
                    var house1 = new SmallHouse(position.X, position.Y, seed);
                    house1.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "HOUSE2":
                    var house2 = new BigHouse(position.X, position.Y, seed);
                    house2.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "HOUSE3":
                    var house3 = new LongHouse(position.X, position.Y, seed);
                    house3.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "HOUSE4":
                    var house4 = new KeepersHouse(position.X, position.Y, seed);
                    house4.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "RATHOUSE":
                    var ratHouse = new RatHouse(position.X, position.Y);
                    ratHouse.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "BEGGAR":
                    if (t2.ToUpper() == "DEAD")
                    {
                        var beggar = new DeadBeggar(position);
                        beggar.Exists = true;
                        beggar.Draw(g, double.MinValue, double.MaxValue);
                    }
                    else
                    {
                        var beggar = new Beggar(position);
                        beggar.Draw(g, double.MinValue, double.MaxValue);
                    }
                    break;
                case "TREE":
                    var tree = new Tree(position, seed);
                    tree.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "COALHOUSE":
                    var coalHouse = new CoalHouse(position.X, position.Y, seed);
                    coalHouse.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "BUCKET":
                    var bucket = new LostBucket(position);
                    bucket.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "ANOMALY":
                    {
                        var r = 400;
                        g.DrawLine(Pens.Black, (int)position.X - r + 100, (int)position.Y - r, (int)position.X + r + 100, (int)position.Y - r);
                        g.DrawLine(Pens.Black, (int)position.X - r - 100, (int)position.Y + r, (int)position.X + r - 100, (int)position.Y + r);
                        var pen = new Pen(Color.Black);
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                        foreach (var rr in new int[] { 300, 350, 450, 500 })
                        {
                            g.DrawLine(pen, (int)position.X - r + 100, (int)position.Y - rr, (int)position.X + r + 100, (int)position.Y - rr);
                            g.DrawLine(pen, (int)position.X - r - 100, (int)position.Y + rr, (int)position.X + r - 100, (int)position.Y + rr);
                        }
                    }
                    break;
                case "ELDER":
                    var elder = new Elder(position);
                    elder.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "STUMP":
                    var stump = new Stump(position, seed);
                    stump.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "POST":
                    var post = new PostSign(position);
                    post.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "CART":
                    var cart = new Cart(position);
                    cart.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "DOGHOUSE":
                    var dogHouse = new DogHouse(position);
                    dogHouse.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "STABLE":
                    var stable = new Stable(position);
                    stable.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "CRYPT":
                    var tomb = new Tomb(position);
                    tomb.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "DOOR":
                    var door = new CastleDoor(position);
                    door.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "WELL":
                    var well = new Well(position);
                    well.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "MADMAN":
                    var madman = new Madman(position);
                    madman.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "GHOST":
                    var ghost = new Ghost(position);
                    ghost.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "MILL":
                    var mill = new Mill(position);
                    mill.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "GUARD":
                    var guard = new Guard(position);
                    guard.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "NOMAD":
                    var nomad = new Nomad(position);
                    nomad.Draw(g, double.MinValue, double.MaxValue);
                    break;
                case "MAPTEXT":
                    var mapText = new MapText(position, seed);
                    mapText.Draw(g);
                    break;
            }
        }
        public static void DrawPlayer(Graphics g, RealPoint position)
        {
            var p = new Drawer.PlayerDrawer(position - new RealPoint(MapState.LegLength, 0), position + new RealPoint(MapState.LegLength, 0))
            {
                IsLeftStep = false,
                Health = 1
            };

            p.DrawPlayer(g);
        }
        public static void DrawSteps (Graphics g, RealPoint[] polyline, String comment)
        {
            var spl = comment.Split(':');
            int seed = 0;
            if (!int.TryParse(spl[0], out seed))
                seed = 0;
            if (spl.Length > 1 && spl[1].ToUpper() == "DARKDOCTOR")
            {
                var steps = StepsGenerator.Generate(polyline, seed, 0);
                Drawer.DrawSteps(g, double.MaxValue, steps);
            }
            else
            {
                var steps = StepsGenerator.Generate(polyline, seed, 0.3);
                Drawer.DrawSteps(g, double.MaxValue, steps);
            }
        }
        public static void DrawFence(Graphics g, RealPoint[] polyline, String comment)
        {
            int seed = 0;
            if (!int.TryParse(comment, out seed))
                seed = 0;
            var f = new Fence(polyline);
            f.Seed = seed;
            f.Draw(g, double.MinValue, double.MaxValue);
        }
        public static void DrawWall(Graphics g, RealPoint[] polyline, String comment)
        {
            int seed = 0;
            if (!int.TryParse(comment, out seed))
                seed = 0;
            var w = new Wall(polyline);
            w.Seed = seed;
            w.Draw(g, double.MinValue, double.MaxValue);
        }
        public static void DrawRoad(Graphics g, RealPoint[] polyline, String comment)
        {
            var spl = comment.Split(':');
            int seed = 50;
            if (!int.TryParse(spl[0], out seed))
                seed = 50;
            foreach ( var pavement in ColliderWorks.GeneratePolyline(seed, polyline))
            {
                pavement.DrawPavement(g);
            }
        }
    }
#endif
}
