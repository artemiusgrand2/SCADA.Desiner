using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using SCADA.Desiner.Enums;

namespace SCADA.Desiner.Converters
{
    public class ScrollConvert : IMultiValueConverter
    {


        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            double scroll = (double)values[0];
            switch ((ViewObject)values[1])
            {
                case ViewObject.objectgrafic:
                    return string.Format("Масштабный коэффициент - {0}", scroll);
                case ViewObject.text:
                    return string.Format("Размер шрифта - {0}", scroll);
                default:
                    return string.Format("Толщина линии - {0}", scroll);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return new object[2];
        }

    }
}
