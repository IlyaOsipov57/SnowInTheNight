using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnowInTheNight
{
    struct Line
    {
        double[] coefficients;
        public Line(RealPoint A, RealPoint B)
        {
            var angle = (B - A).Normalize();
            coefficients = new double[3];
            coefficients[0] = angle.Y;
            coefficients[1] = -angle.X;
            coefficients[2] = A ^ angle;
        }
        public Line (double y)
        {
            coefficients = new double[3];
            coefficients[0] = 0;
            coefficients[1] = 1;
            coefficients[2] = y;
        }
        public bool ISUnder (RealPoint C)
        {
            return coefficients[1] * GetDistanceWithSign(C) > 0 ;
        }
        public double GetDistance(RealPoint A)
        {
            return Math.Abs(GetDistanceWithSign(A));
        }
        private double GetDistanceWithSign(RealPoint A)
        {
            return coefficients[0] * A.X + coefficients[1] * A.Y - coefficients[2];
        }
        public RealPoint GetProjection(RealPoint A)
        {
            return A - GetDistanceWithSign(A) * new RealPoint(coefficients[0], coefficients[1]);
        }
        public static RealPoint? operator &(Line line1, Line line2)
        {
            var matrix = new double[][] { line1.coefficients, line2.coefficients };
            var determinant = Get2DDeterminant(matrix, 0, 1);
            if (Math.Abs(determinant) == 0)
                return null;
            var determinantX = Get2DDeterminant(matrix, 2, 1);
            var determinantY = Get2DDeterminant(matrix, 0, 2);
            return new RealPoint(determinantX / determinant, determinantY / determinant);
        }
        private static double Get2DDeterminant(double[][] matrix, int f, int s)
        {
            return matrix[0][f] * matrix[1][s] - matrix[0][s] * matrix[1][f];
        }
    }

    class Segment
    {
        public RealPoint A
        {
            get;
            private set;
        }
        public RealPoint B
        {
            get;
            private set;
        }
        public Segment(RealPoint A, RealPoint B)
        {
            this.A = A;
            this.B = B;
        }
        Line line
        {
            get
            {
                return new Line(A, B-A);
            }
        }
        public double GetDistance(RealPoint point)
        {
            if (!Geometry.ABCisSharp(point, A, B))
                return (point - A).Length;
            if (!Geometry.ABCisSharp(point, B, A))
                return (point - B).Length;
            return line.GetDistance(point);
        }
        public RealPoint GetProjection(RealPoint point)
        {
            if (!Geometry.ABCisSharp(point, A, B))
                return A;
            if (!Geometry.ABCisSharp(point, B, A))
                return B;
            return line.GetProjection(point);
        }
    }
    static class Geometry
    {
        public static bool ABCisSharp(RealPoint A, RealPoint B, RealPoint C)
        {
            return (A.X - B.X) * (C.X - B.X) > (B.Y - A.Y) * (C.Y - B.Y);
            //var BA = A - B;
            //var BC = C - B;
            //return BA * BC > 0;
        }
    }
}
