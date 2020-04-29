using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapes;
namespace Адамов_Никита_проект_10_класса
{
    class Convex_hull
    {
        static public List<Point> Tops = new List<Point>();

        public Shape StartPoint(List<Shape> figures)
        {
            Shape Start = figures[0];

            foreach (Shape Top in figures)
            {
                if (Top.Y1 <= Start.Y1)
                {
                    if (Top.Y1 == Start.Y1)
                    {
                        if (Top.X1 <= Start.X1)
                        {
                            Start =Top;

                        }
                    }
                    else
                    {
                        Start = Top;

                    }
                }

            }

            return Start;
        }

        public  double Angle(Point One, Point Two, Point Three)
        {
            double x1 = One.X - Two.X, x2 = Three.X - Two.X;

            double y1 = One.Y - Two.Y, y2 = Three.Y - Two.Y;

            double d1 = Math.Sqrt(x1 * x1 + y1 * y1);

            double d2 = Math.Sqrt(x2 * x2 + y2 * y2);

            return Math.Acos((x1 * x2 + y1 * y2) / (d1 * d2));
        }

        public  void Draw(Graphics graphics)
        {
            if (Tops.Count <= 2) return;
            PointF[] tops = new PointF[Tops.Count];
            int i = 0;
            foreach (Point point in Tops)
                tops[i++] = new PointF((float)point.X, (float)point.Y);
            graphics.FillPolygon(new SolidBrush(Color.Red), tops);
        }
        public  double Dist(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
        /*public  void SearchForShell(List<Shape> figures)
        {
            if (figures.Count <= 2) return;
            Point Start = StartPoint(figures);
            Point Current_Point = Start;
            PointD Previous_Point = new PointD(Current_Point.X + 1, Current_Point.Y);
            PointD Next_Point = null;
            double angle, Alpha;
            Tops.Clear();
            Tops.Add(Start);
            while (true)
            {
                angle = -7;
                foreach (Shape shape in figures)
                {
                    Alpha = Angle(Previous_Point, Current_Point, new PointD(shape.X1, shape.Y1));
                    if (angle == Alpha)
                    {
                        if (Next_Point == null || Dist(Current_Point, new PointD(shape.X1, shape.Y1)) > Dist(Current_Point, Next_Point))
                        {
                            Next_Point = new PointD(shape.X1, shape.Y1);
                        }
                    }
                    if (angle < Alpha)
                    {
                        angle = Alpha;
                        Next_Point = new PointD(shape.X1, shape.Y1);
                    }
                }
                if ((Next_Point.X == Start.X && Next_Point.Y == Start.Y))
                {
                    break;
                }
                else
                {
                    Tops.Insert(0, new PointD(Next_Point.X, Next_Point.Y));
                }
                Previous_Point = new PointD(Current_Point.X, Current_Point.Y);
                Current_Point = new PointD(Next_Point.X, Next_Point.Y);
            }
        }
      /* private static bool TopsContains(PointD point)
        {
            foreach (PointD point1 in Tops)
                if (point1.X == point.X && point1.Y == point.Y) return true;
            return false;
        }
        public static void Delete_Points(List<Shape> figures)
        {
            if (figures.Count <= 2) return;

            for (int i = 0; i < figures.Count; i++)
            {
                if (!(TopsContains(new PointD(figures[i].X1, figures[i].Y1))))
                {
                    figures.RemoveAt(i);
                    i--;
                }
            }
        }*/
       /* public static bool Inside_Of_Shape(int X, int Y, List <Shape>Tops)
        {
            if (Tops.Count <= 2) return false;
            PointD Previous_point = new PointD(Tops[0].X1, Tops[0].Y1);
            double S_All = 0;
            double S_Parts = 0;
            int Index = 0;
            foreach ( Shape Current_Point in Tops)
            {

                S_All += Area(new PointD(Tops[0].X1, Tops[0].Y1), new PointD(Current_Point.X1, Current_Point.Y1), Previous_point);
                S_Parts += Area(new PointD(X, Y), new PointD(Current_Point.X1, Current_Point.Y1), Previous_point);
                Previous_point = new PointD(Current_Point.X1, Current_Point.Y1);
                if (Index == Tops.Count - 1)
                    S_Parts += Area(new PointD(X, Y), new PointD( Current_Point.X1, Current_Point.Y1), new PointD(Tops[0].X1, Tops[0].Y1));
                Index++;
            }
            
            return S_All == S_Parts;
        }*/
        private  double Area(PointD One, PointD Two, PointD Three)
        {
            return Math.Abs(((Two.X - One.X) * (Three.Y - One.Y) - (Three.X - One.X) * (Two.Y - One.Y)) / 2);
        }
    }
}