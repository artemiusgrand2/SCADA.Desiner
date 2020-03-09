using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.GeometryTransform;
using SCADA.Desiner.WindowWork;
using SCADA.Desiner.Enums;
using SCADA.Common.Enums;


namespace SCADA.Desiner.CommandElement
{
    /// <summary>
    /// класс описывающий управдяющую кнопку
    /// </summary>
    class ButtonCommand: Shape, IGraficObejct, IText
    {
        #region Переменные и свойства

        public bool IsVisible { get; set; }
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
        /// <summary>
        /// справочный текст
        /// </summary>
        public string HelpText { get; set; }
        /// <summary>
        /// параматры
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// Ид объекта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// радиус закругления концов отрезков
        /// </summary>
        double R = 2.5;
        /// <summary>
        /// ширина по умолчанию
        /// </summary>
        double widthdefult = 43;
        /// <summary>
        /// высота по умолчанию
        /// </summary>
        double heightdefult = 22;
        /// <summary>
        /// минимальная ширина
        /// </summary>
        double _minwidth = 10;
        /// <summary>
        /// минимальная высота
        /// </summary>
        double _minheight = 10;
        /// <summary>
        /// толщина линии  по умолчанию
        /// </summary>
        public static double _strokethickness = 1;
        /// <summary>
        /// цвет заполнения по умолчанию
        /// </summary>
        Brush _colordefult =  new LinearGradientBrush(Color.FromRgb(170, 170, 170), Color.FromRgb(225, 225, 225), 45);
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
        Brush _colordefultstroke = Brushes.Brown;
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
        private TextBlock _text = new TextBlock() { FontWeight = FontWeights.Bold };
        /// <summary>
        /// тескт названия объекта
        /// </summary>
        public TextBlock Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
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
        /// вид управляющей команды
        /// </summary>
        public ViewCommand ViewCommand { get; set; }
        //
        /// <summary>
        /// вид панели к которой относиться управляющий элемент
        /// </summary>
        public ViewPanel ViewPanel { get; set; }
        /// <summary>
        /// центр фигуры
        /// </summary>
        Point _center_figure = new Point();

        ViewElement viewElement = ViewElement.buttoncommand;
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
        public ButtonCommand( double X, double Y)
        {
            StartSize();
            GeometryFigure(X, Y); 
            IsVisible = true;
        }

        public ButtonCommand(PathGeometry geometry, double strokethickness, double left, double top, string name, string help, string parametrs, double Rotate, double FontSize, ViewCommand viewcommand, ViewPanel viewpanel, int station)
        {
            ViewCommand = viewcommand;
            ViewPanel = viewpanel;
            StationNumber = station;
            NameObject = name;
            HelpText = help;
            Parameters = parametrs;
            _text.Text = (NameObject != null) ? NameObject.Split(new char[] { ';' })[0] : string.Empty;
            _text.FontSize = FontSize;
            RotateText = Rotate;
            _text.Margin = new Thickness(left, top, 0, 0);
            _text.RenderTransform = new RotateTransform(RotateText);
            GeometryFigureCopy(geometry, strokethickness);
        }

