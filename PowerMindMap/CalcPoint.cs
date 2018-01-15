using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MindNoderPort
{
    public class CalcPoint
    {
        public int X = 0;
        public int Y = 0;

        public CalcPoint()
        {
            X = 0;
            Y = 0;
        }

        public CalcPoint(int x,int y)
        {
            this.X = x;
            this.Y = y;
        }

        public CalcPoint(double x, double y)
        {
            this.X = (int)x;
            this.Y = (int)y;
        }

        public CalcPoint(Point other)
        {
            this.X = (int)other.X;
            this.Y = (int)other.Y;
        }

        public CalcPoint Add(CalcPoint other)
        {
            return new CalcPoint(this.X + other.X, this.Y + other.Y);
        }

        public void ThisAdd(CalcPoint other)
        {
            this.X += other.X;
            this.Y += other.Y;
        }

        public Point ToPoint() {
            return new Point(this.X, this.Y);
        }



    }
}
