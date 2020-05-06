using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Enums;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.Constanst;
using SCADA.Desiner.GeometryTransform;
using SCADA.Common.Enums;

namespace SCADA.Desiner.BaseElement
{
    /// <summary>
    /// класс разъеденитель
    /// </summary>
    class Disconnectors : Shape, IGraficObejct, IScrollElement, IFreeAngle
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
        /// тип разъеденителя
        /// </summary>
        public TypeDisconnectors Type { get; set; }
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
        /// <summary>
        /// название пути
        /// </summary>
        public string NameObject { get; set; }
        /// <summary>
        /// пояснения
        /// </summary>
        public string Notes { get; set; }

        private string nameobject = "Разъеденитель";
        /// <summary>
        /// название объекта
        /// </summary>
        public string NameEl
        {
            get
            {
                return nameobject;
            }
        }

        /// <summary>
        /// Ид объекта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ширина и высота по умолчанию
        /// </summary>
        public static double heightdefult = 18;
        /// <summary>
        /// толщина линии  по умолчанию
        /// </summary>
        public static double strokethickness = 1.3;
        /// <summary>
        /// цвет заполнения по умолчанию первый
        /// </summary>
        Brush _colordefult = new SolidColorBrush(Color.FromArgb(0, Brushes.Red.Color.R, Brushes.Red.Color.G, Brushes.Red.Color.B));
        /// <summary>
        /// цвет заполнения по умолчанию первый
        /// </summary>
        Brush _colordefult1 = Brushes.Red;
        /// <summary>
        /// цвет заполнения по умолчанию второй
        /// </summary>
        Brush _colordefult2 = Brushes.Green;
        /// <summary>
        /// цвет выделенного объекта
        /// </summary>
        Brush _colorselect = new SolidColorBrush(Color.FromArgb(190, 170, 0, 170));
        /// <summary>
        /// цвет контура по умолчанию
        /// </summary>
        Brush _colordefultstroke = Brushes.Black;
        /// <summary>
        /// коэффициент заполнения окружности
        /// </summary>
        private double K = 0;
        /// <summary>
        /// переменная показывает выбрана ли рамка для редактирования
        /// </summary>
        private bool _selectstroke = false;
        public bool SelectStroke
        {
            get
            {
                return _selectstroke;
            }
            set
            {
                _selectstroke = value;
            }
        }
        /// <summary>
        /// переменная показывает выбрана ли сам объект
        /// </summary>
        private bool _selectobject = false;
        public bool Selectobject
        {
            get
            {
                return _selectobject;
            }
            set
            {
                _selectobject = value;
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
        //
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
        /// <summary>
        /// центр фигуры
        /// </summary>
        Point _center_figure = new Point();

        ViewElement viewElement = ViewElement.disconnectors;
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

        private double currencyScroll = 1;
        /// <summary>
        /// текущий масштаб объекта
        /// </summary>
        public double CurrencyScroll
        {
            get
            {
                return currencyScroll;
            }
            set
            {
                currencyScroll = value;
            }
        }

        /// <summary>
        /// индеск слоя наложения объектов
        /// </summary>
        public int ZIndex { get; set; }

        public string FileClick { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="kscroll">масштаб</param>
        /// <param name="stationnumber">номер станции</param>
        /// <param name="X">координата X точки вставки</param>
        /// <param name="Y">координата Y точки вставки</param>
        public Disconnectors(double kscroll, int stationnumber, double X, double Y)
        {
            StationNumber = stationnumber;
            GeometryFigure(kscroll, X, Y);
        }

        public Disconnectors(int stationnumber, int stationRight, PathGeometry geometry, double strokethickness, string name, TypeDisconnectors type, double currencyScroll)
        {
            this.currencyScroll = currencyScroll;
            StationNumber = stationnumber;
            StationNumberRight = stationRight;
            Type = type;
            NameObject = name;
            GeometryFigureCopy(geometry, strokethickness);
        }

        public SizeElement SizeGeometry()
        {
            return new SizeElement();
        }

        public Point PointInsert()
        {
            return new Point(_figure.Figures[0].StartPoint.X / System.Windows.SystemParameters.CaretWidth, _figure.Figures[0].StartPoint.Y / System.Windows.SystemParameters.CaretWidth);
        }

        public void Normal()
        {
        }

        public void Resize(double deltax, double deltay) { }
        /// <summary>
        /// определяем центр тяжести фигуры
        /// </summary>
        /// <returns></returns>
        public Point CentreFigure()
        {
            double centerX = _figure.Figures[0].StartPoint.X;
            double centerY = _figure.Figures[0].StartPoint.Y;
            //
            foreach (PathSegment seg in _figure.Figures[0].Segments)
            {
                //сегмент арка
                ArcSegment arc = seg as ArcSegment;
                if (arc != null)
                {
                    centerX += arc.Point.X;
                    centerY += arc.Point.Y;
                }
            }
            //
            return new Point(centerX / (1 + _figure.Figures[0].Segments.Count), centerY / (1 + _figure.Figures[0].Segments.Count));
        }
        /// <summary>
        /// находим нужный сегмент для редактирования
        /// </summary>
        /// <param name="CurrentLine">текущая выбранная линия</param>
        /// <returns></returns>
        public Index FindIndexLine(Line CurrentLine)
        {
            return null;
        }

        public AligmentStroke FindLineAligment(Line CurrentLine)
        {
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
            //
            PathGeometry geometry = new PathGeometry();
            //фигура окружность
            double r = heightdefult * kscroll * (1 - 2 * K) / 2;
            PathFigure circle = new PathFigure() { StartPoint = new Point(X + (heightdefult / 2) * kscroll, Y + heightdefult * K * kscroll) };
            circle.Segments.Add(new ArcSegment() { Point = new Point(X, Y + (heightdefult / 2) * kscroll), Size = new Size(r, r) });
            circle.Segments.Add(new ArcSegment() { Point = new Point(circle.StartPoint.X, Y + heightdefult * (1 - K) * kscroll), Size = new Size(r, r) });
            circle.Segments.Add(new ArcSegment() { Point = new Point(X + heightdefult * (1 - K) * kscroll, Y + (heightdefult / 2) * kscroll), Size = new Size(r, r) });
            circle.Segments.Add(new ArcSegment() { Point = new Point(circle.StartPoint.X, circle.StartPoint.Y), Size = new Size(r, r) });
            geometry.Figures.Add(circle);
            //
            _figure = geometry;
            Stroke = _colordefultstroke;
            DefaultColor();
            StrokeThickness = strokethickness * kscroll;
        }

        private void GeometryFigureCopy(PathGeometry geometry, double strokethickness)
        {
            foreach (PathFigure geo in geometry.Figures)
            {
                PathFigure newfigure = new PathFigure();
                newfigure.StartPoint = new Point(geo.StartPoint.X, geo.StartPoint.Y);
                newfigure.IsClosed = true;
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                    {
                        newfigure.Segments.Add(new ArcSegment() { Point = new Point(arc.Point.X, arc.Point.Y), Size = new Size(arc.Size.Width, arc.Size.Height) });
                        continue;
                    }
                }
                _figure.Figures.Add(newfigure);
            }
            Stroke = _colordefultstroke;
            StrokeThickness = strokethickness;
            DefaultColor();
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
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                        arc.Point = new Point(arc.Point.X + deltaX, arc.Point.Y + deltaY);
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
            //StrokeThickness *= scale;
            //currencyScroll *= scale;


            foreach (PathFigure geo in _figure.Figures)
            {
                geo.StartPoint = scaletransform.Transform(geo.StartPoint);
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                    {
                        arc.Point = scaletransform.Transform(arc.Point);
                        arc.Size = new Size(arc.Size.Width * scaletransform.ScaleX, arc.Size.Height * scaletransform.ScaleX);
                    }
                }
            }
            //
            StrokeThickness = scale;
            currencyScroll = scale;
        }

        /// <summary>
        /// поворачиваем элемент 
        /// </summary>
        /// <param name="angle">угол поворота</param>
        public void Rotate(double angle)
        {
            if (Figure.Figures.Count > 0)
            {
                Point center = new Point(OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                foreach (PathFigure figure_element in Figure.Figures)
                {
                    RotateTransform rotate = new RotateTransform(angle, center.X, center.Y);
                    //поворачиваем начальную точку
                    figure_element.StartPoint = rotate.Transform(figure_element.StartPoint);
                    foreach (PathSegment seg in figure_element.Segments)
                    {
                        //сегмент арка
                        ArcSegment arc = seg as ArcSegment;
                        if (arc != null)
                        {
                            arc.Point = rotate.Transform(arc.Point);
                            continue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// установливаем цвет по умолчанию
        /// </summary>
        public void DefaultColor()
        {
            //устанавливаем цвет объекта по умолчанию
            switch (Type)
            {
                case TypeDisconnectors.normalOff:
                    Fill = _colordefult2;
                    break;
                case TypeDisconnectors.normalOn:
                    Fill = _colordefult1;
                    break;
                case TypeDisconnectors.notNormal:
                    Fill = _colordefult;
                    break;
            }
            //
            _selectobject = false;
        }
        /// <summary>
        /// установливаем цвет при выборе объекта
        /// </summary>
        public void SelectColor()
        {
            if (_selectobject)
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
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                        _points.Add(arc.Point);
                }
            }
            //
            double summaX = 0;
            double summaY = 0;
            //
            for (int i = 0; i < _points.Count; i++)
            {
                summaX += _points[i].X;
                summaY += _points[i].Y;
                if (i < _points.Count - 1)
                    _lines.Add(new Line() { X1 = _points[i].X, Y1 = _points[i].Y, X2 = _points[i + 1].X, Y2 = _points[i + 1].Y });
                else if (i == _points.Count - 1)
                    _lines.Add(new Line() { X1 = _points[i].X, Y1 = _points[i].Y, X2 = _points[0].X, Y2 = _points[0].Y });
            }
            //
            if (_points.Count > 0)
            {
                _center_figure.X = summaX / _points.Count;
                _center_figure.Y = summaY / _points.Count;
            }
        }

        public void Mirror(AligmentMirror aligmnet, Point point)
        {
            CreateCollectionLines();
            switch (aligmnet)
            {
                case AligmentMirror.horizontal:
                    {
                        SizeFigure(2 * (point.X - _center_figure.X), 0);
                    }
                    break;
                case AligmentMirror.vertical:
                    {
                        SizeFigure(0, 2 * (point.Y - _center_figure.Y));
                    }
                    break;
            }
        }
    }
}
