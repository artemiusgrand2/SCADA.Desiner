using System;
using System.Windows.Media;
using SCADA.Desiner.HelpsProject;

namespace SCADA.Desiner.Inteface
{
    public interface IShowSettings
    {
        /// <summary>
        /// геометрия рисуемой фигуры
        /// </summary>
        PathGeometry Figure { get; set; }
        /// <summary>
        /// текущая выделенная линия
        /// </summary>
        Index Index { get; set; }
        /// <summary>
        /// выбираем текущий выделенный сегмент
        /// </summary>
        /// <param name="CurrentLine"></param>
        /// <returns></returns>
        Index FindIndexLine(int figure, int segment);
    }
}
