using System;
using System.Windows.Data;

namespace SCADA.Desiner.Converters
{
    public class RoundValue : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            WindowWork.WorkGrafic.StepValue = Math.Round((double)value);
            return WindowWork.WorkGrafic.StepValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            //double result;
            //if (Double.TryParse(value.ToString(), System.Globalization.NumberStyles.Any,
            //             culture, out result))
            //{
            //    return result;
            //}
            //else if (Double.TryParse(value.ToString().Replace(" руб.", ""), System.Globalization.NumberStyles.Any,
            //             culture, out result))
            //{
            //    return result;
            //}
            return value;
        }
    }
}
