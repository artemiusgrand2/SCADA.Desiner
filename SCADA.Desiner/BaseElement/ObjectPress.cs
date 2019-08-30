using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.GeometryTransform;
using SCADA.Desiner.Enums;
using SCADA.Common.Enums;

namespace SCADA.Desiner.BaseElement
{
    class ObjectPress: Shape, IGraficObejct, IFreeAngle
    {

        #region Геометрические параметры

        double width_area = 100;

        double height_area = 200;

        #endregion


        #region Цветовые параметры

        Brush stroke_color = Brushes.Black;

        Brush fill_color = Brushes.Transparent;

        #endregion


        #region Переменные и свойства
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
        public string Names { get; set; }
        /// <summary>
        /// Ид объекта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// толщина линии  по умолчанию
        /// </summary>
        public static double strokethickness = 1;
        /// <summary>
        /// цвет заполнения по умолчанию
        /// </summary>
        Brush _colordefult = new SolidColorBrush(Color.FromRgb(195, 195, 195));
        /// <summary>
        /// цвет выделенного объекта
        /// </summary>
        Brush _colorselect = new SolidColorBrush(Color.FromArgb(190, 170, 0, 170));
        /// <summary>
        /// цвет выделенного контура
        /// </summary>
        Brush _colorselectstroke = Brushes.Red;
        /// <summary>
        /// цвет контура по умолчанию если вид  - таблица поездов
        /// </summary>
        Brush _color_table_train = Brushes.Purple;
        /// <summary>
        /// цвет контура по умолчанию если вид  - таблица поездов
        /// </summary>
        Brush _color_table_autopilot = Brushes.Brown;
        /// <summary>
        /// цвет контура по умолчанию если вид  - область вставки картинки
        /// </summary>
        Brush _color_area_picture = Brushes.Blue;
        /// <summary>
        ///  цвет контура по умолчанию если вид  - станция
        /// </summary>
        Brush _color_area_station = Brushes.Green;
        /// <summary>
        /// цвет контура по умолчанию если вид  - справка
        /// </summary>
        Brush _color_area_message = Brushes.Orange;
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

        //ViewElement view;
        ///// <summary>
        ///// вид элемента
        ///// </summary>
        //public ViewElement View { get { return view; } }
        //
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
       // private ViewElement viewElement = ViewElement.
        /// <summary>
        /// Путь к отображаемой картинке
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Угол поворота
        /// </summary>
        public double Angle { get; set; }
        ViewElement viewElement = ViewElement.area;
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

        public ObjectPress(double X, double Y, double kscroll)
        {
            GeometryFigure(X, Y, kscroll);
        }

        public ObjectPress(PathGeometry geometry, double strokethickness, string path, double angle)
        {
            Angle = angle;
            Path = path;
            GeometryFigureCopy(geometry, strokethickness);
        }

        public SizeElement SizeGeometry()
        {
            Point P1 = ((LineSegment)Figure.Figures[0].Segments[0]).Point;
            Point P2 = ((LineSegment)Figure.Figures[0].Segments[1]).Point;
            return new SizeElement() { Height = Math.Round(Math.Abs(Figure.Figures[0].StartPoint.Y - P2.Y)), Widht = Math.Round(Math.Abs(Figure.Figures[0].StartPoint.X - P1.X)) };
        }

