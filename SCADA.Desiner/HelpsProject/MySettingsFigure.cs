using System.Windows;

namespace SCADA.Desiner.HelpsProject
{
    public class MySettingsFigure
    {
        /// <summary>
        /// идентификатор объекта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// номер фигуры
        /// </summary>
        public int IdFigure { get; set; }
        /// <summary>
        /// номер сегмента
        /// </summary>
        public int IdSegment { get; set; }
        /// <summary>
        /// Номер точки по порядку
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Координата точки
        /// </summary>
        public string Point { get; set; }
        /// <summary>
        /// угол наклона
        /// </summary>
        public string Angle { get; set; }
        /// <summary>
        /// смещение
        /// </summary>
        public string Delta { get; set; }
        /// <summary>
        /// точка
        /// </summary>
        public Point PointValue { get; set; }
        /// <summary>
        /// является ли элемент выделенным
        /// </summary>
        public bool IsSelect { get; set; }
    }
}
