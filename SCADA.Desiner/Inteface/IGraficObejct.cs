using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media;
using SCADA.Desiner.Enums;
using SCADA.Desiner.HelpsProject;
using SCADA.Common.Enums;

namespace SCADA.Desiner.Inteface
{
    public interface IGraficObejct
    {
        #region Свойства

        bool IsVisible { get; set; }
        /// <summary>
        /// толщина линиии
        /// </summary>
        double StrokeThickness { get; set;}
        /// <summary>
        /// Размеры рамки выделения
        /// </summary>
        double RamkaAllocation { get; }
        /// <summary>
        /// геометрия рисуемой фигуры
        /// </summary>
        PathGeometry Figure { get; set; }
        /// <summary>
        /// выделен ли объект
        /// </summary>
        bool Selectobject { get; set; }
        /// <summary>
        /// выделена ли рамка объекта
        /// </summary>
        bool SelectStroke { get; set; }
        /// <summary>
        /// коллекция линий
        /// </summary>
        List<Line> Lines { get; }
        /// <summary>
        /// коллекция вершин
        /// </summary>
        PointCollection Points { get; }
        /// <summary>
        /// Ид объекта
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// угол поворота текста
        /// </summary>
        double RotateText { get; set; }
        /// <summary>
        /// текущая выделенная линия или точка
        /// </summary>
        Index Index { get; set; }
        /// <summary>
        /// Название объекта
        /// </summary>
        string NameObject { get; set; }
        /// <summary>
        /// Пояснения 
        /// </summary>
        string Notes { get; set; }
        /// <summary>
        /// принадлежность к номеру станции
        /// </summary>
        int StationNumber { get; set; }
        /// <summary>
        /// для перегона номер станции справа
        /// </summary>
        int StationNumberRight { get; set; }
        /// <summary>
        /// составное название участков приближения
        /// </summary>
        string Graniza { get; set; }
        /// <summary>
        /// Вид элемента
        /// </summary>
        ViewElement ViewElement { get; }
        /// <summary>
        /// Индекс слоя 
        /// </summary>
        int ZIndex { get; set; }

        #endregion



        #region Функции

        /// <summary>
        /// устанавливаем цвет объекта по умолчанию
        /// </summary>
        void DefaultColor();
        /// <summary>
        /// устанавливаем цвет объекта если его выбрали
        /// </summary>
        void SelectColor();
        /// <summary>
        /// масштабируем объект
        /// </summary>
        /// <param name="scaletransform"></param>
        /// <param name="scale"></param>
        void ScrollFigure(ScaleTransform scaletransform, double scale);
        /// <summary>
        /// перемещение объекта 
        /// </summary>
        /// <param name="deltaX">смещение по оси X</param>
        /// <param name="deltaY">смещение по оси Y</param>
        void SizeFigure(double deltaX, double deltaY);
        /// <summary>
        /// создаем коллекция линий
        /// </summary>
        void CreateCollectionLines();
        /// <summary>
        /// выбираем текущий выделенный сегмент
        /// </summary>
        /// <param name="CurrentLine"></param>
        /// <returns></returns>
        Index FindIndexLine(Line CurrentLine);
        /// <summary>
        /// определяем возможность попадания на границы и ее ориентацию
        /// </summary>
        /// <param name="CurrentLine"></param>
        /// <returns></returns>
        AligmentStroke FindLineAligment(Line CurrentLine);
        /// <summary>
        /// Поворот графического элемента
        /// </summary>
        void Rotate(double angle);
        /// <summary>
        /// изменение размеров объекта
        /// </summary>
        /// <param name="deltax"></param>
        /// <param name="deltay"></param>
        void Resize(double deltax, double deltay);
        /// <summary>
        /// показываем размеры объекта
        /// </summary>
        /// <returns></returns>
        SizeElement SizeGeometry();
        /// <summary>
        /// определяем точку вставки
        /// </summary>
        /// <returns></returns>
        Point PointInsert();
        /// <summary>
        /// симметритный перенос
        /// </summary>
        /// <param name="aligmnet"></param>
        /// <param name="point"></param>
        void Mirror(AligmentMirror aligmnet, Point point);

        #endregion
    }
}
