using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Enums;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.GeometryTransform;
using SCADA.Common.Enums;
using SCADA.Desiner.WindowWork;

namespace SCADA.Desiner.BaseElement
{
    /// <summary>
    /// класс описывающий рамку станции
    /// </summary>
    class RamkaStation : Shape, IGraficObejct, IFreeAngle
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
        /// название элемента
        /// </summary>
        public string NameObject { get; set; }
        /// <summary>
        /// пояснения
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Ид объекта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// радиус закругления концов отрезков
        /// </summary>
        public static double R = 6;
        /// <summary>
        /// толщина линии  по умолчанию
        /// </summary>
        public static double strokethickness = 1;
        /// <summary>
        /// цвет заполнения по умолчанию
        /// </summary>
        Brush _colordefult = new SolidColorBrush(Color.FromRgb(235, 235, 235));
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

        ViewElement viewElement = ViewElement.ramka;
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
        public RamkaStation(double kscroll, int stationnumber, PointCollection points)
        {
            StationNumber = stationnumber;
            StartSize();
            GeometryFigure(kscroll, points);
           // MainWindow.DeltaXY += SizeNew;
        }

        public RamkaStation(int stationnumber, PathGeometry geometry, double strokethickness, string name)
        {
            StationNumber = stationnumber;
            GeometryFigureCopy(geometry, strokethickness);
           // MainWindow.DeltaXY += SizeNew;
        }

        private void StartSize()
        {
            double buffer = 0;
            if (ConfigurationManager.AppSettings["radius_ramka"] != null && double.TryParse(ConfigurationManager.AppSettings["radius_ramka"], out buffer))
            {
                double radius = double.Parse(ConfigurationManager.AppSettings["radius_ramka"]);
                if (radius > 1 && radius < 20)
                    R = radius;
                R *= OperationsGrafic.SaveScroll;
            }
        }

        public SizeElement SizeGeometry()
        {
            return new SizeElement();
        }

        public Point PointInsert()
        {
            return new Point(_figure.Figures[0].StartPoint.X / System.Windows.SystemParameters.CaretWidth, _figure.Figures[0].StartPoint.Y / System.Windows.SystemParameters.CaretWidth);
        }

