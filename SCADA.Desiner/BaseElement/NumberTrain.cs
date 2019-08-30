using System;
using System.Collections.Generic;
using System.Windows;
using System.Configuration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Enums;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.GeometryTransform;
using SCADA.Common.Enums;
using SCADA.Desiner.WindowWork;

namespace SCADA.Desiner
{
    /// <summary>
    /// класс описывающий номера ввода поездов
    /// </summary>
    class NumberTrain : Shape, IGraficObejct, IResize, IFreeAngle
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
        /// название пути
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
        double R = 4;
        /// <summary>
        /// минимальная ширина
        /// </summary>
        double _minwidth = 15;
        /// <summary>
        /// ширина по умолчанию
        /// </summary>
        double widthdefult = 90;
        /// <summary>
        /// высота по умолчанию
        /// </summary>
        double heightdefult = 15;
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
        ViewElement viewElement = ViewElement.numbertrain;
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
        public NumberTrain(double kscroll, int stationnumber, double X, double Y)
        {
            StationNumber = stationnumber;
            StartSize();
            GeometryFigure(kscroll, X, Y);
        }

        public NumberTrain(int stationnumber, PathGeometry geometry, double strokethickness, double angle, string name)
        {
            StationNumber = stationnumber;
            RotateText = angle;
            NameObject = name;
            GeometryFigureCopy(geometry, strokethickness);
        }


