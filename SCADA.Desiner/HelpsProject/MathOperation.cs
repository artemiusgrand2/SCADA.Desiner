using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;

namespace SCADA.Desiner.HelpsProject
{
    public  class MathOperation
    {
        public static Point RoundPoint(Point point)
        {
            return new Point(Math.Round(point.X), Math.Round(point.Y));
        }
    }
}
