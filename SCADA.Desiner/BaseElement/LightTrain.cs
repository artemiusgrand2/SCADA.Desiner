using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
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
    /// класс описывающий поездной светофор
    /// </summary>
    class LightTrain : Shape, IGraficObejct, IScrollElement, IFreeAngle
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

        private string nameobject = "Поездной светофор";
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
        public static double widthdefult = 18;
        /// <summary>
        /// высота по умолчанию
        /// </summary>
        public static double heightdefult = 18;
        /// <summary>
        /// толщина линии  по умолчанию
        /// </summary>
        public static double strokethickness = 1;
        /// <summary>
        /// цвет заполнения по умолчанию
        /// </summary>
        Brush _colordefult =  MainColors.Fon;
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
        /// <summary>
        /// коэффициент заполнения окружности
        /// </summary>
        private double K = 0.25;
        /// <summary>
        /// является ли светофор входным
        /// </summary>
        public bool IsInput { get; set; }

        ViewElement viewElement = ViewElement.lightTrain;
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
        //
        #endregion

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="kscroll">масштаб</param>
        /// <param name="stationnumber">номер станции</param>
        /// <param name="X">координата X точки вставки</param>
        /// <param name="Y">координата Y точки вставки</param>
        public LightTrain(double kscroll, int stationnumber, double X, double Y)
        {
            StationNumber = stationnumber;
            GeometryFigure(kscroll, X, Y);
            Stroke = _colordefultstroke;
            Fill = _colordefult;
            StrokeThickness = strokethickness * kscroll;
        }

        public LightTrain(int stationnumber, PathGeometry geometry, double strokethickness, bool Input, string name, double currencyScroll)
        {
            this.currencyScroll = currencyScroll;
            StationNumber = stationnumber;
            NameObject = name;
            IsInput = Input;
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

        public void Resize(double deltax, double deltay) { }

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
        /// определяем центр тяжести фигуры
        /// </summary>
        /// <returns></returns>
        public Point CentreFigure()
        {
            double Xmin = double.MaxValue;
            double Xmax = double.MinValue;
            double Ymin = double.MaxValue;
            double Ymax = double.MinValue;
            List<Point> points = new List<Point>();
            //
            foreach (PathFigure geo in _figure.Figures)
            {
                points.Add(geo.StartPoint);
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        points.Add(lin.Point);
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                        points.Add(arc.Point);
                }
            }
            //
            foreach (Point point in points)
            {
                if (point.X > Xmax)
                    Xmax = point.X;
                //
                if (point.X < Xmin)
                    Xmin = point.X;
                //
                if (point.Y > Ymax)
                    Ymax = point.Y;
                //
                if (point.Y < Ymin)
                    Ymin = point.Y;
            }
            //
            return new Point((Xmax + Xmin) / 2, (Ymax + Ymin) / 2);
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
            //рисуем основание светофора
            PathFigure newfigure1 = new PathFigure() { StartPoint = new Point(X, Y) , IsClosed = false};
            newfigure1.Segments.Add(new LineSegment() { Point = new Point(X , Y + heightdefult * kscroll) });
            geometry.Figures.Add(newfigure1);
            PathFigure newfigure2 = new PathFigure() { StartPoint = new Point(X, Y + heightdefult * kscroll/2), IsClosed = false };
            newfigure2.Segments.Add(new LineSegment() { Point = new Point(X + widthdefult*kscroll/4, newfigure2.StartPoint.Y) });
            geometry.Figures.Add(newfigure2);
            //рисуем головку светофора
            double r = heightdefult * kscroll * K;
            PathFigure newfigure3 = new PathFigure() { StartPoint = new Point(X + widthdefult * kscroll / 4, newfigure2.StartPoint.Y), IsClosed = false };
            newfigure3.Segments.Add(new ArcSegment() { Point = new Point(X + (widthdefult*0.75 + 2 * heightdefult * K) * kscroll, newfigure2.StartPoint.Y ), Size = new Size(r, r) });
            newfigure3.Segments.Add(new ArcSegment() { Point = new Point(newfigure3.StartPoint.X, newfigure2.StartPoint.Y), Size = new Size(r, r) });
            geometry.Figures.Add(newfigure3);
            //рисуем дополнительную головку светофора
            PathFigure newfigure4 = new PathFigure() { StartPoint = new Point(X + (widthdefult * 0.75 + 2 * heightdefult * K) * kscroll, newfigure2.StartPoint.Y), IsClosed = false };
            newfigure4.Segments.Add(new ArcSegment() { Point = new Point(X + (widthdefult*1.25 + 4 * heightdefult * K) * kscroll, newfigure2.StartPoint.Y), Size = new Size(r, r) });
            newfigure4.Segments.Add(new ArcSegment() { Point = new Point(newfigure4.StartPoint.X, newfigure2.StartPoint.Y), Size = new Size(r, r) });
            geometry.Figures.Add(newfigure4);
            //
            _figure = geometry;
        }


        private void GeometryFigureCopy(PathGeometry geometry, double strokethickness)
        {
            foreach (PathFigure geo in geometry.Figures)
            {
                PathFigure newfigure = new PathFigure();
                newfigure.StartPoint = new Point(geo.StartPoint.X, geo.StartPoint.Y);
                newfigure.IsClosed = false;
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                    {
                        newfigure.Segments.Add(new LineSegment() { Point = new Point(lin.Point.X, lin.Point.Y) });
                        continue;
                    }
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
            //
            Stroke = _colordefultstroke;
            StrokeThickness = strokethickness;
        }

        public void Normal()
        {
            GeometryFigure(currencyScroll, _figure.Figures[0].StartPoint.X, _figure.Figures[0].StartPoint.Y);
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
            foreach (PathFigure geo in _figure.Figures)
            {
                geo.StartPoint = scaletransform.Transform(geo.StartPoint);
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        lin.Point = scaletransform.Transform(lin.Point);
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
            foreach (PathFigure figure_element in Figure.Figures)
            {
                Point center = new Point(OperationsGrafic.CursorX, OperationsGrafic.CursorY);
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

        private Point RoundPoint(Point point)
        {
            return new Point(Math.Round(point.X), Math.Round(point.Y));
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
            //
            double Xmin = double.MaxValue;
            double Xmax = double.MinValue;
            double Ymin = double.MaxValue;
            double Ymax = double.MinValue;
            //
            foreach (PathFigure geo in _figure.Figures)
            {
                _points.Add(geo.StartPoint);
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        _points.Add(lin.Point);
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                        _points.Add(arc.Point);
                }
            }
            //
            foreach (Point point in _points)
            {
                if (point.X > Xmax)
                    Xmax = point.X;
                //
                if (point.X < Xmin)
                    Xmin = point.X;
                //
                if (point.Y > Ymax)
                    Ymax = point.Y;
                //
                if (point.Y < Ymin)
                    Ymin = point.Y;
            }
            //
            _points.Clear();
            _points.Add(new Point(Xmin, Ymin)); _points.Add(new Point(Xmax, Ymin));
            _points.Add(new Point(Xmax, Ymax)); _points.Add(new Point(Xmin, Ymax));
            //
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
                                //сегмент окружность
                                ArcSegment arc = seg as ArcSegment;
                                if (arc != null)
                                {
                                    arc.Point = new Point(arc.Point.X + 2 * (point.X - arc.Point.X), arc.Point.Y);
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
                                //сегмент окружность
                                ArcSegment arc = seg as ArcSegment;
                                if (arc != null)
                                {
                                    arc.Point = new Point(arc.Point.X, arc.Point.Y + 2 * (point.Y - arc.Point.Y));
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
