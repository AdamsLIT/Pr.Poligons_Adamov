using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapes;
namespace Адамов_Никита_проект_10_класса
{
    class Create_Triangle
    {
        PointD[] points = new PointD[3];
    
        
       public Create_Triangle(PointD P1, PointD P2, PointD P3)
        {

            points[0]=P1;
            points[1] = P2;
            points[2] = P3;
        }
        public bool Inside_of_Triangle(int X, int Y)
        {
            if (points[0] == points[1] || points[1] == points[2] || points[0] == points[2]) return false;
            double S = Math.Abs(((points[1].X - points[0].X)*(points[2].Y - points[0].Y) - (points[2].X - points[0].X)*(points[1].Y - points[0].Y))/2);
            double S1 = Math.Abs(((points[1].X - X) * (points[2].Y - Y) - (points[2].X - X) * (points[1].Y - Y)) / 2);
            double S2 = Math.Abs(((X - points[0].X) * (points[2].Y - points[0].Y) - (points[2].X - points[0].X) * (Y - points[0].Y)) / 2);
            double S3 = Math.Abs(((points[1].X - points[0].X) * (Y - points[0].Y) - (X - points[0].X) * (points[1].Y - points[0].Y)) / 2);
            return Math.Round(S1 + S2 + S3,11) <= Math.Round(S,11);
           
        }
            
    }
}
