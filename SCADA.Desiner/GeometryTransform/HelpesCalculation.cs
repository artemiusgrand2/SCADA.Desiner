using System;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using SCADA.Desiner.Inteface;

namespace SCADA.Desiner.GeometryTransform
{
    class HelpesCalculation
    {
        /// <summary>
        /// определяемдлину отрезка между двумя точками
        /// </summary>
        /// <param name="p1">точка один</param>
        /// <param name="p2">точка два</param>
        /// <returns></returns>
        public static double LenghtSide(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public static double FontSizeText(double textKw, double textKh, Rectangle ramka, string _text, double RotateText)
        {
            double fontsizeWr = textKw * ramka.Width / _text.Length;
            double fontsizeHr = ramka.Height * textKh;
            if ((fontsizeHr) >= (fontsizeWr))
                return fontsizeWr;
            else return fontsizeHr;
        }

        /// <summary>
        /// Выравнивание название главного пути по центру при изменении длины или ширины
        /// </summary>
        /// <returns></returns>
        public static Thickness AlingmentCenter(double ramkaX, double ramkaY, double width, double height, TextBlock _text, IGraficObejct graf)
        {
            if (graf.RotateText == 0 || Math.Abs(graf.RotateText) == 360)
                return new Thickness(ramkaX + (width - WidthText(_text)) / 2, ramkaY + (height - HeightText(_text)) / 2, 0, 0);
            //
            if (Math.Abs(graf.RotateText) == 90)
            {
                double _deltaY = (height - HeightText(_text));
                double _deltaX = (width - WidthText(_text));
                return new Thickness(ramkaX - (height - HeightText(_text)) / 2, ramkaY + (width - WidthText(_text)) / 2, 0, 0);
            }
            //
            if (Math.Abs(graf.RotateText) == 180)
                return new Thickness(ramkaX - (width - WidthText(_text)) / 2, ramkaY - (height - HeightText(_text)) / 2, 0, 0);
            //
            if (Math.Abs(graf.RotateText) == 270)
                return new Thickness(ramkaX + (height - HeightText(_text)) / 2, ramkaY - (width - WidthText(_text)) / 2, 0, 0);
            //
            return new Thickness(0, 0, 0, 0);
        }


        /// <summary>
        /// ширина текста в пикселях
        /// </summary>
        /// <param name="textblock"></param>
        /// <returns></returns>
        public static double WidthText(TextBlock textblock)
        {
            Typeface typeface = new Typeface(textblock.FontFamily, textblock.FontStyle, textblock.FontWeight, textblock.FontStretch);
            System.Windows.Media.Brush brush = new SolidColorBrush();
            FormattedText formatedText = new FormattedText(textblock.Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, textblock.FontSize, brush);
            return formatedText.Width;
        }

        /// <summary>
        /// высота текста в пикселях
        /// </summary>
        /// <param name="textblock"></param>
        /// <returns></returns>
        public static double HeightText(TextBlock textblock)
        {
            Typeface typeface = new Typeface(textblock.FontFamily, textblock.FontStyle, textblock.FontWeight, textblock.FontStretch);
            System.Windows.Media.Brush brush = new SolidColorBrush();
            FormattedText formatedText = new FormattedText(textblock.Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, textblock.FontSize, brush);
            return formatedText.Height;
        }

    }
}
