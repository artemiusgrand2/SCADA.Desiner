using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Configuration;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Enums;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.GeometryTransform;
using SCADA.Desiner.Constanst;
using SCADA.Common.Enums;
using SCADA.Desiner.WindowWork;

namespace SCADA.Desiner.BaseElement
{
    /// <summary>
    /// класс описывающий стрелку перегона
    /// </summary>
    class ArrowMove : Shape, IGraficObejct, IScrollElement, IFreeAngle
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
        /// <summary>
        /// название пути
        /// </summary>
        public string NameObject { get; set; }
        /// <summary>
        /// пояснения
        /// </summary>
        public string Notes { get; set; }

        private string nameobject = "Перегонная стрелка";
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
        /// ширина по умолчанию
        /// </summary>
        double widthdefult = 48;
        /// <summary>
        /// высота по умолчанию
        /// </summary>
        double heightdefult = 22;
        /// <summary>
        /// толщина линии  по умолчанию
        /// </summary>
        public  static double strokethickness = 1;
        /// <summary>
        /// цвет заполнения по умолчанию
        /// </summary>
        Brush _colordefult = MainColors.Fon;
        /// <summary>
        /// цвет выделенного объекта
        /// </summary>
        Brush _colorselect = new SolidColorBrush(Color.FromArgb(190, 170, 0, 170));
        /// <summary>
        /// цвет контура по умолчанию
        /// </summary>
        Brush _colordefultstroke = Brushes.Black;
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
        Index _index ;
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

        ViewElement viewElement = ViewElement.arrowmove;
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
        //
        #endregion

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="kscroll">масштаб</param>
        /// <param name="stationnumber">номер станции</param>
        /// <param name="X">координата X точки вставки</param>
        /// <param name="Y">координата Y точки вставки</param>
        public ArrowMove (double kscroll, int stationnumber, double X, double Y)
        {
            StationNumber = stationnumber;
            StartSize();
            GeometryFigure(kscroll, X, Y); 
        }

        public ArrowMove (int stationnumber, PathGeometry geometry, double strokethickness, string name, double currencyScroll)
        {
            this.currencyScroll = currencyScroll;
            StationNumber = stationnumber;
            NameObject = name;
            GeometryFigureCopy(geometry, strokethickness);
        }

        public void Resize(double deltax, double deltay) { }

        public SizeElement SizeGeometry()
        {
            Point P1 = ((LineSegment)Figure.Figures[0].Segments[0]).Point;
            Point P2 = ((LineSegment)Figure.Figures[0].Segments[1]).Point;
            Point P3 = ((LineSegment)Figure.Figures[2].Segments[1]).Point;
            return new SizeElement() { Height = Math.Round(Math.Abs(Figure.Figures[0].StartPoint.Y - P1.Y) / OperationsGrafic.SaveScroll), Widht = Math.Round(Math.Abs(P2.X - P1.X) / OperationsGrafic.SaveScroll) };
        }

        public Point PointInsert()
        {
            return new Point(_figure.Figures[0].StartPoint.X / System.Windows.SystemParameters.CaretWidth, _figure.Figures[0].StartPoint.Y / System.Windows.SystemParameters.CaretWidth);
        }

        private void StartSize()
        {
            double buffer = 0;
            if (ConfigurationManager.AppSettings["wight_arrow"] != null && double.TryParse(ConfigurationManager.AppSettings["wight_arrow"], out buffer))
            {
                double wight_arrow = double.Parse(ConfigurationManager.AppSettings["wight_arrow"]);
                if (wight_arrow > 1 && wight_arrow < 1000)
                    widthdefult = wight_arrow;
            }
            //
            if (ConfigurationManager.AppSettings["height_arrow"] != null && double.TryParse(ConfigurationManager.AppSettings["height_arrow"], out buffer))
            {
                double height_arrow = double.Parse(ConfigurationManager.AppSettings["height_arrow"]);
                if (height_arrow > 1 && height_arrow < 1000)
                    heightdefult = height_arrow;
            }
            //
            widthdefult *= OperationsGrafic.SaveScroll;
            heightdefult *= OperationsGrafic.SaveScroll;
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
            PathGeometry geometry = new PathGeometry();
            //создание левой стрелки
            PathFigure newfigure1arrow = new PathFigure() { StartPoint = new Point(X + kscroll* widthdefult/3, Y) , IsClosed = true};
            newfigure1arrow.Segments.Add(new LineSegment() { Point = new Point(X + kscroll * widthdefult / 3, Y + heightdefult * kscroll) });
            newfigure1arrow.Segments.Add(new LineSegment() { Point = new Point(X , Y + heightdefult * kscroll/2) });
          //  newfigure1arrow.Segments.Add(new LineSegment() { Point = newfigure1arrow.StartPoint });
            geometry.Figures.Add(newfigure1arrow);
            //создание средней части
            PathFigure newfigure2rec = new PathFigure() { StartPoint = new Point(X + kscroll * widthdefult / 3, Y + heightdefult * kscroll*0.2), IsClosed = true };
            newfigure2rec.Segments.Add(new LineSegment() { Point = new Point(X + kscroll *2* widthdefult / 3, Y + heightdefult * kscroll * 0.2) });
            newfigure2rec.Segments.Add(new LineSegment() { Point = new Point(X + kscroll * 2 * widthdefult / 3, Y + heightdefult * kscroll * 0.8) });
            newfigure2rec.Segments.Add(new LineSegment() { Point = new Point(X + kscroll  * widthdefult / 3, Y + heightdefult * kscroll * 0.8) });
          //  newfigure2rec.Segments.Add(new LineSegment() { Point = newfigure2rec.StartPoint });
            geometry.Figures.Add(newfigure2rec);
            //создание правой стрелки
            PathFigure newfigure2arrow = new PathFigure() { StartPoint = new Point(X + kscroll * 2*widthdefult / 3, Y), IsClosed = true };
            newfigure2arrow.Segments.Add(new LineSegment() { Point = new Point(X + kscroll *2 * widthdefult / 3, Y + heightdefult * kscroll) });
            newfigure2arrow.Segments.Add(new LineSegment() { Point = new Point(X + kscroll *  widthdefult, Y + heightdefult * kscroll / 2) });
         //   newfigure2arrow.Segments.Add(new LineSegment() { Point = newfigure2arrow.StartPoint });
            geometry.Figures.Add(newfigure2arrow);
            _figure = geometry;
            Stroke = _colordefultstroke;
            Fill = _colordefult;
            StrokeThickness = strokethickness * kscroll;
        }