        public SizeElement SizeGeometry()
        {
            Point P1 = ((ArcSegment)Figure.Figures[0].Segments[2]).Point;
            Point P2 = ((ArcSegment)Figure.Figures[0].Segments[0]).Point;
            Point P3 = ((LineSegment)Figure.Figures[0].Segments[5]).Point;
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

        public void Newsize(double newwidth, double newheight)
        {
            try
            {
                //изменяем длину
                if (newwidth >= _minwidth)
                {
                    LineSegment lin = _figure.Figures[0].Segments[5] as LineSegment;
                    ArcSegment arc = _figure.Figures[0].Segments[0] as ArcSegment;
                    double delta = (newwidth - LenghtStorona(arc.Point, new Point(lin.Point.X, lin.Point.Y)));
                    if (Math.Abs(lin.Point.Y - arc.Point.Y) < Math.Abs(lin.Point.X - arc.Point.X))
                    {
                        if (lin.Point.Y > arc.Point.Y)
                            NewKoordinate(0, 5, -delta, 0);
                        else NewKoordinate(0, 5, delta, 0);
                    }
                    else
                    {
                        double len1 = LenghtStorona(arc.Point, new Point(lin.Point.X, lin.Point.Y + delta));
                        double len2 = LenghtStorona(arc.Point, new Point(lin.Point.X, lin.Point.Y - delta));
                        if (Math.Abs(newwidth - len1) < Math.Abs(newwidth - len2))
                            NewKoordinate(0, 5, 0, delta);
                        else NewKoordinate(0, 5, 0, -delta);
                    }

                }
            }
            catch { }
        }

        private void NewKoordinate(int figure, int segment, double deltax, double deltay)
        {
            switch (segment)
            {
                case 1:
                    {
                        LineSegment lin = _figure.Figures[figure].Segments[Index.IndexSegment] as LineSegment;
                        ArcSegment arc = _figure.Figures[figure].Segments[4] as ArcSegment;
                        if (LenghtStorona(arc.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) >= _minwidth)
                        {
                            ArcSegment arc1 = _figure.Figures[figure].Segments[Index.IndexSegment - 1] as ArcSegment;
                            if (arc1 != null)
                                arc1.Point = new Point(arc1.Point.X + deltax, arc1.Point.Y + deltay);
                            _figure.Figures[figure].StartPoint = new Point(_figure.Figures[figure].StartPoint.X + deltax, _figure.Figures[figure].StartPoint.Y + deltay);
                            lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                            //
                            LineSegment lin2 = _figure.Figures[figure].Segments[_figure.Figures[figure].Segments.Count - 1] as LineSegment;
                            if (lin2 != null)
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                            //
                            ArcSegment arc2 = _figure.Figures[figure].Segments[segment + 1] as ArcSegment;
                            if (arc2 != null)
                                arc2.Point = new Point(arc2.Point.X + deltax, arc2.Point.Y + deltay);
                        }
                    }
                    break;
                case 5:
                    {
                        LineSegment lin = _figure.Figures[figure].Segments[segment] as LineSegment;
                        ArcSegment arc = _figure.Figures[figure].Segments[0] as ArcSegment;
                        if (LenghtStorona(arc.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) >= _minwidth)
                        {
                            lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                            //меняем сегменты до текущего
                            LineSegment lineminus = _figure.Figures[figure].Segments[segment - 2] as LineSegment;
                            if (lineminus != null)
                                lineminus.Point = new Point(lineminus.Point.X + deltax, lineminus.Point.Y + deltay);
                            //
                            ArcSegment arcminus = _figure.Figures[figure].Segments[segment - 1] as ArcSegment;
                            if (arcminus != null)
                                arcminus.Point = new Point(arcminus.Point.X + deltax, arcminus.Point.Y + deltay);
                            //
                            //меняем сегменты после текущего
                            //
                            ArcSegment arcplus = _figure.Figures[figure].Segments[segment + 1] as ArcSegment;
                            if (arcplus != null)
                                arcplus.Point = new Point(arcplus.Point.X + deltax, arcplus.Point.Y + deltay);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// определяем длину отрезка между двумя точками
        /// </summary>
        /// <param name="p1">точка один</param>
        /// <param name="p2">точка два</param>
        /// <returns></returns>
        private double LenghtStorona(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public void Resize(double deltax, double deltay)
        {
            if (Index != null)
            {
                if (Index.XY)
                    deltay = 0;
                else deltax = 0;
                NewKoordinate(Index.IndexFigure, Index.IndexSegment, deltax, deltay);
            }
        }

        private void StartSize()
        {
            double buffer = 0;
            if (ConfigurationManager.AppSettings["wwight_trainnumber"] != null && double.TryParse(ConfigurationManager.AppSettings["wight_trainnumber"], out buffer))
            {
                double wight_train = double.Parse(ConfigurationManager.AppSettings["wight_trainnumber"]);
                if (wight_train > 1 && wight_train < 1000)
                    widthdefult = wight_train;
            }
            //
            if (ConfigurationManager.AppSettings["height_trainnumber"] != null && double.TryParse(ConfigurationManager.AppSettings["height_trainnumber"], out buffer))
            {
                double height_train = double.Parse(ConfigurationManager.AppSettings["height_trainnumber"]);
                if (height_train > 1 && height_train < 1000)
                    heightdefult = height_train;
            }
            //
            if (heightdefult > widthdefult)
                R = widthdefult * 0.25;
            else R = heightdefult * 0.25;

            //
            widthdefult *= OperationsGrafic.SaveScroll;
            heightdefult *= OperationsGrafic.SaveScroll;
            R *= OperationsGrafic.SaveScroll;
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
                    if (lin != null &&  lin.Point.X == CurrentLine.X2 && lin.Point.Y == CurrentLine.Y2)
                    {
                        if (j == 1 || j == 5)
                        {
                            if (Math.Abs(CurrentLine.Y1 - CurrentLine.Y2) < Math.Abs(CurrentLine.X1 - CurrentLine.X2))
                                return new Index() { IndexFigure = i, IndexSegment = j, XY = false };
                           else
                                return new Index() { IndexFigure = i, IndexSegment = j, XY = true };
                        }
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
                        if (j == 1 || j == 5)
                        {
                            if (Math.Abs(CurrentLine.Y1 - CurrentLine.Y2) <= Math.Abs(CurrentLine.X1 - CurrentLine.X2))
                                return  AligmentStroke.vertical;
                            else
                                return  AligmentStroke.horizontal;
                        }
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
            PathFigure newfigure = new PathFigure() {  StartPoint = new Point(X + R * kscroll, Y) };
            newfigure.Segments.Add(new ArcSegment() { Point = new Point(X, Y + R * kscroll), Size = new Size(R * kscroll, R * kscroll) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X, Y + (heightdefult - R) * kscroll) });
            newfigure.Segments.Add(new ArcSegment() { Point = new Point(X + R * kscroll, Y + heightdefult * kscroll), Size = new Size(R * kscroll, R * kscroll) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X + (widthdefult - R) * kscroll, Y + heightdefult * kscroll) });
            newfigure.Segments.Add(new ArcSegment() { Point = new Point(X + widthdefult * kscroll, Y + (heightdefult - R) * kscroll), Size = new Size(R * kscroll, R * kscroll) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X + widthdefult * kscroll, Y + R * kscroll) });
            newfigure.Segments.Add(new ArcSegment() { Point = new Point(X + (widthdefult - R) * kscroll, Y), Size = new Size(R * kscroll, R * kscroll) });
            newfigure.Segments.Add(new LineSegment() { Point = newfigure.StartPoint});
            geometry.Figures.Add(newfigure);
            _figure = geometry;
            Stroke = _colordefultstroke;
            Fill = _colordefult;
            StrokeThickness = strokethickness * kscroll;
           // _minwidth *= kscroll;
        }

        private void GeometryFigureCopy(PathGeometry geometry, double strokethickness)
        {
            foreach (PathFigure geo in geometry.Figures)
            {
                PathFigure newfigure = new PathFigure();
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
           // _minwidth *= OperationsGrafic.CurrentScroll;
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
                        arc.Size = new Size(arc.Size.Width * scale, arc.Size.Height * scale);
                    }
                }
            }
            //
            StrokeThickness *= scale;
          //  _minwidth *= scale;
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
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                        _points.Add(arc.Point);
                }
            }
            double summaX = 0;
            double summaY = 0;
            //
            for (int i = 0; i < _points.Count; i++)
            {
                summaX += +_points[i].X;
                summaY += +_points[i].Y;
                if(i< _points.Count-1)
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
