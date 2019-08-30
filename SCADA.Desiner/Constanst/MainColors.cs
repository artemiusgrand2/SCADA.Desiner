using System;
using System.Text;
using System.Windows.Media;

namespace SCADA.Desiner.Constanst
{
    struct MainColors
    {
       
        static Brush grid = new SolidColorBrush(Color.FromRgb(180, 180, 180));
        /// <summary>
        /// цвет сетки
        /// </summary>
        public static Brush Grid
        {
            get
            {
                return grid;
            }
        }
     
        static Brush fon = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        /// <summary>
        /// цвет фона
        /// </summary>
        public static Brush Fon
        {
            get
            {
                return fon;
            }
        }

        static Brush selectramka = Brushes.Red;
        /// <summary>
        /// цвет рамки выделения
        /// </summary>
        public static Brush SelectRamka
        {
            get
            {
                return selectramka;
            }
        }

        
        static Brush tabletrainramka = Brushes.Blue;
        /// <summary>
        /// цвет рамки таблицы поездов
        /// </summary>
        public static Brush TableTrainRamka
        {
            get
            {
                return tabletrainramka;
            }
        }

       
        static Brush tableautopilot = Brushes.Yellow;
        /// <summary>
        /// цвет рамки таблицы автопилота
        /// </summary>
        public static Brush TableAutoPilot
        {
            get
            {
                return tableautopilot;
            }
        }
      
        static Brush area_station = Brushes.Green;
        /// <summary>
        /// цвет рамки области станции
        /// </summary>
        public static Brush AreaStation
        {
            get
            {
                return area_station;
            }
        }
      
        static Brush area_message = Brushes.Orange;
        /// <summary>
        /// цвет рамки области справки
        /// </summary>
        public static Brush AreaMessage
        {
            get
            {
                return area_message;
            }
        }
    }
}