        public void Normal()
        {
        }

        private void GeometryFigureCopy(PathGeometry geometry, double strokethickness)
        {
            foreach (PathFigure geo in geometry.Figures)
            {
                PathFigure newfigure = new PathFigure() {IsClosed = true };
                newfigure.StartPoint = new Point(geo.StartPoint.X, geo.StartPoint.Y);
                newfigure.IsClosed = true;
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                    {
                        newfigure.Segments.Add(new LineSegment() { Point = new Point(lin.Point.X, lin.Point.Y) });
                        continue;
                    }
                }
                _figure.Figures.Add(newfigure);
            }
            Stroke = _colordefultstroke;
            StrokeThickness = strokethickness;
        }
        /// <summary>
        /// перемещение объекта
        /// </summary>
        /// <param name="deltaX">изменение по оси X</param>
        /// <param name="deltaY">изменение по оси Y</param>
        public void SizeFigure(double  deltaX, double deltaY)
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
            foreach (PathFigure geo in _figure.Figures)
            {
                geo.StartPoint = scaletransform.Transform(geo.StartPoint);
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        lin.Point = scaletransform.Transform(lin.Point);
                }
            }
            //
            StrokeThickness = scale;
            currencyScroll = scale;
        }

        /// <summary>
        /// определяем центр тяжести фигуры
        /// </summary>
        /// <returns></returns>
        public Point CentreFigure()
        {
            double centerX =_figure.Figures[1].StartPoint.X;
            double centerY =_figure.Figures[1].StartPoint.Y;
            //
            foreach (PathSegment seg in _figure.Figures[1].Segments)
            {
                //сегмент линия
                LineSegment lin = seg as LineSegment;
                if (lin != null)
                {
                    centerX += lin.Point.X;
                    centerY += lin.Point.Y;
                }
            }
            //
            return new Point(centerX / (1 + _figure.Figures[1].Segments.Count), centerY / (1 + _figure.Figures[1].Segments.Count));
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
        }

        private Point RoundPoint(Point point)
        {
            return new Point(Math.Round(point.X), Math.Round(point.Y));
        }

        public void Mirror(AligmentMirror aligmnet, Point point)
        {
            switch (aligmnet)
            {
                case AligmentMirror.horizontal:
                    {
                        foreach (PathFigure geo in _figure.Figures)
                        {
                            geo.StartPoint = new Point(geo.StartPoint.X + 2*(point.X - geo.StartPoint.X), geo.StartPoint.Y);
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
        /// <summary>
        /// установливаем цвет по умолчанию
        /// </summary>
        public void DefaultColor()
        {
            //устанавливаем цвет объекта по умолчанию
            if (Fill != _colordefult)
                Fill = _colordefult;
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
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        _points.Add(lin.Point);
                }
            }
            //
            for (int i = 0; i < _points.Count; i++)
            {
                if(i< _points.Count-1)
                    _lines.Add(new Line() { X1 = _points[i].X, Y1 = _points[i].Y, X2 = _points[i + 1].X, Y2 = _points[i + 1].Y });
                else if (i == _points.Count - 1)
                    _lines.Add(new Line() { X1 = _points[i].X, Y1 = _points[i].Y, X2 = _points[0].X, Y2 = _points[0].Y });
            }
        }
    }
}
