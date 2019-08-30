using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Enums;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.GeometryTransform;
using SCADA.Desiner.WindowWork;
using SCADA.Common.Enums;

namespace SCADA.Desiner.BaseElement
{

    /// <summary>
    /// класс описывающий главный путь
    /// </summary>
    public class DiagnostikCell : Shape, IGraficObejct, IResize
    {
        #region Переменные и свойства

        private bool isvisible = true;
        /// <summary>
        /// Оттображать ли объект
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return isvisible;
            }
            set
            {
                isvisible = value;
            }
        }
        /// <summary>
        /// коэффициент увеличения
        /// </summary>
        private double _sctollrectangle = 10;
        public double RamkaAllocation
        {
            get
            {
                return _sctollrectangle;
            }
        }
        public int StationNumberRight { get; set; }


        public string Graniza { get; set; }
        /// <summary>
        /// отрисовываемая геометрия
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                return _figure;
            }
        }
        /// <summary>
        /// номер станции
        /// </summary>
        public int StationNumber { get; set; }
        private PathGeometry _figure = new PathGeometry();
        /// <summary>
        /// геометрическое иписание фигуры
        /// </summary>
        public PathGeometry Figure
        {
            get
            {
                return _figure;
            }
            set
            {
                _figure = value;
            }
        }
        private string nameObject = string.Empty;
        /// <summary>
        /// название пути
        /// </summary>
        public string NameObject
        {
            get
            {
                return nameObject;
            }
            set
            {
                nameObject = value;
            }
        }
        double _rotatetext = 0;
        //показываем повернут ли текст
        public double RotateText
        {
            get
            {
                return _rotatetext;
            }
            set
            {
                _rotatetext = value;
            }
        }

        /// <summary>
        /// пояснения
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Ид объекта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ширина по умолчанию
        /// </summary>
        double widthdefult = 15;
        /// <summary>
        /// высота по умолчанию
        /// </summary>
        double heightdefult = 15;
        /// <summary>
        /// минимальная длина грани
        /// </summary>
        double m_min = 10;
        /// <summary>
        /// толщина линии  по умолчанию
        /// </summary>
        public static double strokethickness = 1;
        /// <summary>
        /// цвет заполнения по умолчанию
        /// </summary>
        Brush _colordefult = new SolidColorBrush(Color.FromRgb(170, 170, 170));
        /// <summary>
        /// цвет выделенного объекта
        /// </summary>
        Brush _colorselect = new SolidColorBrush(Color.FromArgb(190, 170, 0, 170));
        /// <summary>
        /// цвет выделенного контура
        /// </summary>
        Brush _colorselectstroke = Brushes.Red;
        /// <summary>
        /// цвет контура по умолчанию
        /// </summary>
        Brush _colordefultstroke = Brushes.Black;

        /// <summary>
        /// переменная показывает выбрана ли рамка для редактирования
        /// </summary>
        private bool m_selectstroke = false;
        public bool SelectStroke
        {
            get
            {
                return m_selectstroke;
            }
            set
            {
                m_selectstroke = value;
            }
        }
        /// <summary>
        /// переменная показывает выбрана ли сам объект
        /// </summary>
        private bool m_selectobject = false;
        public bool Selectobject
        {
            get
            {
                return m_selectobject;
            }
            set
            {
                m_selectobject = value;
            }
        }
        /// <summary>
        /// коллекция используемых линий
        /// </summary>
        List<Line> _lines = new List<Line>();
        public List<Line> Lines
        {
            get
            {
                return _lines;
            }
        }
        /// <summary>
        /// коллекция используемых точек
        /// </summary>
        PointCollection _points = new PointCollection();
        public PointCollection Points
        {
            get
            {
                return _points;
            }
        }
        Index _index;
        /// <summary>
        /// текущая линия выделения
        /// </summary>
        public Index Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }
        ViewElement viewElement = ViewElement.diagnostikCell;
        /// <summary>
        /// Вид элемента
        /// </summary>
        public ViewElement ViewElement
        {
            get
            {
                return viewElement;
            }
        }
        /// <summary>
        /// индеск слоя наложения объектов
        /// </summary>
        public int ZIndex { get; set; }

        #endregion

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="kscroll">масштаб</param>
        /// <param name="stationnumber">номер станции</param>
        /// <param name="X">координата X точки вставки</param>
        /// <param name="Y">координата Y точки вставки</param>
        public DiagnostikCell(double kscroll, int stationnumber, double X, double Y)
        {
            StationNumber = stationnumber;
            StartSize();
            GeometryFigure(kscroll, X, Y);
        }

        public DiagnostikCell(int stationnumber, PathGeometry geometry, double strokethickness, string namePath)
        {
            StationNumber = stationnumber;
            this.nameObject = namePath;
            GeometryFigureCopy(geometry, strokethickness);
        }

        public SizeElement SizeGeometry()
        {
            Point P1 = ((LineSegment)Figure.Figures[0].Segments[0]).Point;
            Point P2 = ((LineSegment)Figure.Figures[0].Segments[1]).Point;
            Point P3 = ((LineSegment)Figure.Figures[0].Segments[2]).Point;
            return new SizeElement()
            {
                Height = Math.Round(Math.Sqrt(Math.Pow((Figure.Figures[0].StartPoint.Y - P1.Y), 2) + Math.Pow((Figure.Figures[0].StartPoint.X - P1.X), 2)) / OperationsGrafic.SaveScroll),
                Widht = Math.Round(Math.Sqrt(Math.Pow((P2.X - P3.X), 2) + Math.Pow((P2.Y - P3.Y), 2)) / OperationsGrafic.SaveScroll)
            };
        }

        public Point PointInsert()
        {
            return new Point(_figure.Figures[0].StartPoint.X / System.Windows.SystemParameters.CaretWidth, _figure.Figures[0].StartPoint.Y / System.Windows.SystemParameters.CaretWidth);
        }

        public void Resize(double deltax, double deltay)
        {
            try
            {
                if (Index != null)
                {
                    if (Index.XY)
                        deltay = 0;
                    else deltax = 0;
                    //
                    NewKoordinate(Index.IndexFigure, Index.IndexSegment, deltax, deltay);
                }
            }
            catch { }
        }

        private void NewKoordinate(int figure, int segment, double deltax, double deltay)
        {
            var lin1 = _figure.Figures[figure].Segments[segment] as LineSegment;
            var lin2 = _figure.Figures[figure].Segments[((segment + 1) < _figure.Figures[figure].Segments.Count) ? segment + 1 : 0] as LineSegment;
            var predLine = _figure.Figures[figure].Segments[((segment - 1) >= 0) ? segment - 1 : _figure.Figures[figure].Segments.Count - 1] as LineSegment;
            if (LenghtStorona(predLine.Point, new Point(lin1.Point.X + deltax, lin1.Point.Y + deltay)) >= m_min)
            {
                lin1.Point = new Point(lin1.Point.X + deltax, lin1.Point.Y + deltay);
                predLine.Point = new Point(predLine.Point.X + deltax, predLine.Point.Y + deltay);
            }
        }

        public void Newsize(double newwidth, double newheight)
        {
            //try
            //{
            //    //изменяем длину
            //    if (newwidth >= m_min)
            //    {
            //        LineSegment lin = _figure.Figures[0].Segments[5] as LineSegment;
            //        ArcSegment arc = _figure.Figures[0].Segments[0] as ArcSegment;
            //        double delta = (newwidth - LenghtStorona(arc.Point, new Point(lin.Point.X, lin.Point.Y)));
            //        if (Math.Abs(lin.Point.Y - arc.Point.Y) < Math.Abs(lin.Point.X - arc.Point.X))
            //        {
            //            if (lin.Point.Y > arc.Point.Y)
            //                NewKoordinate(0, 5, -delta, 0);
            //            else NewKoordinate(0, 5, delta, 0);
            //        }
            //        else
            //        {
            //            double len1 = LenghtStorona(arc.Point, new Point(lin.Point.X, lin.Point.Y + delta));
            //            double len2 = LenghtStorona(arc.Point, new Point(lin.Point.X, lin.Point.Y - delta));
            //            if (Math.Abs(newwidth - len1) < Math.Abs(newwidth - len2))
            //                NewKoordinate(0, 5, 0, delta);
            //            else NewKoordinate(0, 5, 0, -delta);
            //        }

            //    }
            //    //изменяем высоту
            //    if (newheight >= _minheight)
            //    {
            //        LineSegment lin = _figure.Figures[0].Segments[3] as LineSegment;
            //        ArcSegment arc = _figure.Figures[0].Segments[6] as ArcSegment;
            //        double delta = (newheight - LenghtStorona(arc.Point, new Point(lin.Point.X, lin.Point.Y)));
            //        if (Math.Abs(lin.Point.Y - arc.Point.Y) > Math.Abs(lin.Point.X - arc.Point.X))
            //        {
            //            if (lin.Point.Y > arc.Point.Y)
            //                NewKoordinate(0, 3, 0, delta);
            //            else NewKoordinate(0, 3, 0, -delta);
            //        }
            //        else
            //        {
            //            double len1 = LenghtStorona(arc.Point, new Point(lin.Point.X + delta, lin.Point.Y));
            //            double len2 = LenghtStorona(arc.Point, new Point(lin.Point.X - delta, lin.Point.Y));
            //            if (Math.Abs(newheight - len1) < Math.Abs(newheight - len2))
            //                NewKoordinate(0, 3, delta, 0);
            //            else NewKoordinate(0, 3, -delta, 0);
            //        }
            //    }
            //}
            //catch { }
        }

        private void StartSize()
        {
            widthdefult *= OperationsGrafic.SaveScroll;
            heightdefult *= OperationsGrafic.SaveScroll;
        }



        /// <summary>
        /// определяем длину отрезка между двумя точками
        /// </summary>
        /// <param name="p1">точка один</param>
        /// <param name="p2">точка два</param>
        /// <returns></returns>
        private double LenghtStorona(Point p1, Point p2)
        {
            double result = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            return result;
        }
        /// <summary>
        /// находим нужный сегмент для редактирования
        /// </summary>
        /// <param name="CurrentLine">текущая выбранная линия</param>
        /// <returns></returns>
        public Index FindIndexLine(Line CurrentLine)
        {
            //
            for (int i = 0; i < _figure.Figures.Count; i++)
            {
                for (int j = 0; j < _figure.Figures[i].Segments.Count; j++)
                {
                    LineSegment lin = _figure.Figures[i].Segments[j] as LineSegment;
                    if (lin != null && lin.Point.X == CurrentLine.X2 && lin.Point.Y == CurrentLine.Y2)
                    {
                        if (Math.Abs(CurrentLine.Y1 - CurrentLine.Y2) < Math.Abs(CurrentLine.X1 - CurrentLine.X2))
                            return new Index() { IndexFigure = i, IndexSegment = j, XY = false };
                        else
                            return new Index() { IndexFigure = i, IndexSegment = j, XY = true };
                    }

                }
            }
            return null;
            //
        }

        public AligmentStroke FindLineAligment(Line CurrentLine)
        {
            for (int i = 0; i < _figure.Figures.Count; i++)
            {
                for (int j = 0; j < _figure.Figures[i].Segments.Count; j++)
                {
                    LineSegment lin = _figure.Figures[i].Segments[j] as LineSegment;
                    if (lin != null && lin.Point.X == CurrentLine.X2 && lin.Point.Y == CurrentLine.Y2)
                    {
                        if (Math.Abs(CurrentLine.Y1 - CurrentLine.Y2) <= Math.Abs(CurrentLine.X1 - CurrentLine.X2))
                            return AligmentStroke.vertical;
                        else
                            return AligmentStroke.horizontal;
                    }

                }
            }
            //
            return AligmentStroke.none;
        }

        /// <summary>
        /// формирование геометрии 
        /// </summary>
        /// <param name="kscroll">масштаб</param>
        /// <param name="X">координата точки вставки X</param>
        /// <param name="Y">координата точки вставки Y</param>
        private void GeometryFigure(double kscroll, double X, double Y)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure newfigure = new PathFigure() { StartPoint = new Point(X, Y), IsClosed = true };
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X, Y + heightdefult * kscroll) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X + widthdefult * kscroll, Y + heightdefult * kscroll) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X + widthdefult * kscroll, Y) });
            geometry.Figures.Add(newfigure);
            _figure = geometry;
            SetFillStroke();
            StrokeThickness = strokethickness * kscroll;
        }

        private void GeometryFigureCopy(PathGeometry geometry, double strokethickness)
        {
            foreach (PathFigure geo in geometry.Figures)
            {
                PathFigure newfigure = new PathFigure() { IsClosed = true };
                newfigure.StartPoint = new Point(geo.StartPoint.X, geo.StartPoint.Y);
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        newfigure.Segments.Add(new LineSegment() { Point = new Point(lin.Point.X, lin.Point.Y) });
                }
                _figure.Figures.Add(newfigure);
            }
            //
            SetFillStroke();
        }

        private void SetFillStroke()
        {
            Stroke = _colordefultstroke;
            Fill = _colordefult;
        }

        /// <summary>
        /// перемещение объекта
        /// </summary>
        /// <param name="deltaX">изменение по оси X</param>
        /// <param name="deltaY">изменение по оси Y</param>
        public void SizeFigure(double deltaX, double deltaY)
        {
            foreach (PathFigure geo in _figure.Figures)
            {
                geo.StartPoint = new Point(geo.StartPoint.X + deltaX, geo.StartPoint.Y + deltaY);
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        lin.Point = new Point(lin.Point.X + deltaX, lin.Point.Y + deltaY);
                }
            }
        }

        /// <summary>
        /// масштабироание объекта
        /// </summary>
        /// <param name="scale">масштаб</param>
        public void ScrollFigure(ScaleTransform scaletransform, double scale)
        {
            //foreach (PathFigure geo in _figure.Figures)
            //{
            //    geo.StartPoint = scaletransform.Transform(geo.StartPoint);
            //    foreach (PathSegment seg in geo.Segments)
            //    {
            //        //сегмент линия
            //        LineSegment lin = seg as LineSegment;
            //        if (lin != null)
            //            lin.Point = scaletransform.Transform(lin.Point);
            //        //сегмент арка
            //        ArcSegment arc = seg as ArcSegment;
            //        if (arc != null)
            //        {
            //            arc.Point = scaletransform.Transform(arc.Point);
            //            arc.Size = new Size(arc.Size.Width * scale, arc.Size.Height * scale);
            //        }
            //    }
            //}
            ////
            //Point point = scaletransform.Transform(new Point(_text.Margin.Left, _text.Margin.Top));
            //_text.Margin = new Thickness(point.X, point.Y, 0, 0);
            //_text.FontSize *= scale;
            ////
            //StrokeThickness *= scale;
            //_minheight *= scale;
            //_minwidth *= scale;
        }

        /// <summary>
        /// поворачиваем элемент 
        /// </summary>
        /// <param name="angle">угол поворота</param>
        public void Rotate(double angle)
        {
            if (_figure.Figures.Count > 0)
            {
                RotateText += (int)angle;
                if (Math.Abs(RotateText) > 360)
                {
                    int ostatok = (int)Math.Abs(RotateText) / 360;
                    if (RotateText >= 0)
                        RotateText -= (ostatok * 360);
                    else RotateText += (ostatok * 360);
                }
                //
                Point center = new Point(OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                RotateTransform rotate = new RotateTransform(angle, center.X, center.Y);
                //поворачиваем начальную точку
                _figure.Figures[0].StartPoint = rotate.Transform(_figure.Figures[0].StartPoint);
                foreach (PathSegment seg in _figure.Figures[0].Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                    {
                        lin.Point = rotate.Transform(lin.Point);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// установливаем цвет по умолчанию
        /// </summary>
        public void DefaultColor()
        {
            //устанавливаем по умолчанию цвет фона
            if (Fill != _colordefult)
                Fill = _colordefult;
            //устанавливаем по умолчанию цвет рамки
            if (Stroke != _colordefultstroke)
                Stroke = _colordefultstroke;
            //
            m_selectstroke = false;
            m_selectobject = false;
            Index = null;
        }
        /// <summary>
        /// установливаем цвет при выборе объекта
        /// </summary>
        public void SelectColor()
        {
            if (m_selectstroke)
                Stroke = _colorselectstroke;
            if (m_selectobject)
                Fill = _colorselect;
        }

        /// <summary>
        /// создаем коллекцию линий и точек
        /// </summary>
        public void CreateCollectionLines()
        {
            _points.Clear();
            _lines.Clear();
            foreach (PathFigure geo in _figure.Figures)
            {
                _points.Add(geo.StartPoint);
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        _points.Add(lin.Point);
                }
            }
            for (int i = 0; i < _points.Count; i++)
            {
                if (i < _points.Count - 1)
                    _lines.Add(new Line() { X1 = _points[i].X, Y1 = _points[i].Y, X2 = _points[i + 1].X, Y2 = _points[i + 1].Y });
                else if (i == _points.Count - 1)
                    _lines.Add(new Line() { X1 = _points[i].X, Y1 = _points[i].Y, X2 = _points[0].X, Y2 = _points[0].Y });
            }
        }

        public void Mirror(AligmentMirror aligmnet, Point point)
        {
            CreateCollectionLines();
            switch (aligmnet)
            {
                case AligmentMirror.horizontal:
                    {
                        foreach (PathFigure geo in _figure.Figures)
                        {
                            geo.StartPoint = new Point(geo.StartPoint.X + 2 * (point.X - geo.StartPoint.X), geo.StartPoint.Y);
                            foreach (PathSegment seg in geo.Segments)
                            {
                                //сегмент линия
                                LineSegment lin = seg as LineSegment;
                                if (lin != null)
                                {
                                    lin.Point = new Point(lin.Point.X + 2 * (point.X - lin.Point.X), lin.Point.Y);
                                }
                            }
                        }
                    }
                    break;
                case AligmentMirror.vertical:
                    {
                        foreach (PathFigure geo in _figure.Figures)
                        {
                            geo.StartPoint = new Point(geo.StartPoint.X, geo.StartPoint.Y + 2 * (point.Y - geo.StartPoint.Y));
                            foreach (PathSegment seg in geo.Segments)
                            {
                                //сегмент линия
                                LineSegment lin = seg as LineSegment;
                                if (lin != null)
                                {
                                    lin.Point = new Point(lin.Point.X, lin.Point.Y + 2 * (point.Y - lin.Point.Y));
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}