        public SizeElement SizeGeometry()
        {
            Point P1 = ((ArcSegment)Figure.Figures[0].Segments[2]).Point;
            Point P2 = ((ArcSegment)Figure.Figures[0].Segments[0]).Point;
            Point P3 = ((LineSegment)Figure.Figures[0].Segments[5]).Point;
            return new SizeElement() { Height = Math.Round(Math.Abs(Figure.Figures[0].StartPoint.Y - P1.Y) / OperationsGrafic.SaveScroll), Widht = Math.Round(Math.Abs(P2.X - P3.X) / OperationsGrafic.SaveScroll) };
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
                    switch (Index.IndexSegment)
                    {
                        case 1:
                            {
                                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                                ArcSegment arc = _figure.Figures[Index.IndexFigure].Segments[4] as ArcSegment;
                                if (LenghtStorona(arc.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) >= _minwidth)
                                {
                                    lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                    //арка впереди
                                    ArcSegment arcpered = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as ArcSegment;
                                    arcpered.Point = new Point(arcpered.Point.X + deltax, arcpered.Point.Y + deltay);
                                    //арка сзади
                                    ArcSegment arcrevers = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as ArcSegment;
                                    arcrevers.Point = new Point(arcrevers.Point.X + deltax, arcrevers.Point.Y + deltay);
                                    //Начальная точка
                                    _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                                    //последний элемент
                                    LineSegment line_post = _figure.Figures[Index.IndexFigure].Segments[7] as LineSegment;
                                    line_post.Point = new Point(line_post.Point.X + deltax, line_post.Point.Y + deltay);
                                }
                            }
                            break;
                        case 3:
                            {
                                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                                ArcSegment arc = _figure.Figures[Index.IndexFigure].Segments[6] as ArcSegment;
                                if (LenghtStorona(arc.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) >= _minheight)
                                {
                                    lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                    //арка впереди
                                    ArcSegment arcpered = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as ArcSegment;
                                    arcpered.Point = new Point(arcpered.Point.X + deltax, arcpered.Point.Y + deltay);
                                    //арка сзади
                                    ArcSegment arcrevers = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as ArcSegment;
                                    arcrevers.Point = new Point(arcrevers.Point.X + deltax, arcrevers.Point.Y + deltay);
                                    //последний элемент
                                    LineSegment line_post = _figure.Figures[Index.IndexFigure].Segments[1] as LineSegment;
                                    line_post.Point = new Point(line_post.Point.X + deltax, line_post.Point.Y + deltay);
                                }
                            }
                            break;
                        case 5:
                            {
                                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                                ArcSegment arc = _figure.Figures[Index.IndexFigure].Segments[0] as ArcSegment;
                                if (LenghtStorona(arc.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) >= _minwidth)
                                {
                                    lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                    //арка впереди
                                    ArcSegment arcpered = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as ArcSegment;
                                    arcpered.Point = new Point(arcpered.Point.X + deltax, arcpered.Point.Y + deltay);
                                    //арка сзади
                                    ArcSegment arcrevers = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as ArcSegment;
                                    arcrevers.Point = new Point(arcrevers.Point.X + deltax, arcrevers.Point.Y + deltay);
                                    //последний элемент
                                    LineSegment line_post = _figure.Figures[Index.IndexFigure].Segments[3] as LineSegment;
                                    line_post.Point = new Point(line_post.Point.X + deltax, line_post.Point.Y + deltay);
                                }
                            }
                            break;
                        case 7:
                            {
                                LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                                ArcSegment arc = _figure.Figures[Index.IndexFigure].Segments[2] as ArcSegment;
                                if (LenghtStorona(arc.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) >= _minheight)
                                {
                                    lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                    //арка впереди
                                    ArcSegment arcpered = _figure.Figures[Index.IndexFigure].Segments[0] as ArcSegment;
                                    arcpered.Point = new Point(arcpered.Point.X + deltax, arcpered.Point.Y + deltay);
                                    //арка сзади
                                    ArcSegment arcrevers = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as ArcSegment;
                                    arcrevers.Point = new Point(arcrevers.Point.X + deltax, arcrevers.Point.Y + deltay);
                                    //последний элемент
                                    LineSegment line_post = _figure.Figures[Index.IndexFigure].Segments[5] as LineSegment;
                                    line_post.Point = new Point(line_post.Point.X + deltax, line_post.Point.Y + deltay);
                                    //Начальная точка
                                    _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                                }
                            }
                            break;
                    }
                    //центрируем надпись
                    double width = LenghtStorona(((ArcSegment)_figure.Figures[0].Segments[_figure.Figures[0].Segments.Count - 2]).Point, _figure.Figures[0].StartPoint);
                    double height = LenghtStorona(((ArcSegment)_figure.Figures[0].Segments[2]).Point, _figure.Figures[0].StartPoint);
                    _text.FontSize = HelpesCalculation.FontSizeText(WorkGrafic.Kwtext, WorkGrafic.Khtext,
                        new Rectangle()
                        {
                            Width = width,
                            Height = height
                        },
                    _text.Text, RotateText);
                    _text.Margin = HelpesCalculation.AlingmentCenter(_figure.Figures[0].StartPoint.X, _figure.Figures[0].StartPoint.Y, width, height, _text, this);
                    //
                }
            }
            catch { }
        }

        private void StartSize()
        {
            if (heightdefult > widthdefult)
                R = widthdefult * 0.1;
            else R = heightdefult * 0.1;
            //
            widthdefult *= OperationsGrafic.SaveScroll;
            heightdefult *= OperationsGrafic.SaveScroll;
            R *= OperationsGrafic.SaveScroll;
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

        // <summary>
        // приводим к оригинальным координатм
        // </summary>
        //public void OriginalKoordinate(Point center, double scroll)
        //{
        //    foreach (PathFigure geo in _figure.Figures)
        //    {
        //        geo.StartPoint = new Point((geo.StartPoint.X - center.X) / scroll, (geo.StartPoint.Y - center.Y) / scroll);

        //        foreach (PathSegment seg in geo.Segments)
        //        {
        //            сегмент линия
        //            LineSegment lin = seg as LineSegment;
        //            if (lin != null)
        //            {
        //                lin.Point = new Point((lin.Point.X - center.X) / scroll, (lin.Point.Y - center.Y) / scroll);
        //                continue;
        //            }
        //            сегмент арка
        //            ArcSegment arc = seg as ArcSegment;
        //            if (arc != null)
        //            {
        //                arc.Point = new Point((arc.Point.X - center.X) / scroll, (arc.Point.Y - center.Y) / scroll);
        //                arc.Size = new Size(arc.Size.Width/scroll, arc.Size.Height/scroll);
        //                continue;
        //            }
        //        }
        //    }
            
        //    _text.Margin = new Thickness((_text.Margin.Left - center.X) / scroll, (_text.Margin.Top - center.Y) / scroll, 0, 0);
        //    _text.FontSize = (_text.FontSize/scroll);
        //    StrokeThickness = _strokethickness;
        //}


        public void SetBackKoordinate(Point centerhistory, double scrollhistory, Point centercurrent, double scrollcurrent)
        {
            foreach (PathFigure geo in _figure.Figures)
            {

                geo.StartPoint = new Point(((geo.StartPoint.X - centerhistory.X) * scrollcurrent / scrollhistory) + centercurrent.X, ((geo.StartPoint.Y - centerhistory.Y) * scrollcurrent / scrollhistory) + centercurrent.Y);

                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                        lin.Point = new Point(((lin.Point.X - centerhistory.X) * scrollcurrent / scrollhistory) + centercurrent.X, ((lin.Point.Y - centerhistory.Y) * scrollcurrent / scrollhistory) + centercurrent.Y);
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                    {
                        arc.Point = new Point(((arc.Point.X - centerhistory.X) * scrollcurrent / scrollhistory) + centercurrent.X, ((arc.Point.Y - centerhistory.Y) * scrollcurrent / scrollhistory) + centercurrent.Y);
                        arc.Size = new Size(arc.Size.Height * (scrollcurrent / scrollhistory), arc.Size.Width * (scrollcurrent / scrollhistory));
                        continue;
                    }
                }
            }
            //
            _text.Margin = new Thickness(((_text.Margin.Left - centerhistory.X) * scrollcurrent / scrollhistory) + centercurrent.X, ((_text.Margin.Top - centerhistory.Y) * scrollcurrent / scrollhistory) + centercurrent.Y, 0, 0);
            _text.FontSize = (_text.FontSize * (scrollcurrent / scrollhistory));

            StrokeThickness = _strokethickness * scrollcurrent;
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
            //
            return null;
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
        private void GeometryFigure(double X, double Y)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure newfigure = new PathFigure() {  StartPoint = new Point(X + R, Y) , IsClosed = true};
            newfigure.Segments.Add(new ArcSegment() { Point = new Point(X, Y + R), Size = new Size(R, R) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X, Y + (heightdefult - R)) });
            newfigure.Segments.Add(new ArcSegment() { Point = new Point(X + R, Y + heightdefult), Size = new Size(R, R) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X + (widthdefult - R), Y + heightdefult) });
            newfigure.Segments.Add(new ArcSegment() { Point = new Point(X + widthdefult, Y + (heightdefult - R)), Size = new Size(R, R) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X + widthdefult, Y + R) });
            newfigure.Segments.Add(new ArcSegment() { Point = new Point(X + (widthdefult - R), Y), Size = new Size(R, R) });
            newfigure.Segments.Add(new LineSegment() { Point = newfigure.StartPoint});
            geometry.Figures.Add(newfigure);
            _figure = geometry;
            Stroke = _colordefultstroke;
            Fill = _colordefult;
            StrokeThickness = _strokethickness;
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
            _minheight *= OperationsGrafic.CurrentScroll;
            _minwidth *= OperationsGrafic.CurrentScroll;
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
            //
            _text.Margin = new Thickness(_text.Margin.Left+ deltaX, _text.Margin.Top + deltaY, 0,0);
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
            Point point = scaletransform.Transform(new Point(_text.Margin.Left, _text.Margin.Top));
            _text.Margin = new Thickness(point.X, point.Y, 0, 0);
            _text.FontSize *= scale;
            //
            StrokeThickness *= scale;
            _minheight *= scale;
            _minwidth *= scale;
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
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                    {
                        arc.Point = rotate.Transform(arc.Point);
                        continue;
                    }
                }
                Point NewMargin = rotate.Transform(new Point(_text.Margin.Left,_text.Margin.Top));
                //поворачиваем текст
                _text.RenderTransform = new RotateTransform(RotateText);
                _text.Margin = new Thickness(NewMargin.X,NewMargin.Y,0,0);
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
             //CreateCollectionLines();
             //switch (aligmnet)
             //{
             //    case AligmentMirror.horizontal:
             //        {
             //            SizeFigure(2 * (point.X - _center_figure.X), 0);
             //        }
             //        break;
             //    case AligmentMirror.vertical:
             //        {
             //            SizeFigure(0, 2 * (point.Y - _center_figure.Y));
             //        }
             //        break;
             //}
         }
    }
}
