using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SnowMapEditor
{
    [Serializable]
    [DataContract]
    public struct RealPoint
    {
        [DataMember]
        public double X
        {
            get;
            private set;
        }
        [DataMember]
        public double Y
        {
            get;
            private set;
        }
        public RealPoint(double x, double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(SquaredLength);
            }
        }
        public double SquaredLength
        {
            get
            {
                return X * X + Y * Y;
            }
        }
        public RealPoint Rotate90()
        {
            return new RealPoint(Y, -X);
        }
        public RealPoint Normalize ()
        {
            var l = this.Length;
            if (l>0)
                return this / this.Length;
            return new RealPoint(0,1);
        }
        public IntPoint Floor()
        {
            return new IntPoint((int)Math.Floor(X), (int)Math.Floor(Y));
        }
        public IntPoint Ceiling()
        {
            return new IntPoint((int)Math.Ceiling(X), (int)Math.Ceiling(Y));
        }
        public IntPoint Round()
        {
            return new IntPoint((int)Math.Round(X), (int)Math.Round(Y));
        }

        public static RealPoint Zero
        {
            get
            {
                return new RealPoint(0, 0);
            }
        }

        public static RealPoint Infinity
        {
            get
            {
                return new RealPoint(double.PositiveInfinity, double.PositiveInfinity);
            }
        }

        public static RealPoint operator +(RealPoint A, RealPoint B)
        {
            return new RealPoint(A.X + B.X, A.Y + B.Y);
        }
        public static RealPoint operator -(RealPoint A, RealPoint B)
        {
            return new RealPoint(A.X - B.X, A.Y - B.Y);
        }
        public static RealPoint operator -(RealPoint A)
        {
            return new RealPoint(-A.X, -A.Y);
        }
        public static double operator *(RealPoint A, RealPoint B)
        {
            return A.X * B.X + A.Y * B.Y;
        }
        public static double operator ^(RealPoint A, RealPoint B)
        {
            return A.X * B.Y - B.X * A.Y;
        }
        public static RealPoint operator *(RealPoint A, double k)
        {
            return new RealPoint(A.X * k, A.Y * k);
        }
        public static RealPoint operator *(double k, RealPoint A)
        {
            return new RealPoint(A.X * k, A.Y * k);
        }
        public static RealPoint operator /(RealPoint A, double k)
        {
            return new RealPoint(A.X / k, A.Y / k);
        }
        public static bool operator !=(RealPoint A, RealPoint B)
        {
            return !(A == B);
        }
        public static bool operator ==(RealPoint A, RealPoint B)
        {
            if (System.Object.ReferenceEquals(A, B))
            {
                return true;
            }

            if (((object)A == null) || ((object)B == null))
            {
                return false;
            }

            return Eq(A, B);
        }
        private static bool Eq(RealPoint A, RealPoint B)
        {
            return A.X == B.X && A.Y == B.Y;
        }
        public override bool Equals(object obj)
        {
            var p = (RealPoint)obj;
            if ((object)p == null)
                return false;

            return Eq(this, p);
        }
        public override int GetHashCode()
        {
            return (new Tuple<double, double>(X, Y)).GetHashCode();
        }
        public override string ToString()
        {
            return String.Format("RealPoint: {0:f2}; {1:f2}", X, Y);
        }
        public static explicit operator SizeF(RealPoint p)
        {
            return new SizeF((float)p.X, (float)p.Y);
        }
        public static explicit operator PointF(RealPoint p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }
        public static explicit operator RealPoint(IntPoint p)
        {
            return new RealPoint(p.X, p.Y);
        }
        public static explicit operator RealPoint(SizeF size)
        {
            return new RealPoint(size.Height, size.Width);
        }
    }
}