        public Point PointInsert()
        {
            return new Point(_figure.Figures[0].StartPoint.X, _figure.Figures[0].StartPoint.Y);
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
                    switch (Index.IndexSegment)
                    {
                        case 0:
                            {
                                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                LineSegment linbefore = _figure.Figures[Index.IndexFigure].Segments[_figure.Figures[Index.IndexFigure].Segments.Count-1] as LineSegment;
                                linbefore.Point = new Point(linbefore.Point.X + deltax, linbefore.Point.Y + deltay);
                                _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                            }
                            break;
                        case 1:
                            {
                                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                LineSegment linbefore = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment -1] as LineSegment;
                                linbefore.Point = new Point(linbefore.Point.X + deltax, linbefore.Point.Y + deltay);
                            }
                            break;
                        case 2:
                            {
                                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                LineSegment linbefore = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as LineSegment;
                                linbefore.Point = new Point(linbefore.Point.X + deltax, linbefore.Point.Y + deltay);
                            }
                            break;
                        case 3:
                            {
                                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                LineSegment linbefore = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as LineSegment;
                                linbefore.Point = new Point(linbefore.Point.X + deltax, linbefore.Point.Y + deltay);
                                _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                            }
                            break;
                    }
                }
            }
            catch { }
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
        private void GeometryFigure(double X, double Y, double kscroll)
        {
            PathGeometry geometry = new PathGeometry();
            //рамка объекта
            PathFigure area = new PathFigure() { StartPoint = new Point(X, Y), IsClosed = true };
            area.Segments.Add(new LineSegment() { Point = new Point(X + width_area, Y) });
            area.Segments.Add(new LineSegment() { Point = new Point(X + width_area, Y + height_area) });
            area.Segments.Add(new LineSegment() { Point = new Point(X, Y + height_area) });
            area.Segments.Add(new LineSegment() { Point = area.StartPoint});
            geometry.Figures.Add(area);
            //верхний тормозной башмак
            PathFigure press_top = new PathFigure() { StartPoint = new Point(X + width_area/2, Y), IsClosed = false };
            press_top.Segments.Add(new LineSegment() { Point = new Point(area.StartPoint.X, Y + height_area/10) });
            press_top.Segments.Add(new LineSegment() { Point = new Point(area.StartPoint.X + width_area * 0.4, area.StartPoint.Y) });
            press_top.Segments.Add(new LineSegment() { Point = new Point(area.StartPoint.X + width_area * 0.4, Y + height_area / 4) });
            press_top.Segments.Add(new LineSegment() { Point = new Point(X + width_area*0.1, Y + height_area / 4) });
            press_top.Segments.Add(new LineSegment() { Point = new Point(X + width_area * 0.1, area.StartPoint.Y) });
            press_top.Segments.Add(new LineSegment() { Point = new Point(area.StartPoint.X, Y + height_area/10) });
            geometry.Figures.Add(area);
            //нижний тормозной башмак
            PathFigure press_bottom = new PathFigure() { IsClosed = false};
            press_bottom.Segments.Add(new LineSegment() { Point = new Point(area.StartPoint.X, Y + height_area / 10) });
            press_bottom.Segments.Add(new LineSegment() { Point = new Point(area.StartPoint.X + width_area * 0.4, area.StartPoint.Y) });
            press_bottom.Segments.Add(new LineSegment() { Point = new Point(area.StartPoint.X + width_area * 0.4, Y + height_area / 4) });
            press_bottom.Segments.Add(new LineSegment() { Point = new Point(X + width_area * 0.1, Y + height_area / 4) });
            press_bottom.Segments.Add(new LineSegment() { Point = new Point(X + width_area * 0.1, area.StartPoint.Y) });
            press_bottom.Segments.Add(new LineSegment() { Point = new Point(area.StartPoint.X, Y + height_area / 10) });

            Point mirror_line = new Point(X+width_area/2, Y+ height_area/2);
            press_bottom.StartPoint = new Point(press_top.StartPoint.X + 2 * (mirror_line.X - press_top.StartPoint.X), press_top.StartPoint.Y);
            foreach (PathSegment seg in press_top.Segments)
            {
                //сегмент линия
                LineSegment lin = seg as LineSegment;
                if (lin != null)
                    press_bottom.Segments.Add(new LineSegment() { Point = new Point(lin.Point.X + 2 * (mirror_line.X - lin.Point.X), lin.Point.Y) });
            }
            //
            _figure = geometry;
            Stroke = stroke_color;
            StrokeThickness = strokethickness * kscroll;
        }

        private void GeometryFigureCopy(PathGeometry geometry, double strokethickness)
        {
            foreach (PathFigure geo in geometry.Figures)
            {
                PathFigure newfigure = new PathFigure() {  IsClosed = true};
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
            //
            SetColor();
            StrokeThickness = strokethickness;
        }


        private void SetColor()
        {
            switch (View)
            {
                case ViewArea.table_train:
                    {
                        Stroke = _color_table_train;
                        Fill = _colordefult;
                    }
                    break;
                case ViewArea.area_message:
                    {
                        Stroke = _color_area_message;
                        Fill = _colordefult;
                    }
                    break;
                case ViewArea.area_station:
                    {
                        Stroke = _color_area_station;
                        Fill = _colordefult;
                    }
                    break;
                case ViewArea.table_autopilot:
                    {
                        Stroke = _color_table_autopilot;
                        Fill = _colordefult;
                    }
                    break;
                case ViewArea.area_picture:
                    {
                        Stroke = _color_area_picture;
                        if (!string.IsNullOrEmpty(Path))
                            SetFillColor(Path);
                        else
                            Fill = _colordefult;
                    }
                    break;
                default:
                    {
                        Stroke = _color_table_train;
                        Fill = _colordefult;
                    }
                    break;
            }
        }

        public void SetFillColor(string path)
        {
            try
            {
                TransformedBitmap tb = new TransformedBitmap();
                // Create the source to use as the tb source.
                BitmapImage bi = new BitmapImage(new Uri(path));
                // Properties must be set between BeginInit and EndInit calls.
                tb.BeginInit();
                tb.Source = bi;
                RotateTransform transform = new RotateTransform(Angle);
                tb.Transform = transform;
                tb.EndInit();
                Fill = new ImageBrush(tb);

            }
            catch { }
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
            //
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
        }

        /// <summary>
        /// поворачиваем элемент 
        /// </summary>
        /// <param name="angle">угол поворота</param>
        public void Rotate(double angle) 
        {
            Angle += (int)angle;
            if (Math.Abs(Angle) > 360)
            {
                int ostatok = (int)Math.Abs(RotateText) / 360;
                if (Angle >= 0)
                    Angle -= (ostatok * 360);
                else Angle += (ostatok * 360);
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
                }
            }
           
        }

        /// <summary>
        /// установливаем цвет по умолчанию
        /// </summary>
        public void DefaultColor()
        {
            SetColor();
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