        public void Resize(double deltax, double deltay)
        {
            if (Index != null )
            {
                if (Index.XY)
                    deltay = 0;
                else deltax = 0;
                //
                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                //одна точка впереди
                if (Index.IndexSegment + 1 == _figure.Figures[Index.IndexFigure].Segments.Count - 1)
                {
                    lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as LineSegment;
                    lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                    _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                }
                else if (Index.IndexSegment + 1 < _figure.Figures[Index.IndexFigure].Segments.Count - 1)
                {
                    lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as LineSegment;
                    lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                }
                //первая точка позади
                if (Index.IndexSegment - 1 >= 0)
                {
                    lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as LineSegment;
                    lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                }
                else
                {
                    lin = _figure.Figures[Index.IndexFigure].Segments[_figure.Figures[Index.IndexFigure].Segments.Count - 1] as LineSegment;
                    lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                    _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                }
                //вторая точка позади
                switch (Index.IndexSegment - 2)
                {
                    case -2:
                        {
                            lin = _figure.Figures[Index.IndexFigure].Segments[_figure.Figures[Index.IndexFigure].Segments.Count - 2] as LineSegment;
                            lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                        }
                        break;
                    case -1:
                        {
                            lin = _figure.Figures[Index.IndexFigure].Segments[_figure.Figures[Index.IndexFigure].Segments.Count - 1] as LineSegment;
                            lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                            _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                        }
                        break;
                    default:
                        {
                            lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 2] as LineSegment;
                            lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                        }
                        break;
                }
            }
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
                    if (lin.Point.X == CurrentLine.X2 && lin.Point.Y == CurrentLine.Y2)
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
                    if (lin.Point.X == CurrentLine.X2 && lin.Point.Y == CurrentLine.Y2)
                    {
                        if (Math.Abs(CurrentLine.Y1 - CurrentLine.Y2) <= Math.Abs(CurrentLine.X1 - CurrentLine.X2))
                            return  AligmentStroke.vertical;
                        else
                            return  AligmentStroke.horizontal;
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
        private void GeometryFigure(double kscroll, PointCollection points)
        {
            PathGeometry geometry = new PathGeometry();
            //обновленная коллекция точек
            List<Point> _updatepoint = new List<Point>();
            //удаляем лишнюю точку
            if (points[1].Y == points[points.Count - 1].Y)
                points.RemoveAt(0);
            //получаем новую коллекцию точек
            for (int i = 0; i < points.Count; i++)
            {
                //если последняя точка
                if (i == points.Count - 1)
                {
                    if (points[i].X == points[0].X)
                    {
                        if (points[i].Y < points[0].Y)
                        {
                            _updatepoint.Add(new Point(points[i].X, (points[i].Y + kscroll * R)));
                            _updatepoint.Add(new Point(points[i].X, (points[0].Y - kscroll * R)));
                        }
                        else
                        {
                            _updatepoint.Add(new Point(points[i].X, (points[i].Y - kscroll * R)));
                            _updatepoint.Add(new Point(points[i].X, (points[0].Y + kscroll * R)));
                        }
                    }
                    else
                    {
                        if (points[i].X < points[0].X)
                        {
                            _updatepoint.Add(new Point(points[i].X + kscroll * R, points[i].Y));
                            _updatepoint.Add(new Point(points[0].X - kscroll * R, points[i].Y));
                        }
                        else
                        {
                            _updatepoint.Add(new Point(points[i].X - kscroll * R, points[i].Y));
                            _updatepoint.Add(new Point(points[0].X + kscroll * R, points[i].Y));
                        }
                    }
                }
                //если внутренние точкм
                if (i != points.Count - 1)
                {
                    if (points[i + 1].X == points[i].X)
                    {
                        if (points[i].Y < points[i + 1].Y)
                        {
                            _updatepoint.Add(new Point(points[i].X,(points[i].Y + kscroll*R)));
                            _updatepoint.Add(new Point(points[i].X, (points[i+1].Y - kscroll * R)));
                        }
                        else
                        {
                            _updatepoint.Add(new Point(points[i].X, (points[i].Y - kscroll * R)));
                            _updatepoint.Add(new Point(points[i].X, (points[i + 1].Y + kscroll * R)));
                        }
                    }
                    else
                    {
                        if (points[i].X < points[i + 1].X)
                        {
                            _updatepoint.Add(new Point(points[i].X + kscroll*R, points[i].Y));
                            _updatepoint.Add(new Point(points[i + 1].X - kscroll * R, points[i].Y));
                        }
                        else
                        {
                            _updatepoint.Add(new Point(points[i].X - kscroll * R, points[i].Y));
                            _updatepoint.Add(new Point(points[i+1].X + kscroll * R, points[i].Y));
                        }
                    }
                }
            }
            PathFigure newfigure = new PathFigure() { StartPoint = new Point(_updatepoint[0].X, _updatepoint[0].Y), IsClosed = true };
            for (int i = 1; i < _updatepoint.Count; i++)
            {
                newfigure.Segments.Add(new LineSegment() { Point = new Point(_updatepoint[i].X, _updatepoint[i].Y) });
            }
            newfigure.Segments.Add(new LineSegment() { Point = newfigure.StartPoint });
            geometry.Figures.Add(newfigure);
            _figure = geometry;
            Stroke = _colordefultstroke;
            Fill = _colordefult;
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
            StrokeThickness *= scale;
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
                }
            }
        }

        /// <summary>
        /// установливаем цвет по умолчанию
        /// </summary>
        public void DefaultColor()
        {
            if (Fill != _colordefult)
                Fill = _colordefult;
            //устанавливаем по умолчанию цвет рамки
            if (Stroke != _colordefultstroke)
                Stroke = _colordefultstroke;
            //
            _selectstroke = false;
            _selectobject = false;
            Index = null;
        }
        /// <summary>
        /// установливаем цвет при выборе объекта
        /// </summary>
        public void SelectColor()
        {
            if (_selectstroke)
                Stroke = _colorselectstroke;
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
