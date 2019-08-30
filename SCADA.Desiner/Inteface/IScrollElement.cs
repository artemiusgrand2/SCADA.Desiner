using System;
using System.Windows;
using System.Windows.Media;

namespace SCADA.Desiner.Inteface
{
    public interface IScrollElement
    {
        string NameEl { get;}
        Point CentreFigure();
        double CurrencyScroll { get; set; }
        void ScrollFigure(ScaleTransform scaletransform, double scale);
        void Normal();
    }
}
