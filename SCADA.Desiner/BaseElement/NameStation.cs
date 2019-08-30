using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
    /// класс описывающий элемент название станции
    /// </summary>
    class NameStation : Shape, IGraficObejct, IFontSize, IText
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
        /// ширина по умолчанию
        /// </summary>
        double widthdefult = 100;
        /// <summary>
        /// высота по умолчанию
        /// </summary>
        double heightdefult = 25;
        /// <summary>
        /// высота текста по умолчанию
        /// </summary>
        public double heighttext = 16;
        /// <summary>
        /// минимальная ширина
        /// </summary>
        double _minwidth = 1;
        /// <summary>
        /// минимальная высота
        /// </summary>
        double _minheight = 1;
        /// <summary>
        /// толщина рамки по умолчанию
        /// </summary>
        public static double strokethickness = 1;
        /// <summary>
        /// название элемента
        /// </summary>
        public string NameObject { get; set; }
        /// <summary>
        /// пояснения
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// идентификатор элемента
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// цвет фона по умолчанию
        /// </summary>
        Brush _colordefult = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        /// <summary>
        /// цвет выделенного объекта
        /// </summary>
        Brush _colorselect = new SolidColorBrush(Color.FromArgb(190, 170, 0, 170));
        /// <summary>
        /// цвет выделенной рамки
        /// </summary>
        Brush _colorselectrstroke = Brushes.Red;
        /// <summary>
        /// цвет рамки по умолчанию
        /// </summary>
        Brush _colordefultstroke = new SolidColorBrush(Color.FromRgb(225, 225, 225));
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
        /// <summary>
        /// показывет выделенную сторону
        /// </summary>
        public  Index Index { get; set; }
        private TextBlock _text = new TextBlock() { FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Center };
        /// <summary>
        /// элемент для отрисовки названия станции
        /// </summary>
        public TextBlock Text
        {
            get { return _text; }
            set { _text = value; }
        }
        /// <summary>
        /// текущий угол повотора текста
        /// </summary>
        public double RotateText { get; set; }
        /// <summary>
        /// центр фигуры
        /// </summary>
        Point _center_figure = new Point();

        ViewElement viewElement = ViewElement.namestation;
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
        //
        #endregion

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="kscroll">масштаб</param>
        /// <param name="stationnumber">номер станции</param>
        /// <param name="X">координата X точки вставки</param>
        /// <param name="Y">координата Y точки вставки</param>
        public NameStation(double kscroll, int stationnumber, string station, double X, double Y)
        {
            TransformGroup group_transform = new TransformGroup();
            group_transform.Children.Add(new TranslateTransform());
            _text.RenderTransform = group_transform;
            StationNumber = stationnumber;
            _text.Text = station;
            NameObject = station;
            GeometryFigure( X, Y);

        }

        public NameStation(int stationnumber, PathGeometry geometry, string station, double strokethickness, TextBlock text)
        {
            StationNumber = stationnumber;
            _text.Text = station;
            NameObject = station;
            GeometryFigureCopy(geometry, strokethickness, text);
        }

        public SizeElement SizeGeometry()
        {
            Point P1 = ((LineSegment)Figure.Figures[0].Segments[0]).Point;
            Point P2 = ((LineSegment)Figure.Figures[0].Segments[1]).Point;
            return new SizeElement() { Height = Math.Round(Math.Abs(Figure.Figures[0].StartPoint.Y - P2.Y) / OperationsGrafic.SaveScroll),
                Widht = Math.Round(Math.Abs(Figure.Figures[0].StartPoint.X - P1.X) / OperationsGrafic.SaveScroll) };
        }

        public Point PointInsert()
        {
            return new Point(_text.Margin.Left / System.Windows.SystemParameters.CaretWidth, _text.Margin.Top / System.Windows.SystemParameters.CaretWidth);
        }

        public void Resize(double deltax, double deltay)
        {
            if (Index != null)
            {
                if (Index.XY)
                    deltay = 0;
                else deltax = 0;

                switch (Index.IndexSegment)
                {
                    case 0:
                        {

                            LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                            LineSegment lin2 = _figure.Figures[Index.IndexFigure].Segments[_figure.Figures[Index.IndexFigure].Segments.Count - 1] as LineSegment;
                            LineSegment lin3 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as LineSegment;
                            //
                            if ((LenghtStorona(lin3.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) - 2 * StrokeThickness) >= _minheight)
                            {
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                                _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                                _text.Margin = new Thickness(_text.Margin.Left + deltax, _text.Margin.Top + deltay, 0, 0);
                            }
                        }
                        break;
                    case 1:
                        {
                            LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                            LineSegment lin2 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as LineSegment;
                            LineSegment lin3 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as LineSegment;
                            //
                            if ((LenghtStorona(lin3.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) - 2 * StrokeThickness) >= _minwidth)
                            {
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                            }
                        }
                        break;
                    case 2:
                        {
                            LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                            LineSegment lin2 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as LineSegment;
                            LineSegment lin3 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as LineSegment;
                            //
                            if ((LenghtStorona(lin3.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) - 2 * StrokeThickness) > _minheight)
                            {
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                            }
                        }
                        break;
                    case 3:
                        {
                            LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                            LineSegment lin2 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as LineSegment;
                            LineSegment lin3 = _figure.Figures[Index.IndexFigure].Segments[0] as LineSegment;
                            if ((LenghtStorona(lin3.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) - 2 * StrokeThickness) >= _minwidth)
                            {
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                                _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                                _text.Margin = new Thickness(_text.Margin.Left + deltax, _text.Margin.Top + deltay, 0, 0);
                            }
                        }
                        break;
                }
                //изменяем положение текста
                if (_figure.Figures.Count > 0)
                {
                    LineSegment lin1 = _figure.Figures[0].Segments[0] as LineSegment;
                    if (lin1 != null)
                        _text.MaxWidth = LenghtStorona(_figure.Figures[0].StartPoint, lin1.Point) - 2 * StrokeThickness;
                    //
                    LineSegment lin2 = _figure.Figures[0].Segments[1] as LineSegment;
                    if (lin2 != null)
                        _text.MaxHeight = LenghtStorona(lin1.Point, lin2.Point) - 2 * StrokeThickness;
                }
                //
            }
        }

        private void SizeNew(double deltax, double deltay)
        {
            if (Index != null)
            {
                if (Index.XY)
                    deltay = 0;
                else deltax = 0;

                switch (Index.IndexSegment)
                {
                    case 0:
                        {
                            
                            LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                            LineSegment lin2 = _figure.Figures[Index.IndexFigure].Segments[_figure.Figures[Index.IndexFigure].Segments.Count - 1] as LineSegment;
                            LineSegment lin3 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as LineSegment;
                            //
                            if ((LenghtStorona(lin3.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) - 2 * StrokeThickness) >= _minheight)
                            {
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                                _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                                _text.Margin = new Thickness(_text.Margin.Left + deltax, _text.Margin.Top + deltay, 0, 0);
                            }
                        }
                        break;
                    case 1:
                        {
                            LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                            LineSegment lin2 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment-1] as LineSegment;
                            LineSegment lin3 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as LineSegment;
                            //
                            if ((LenghtStorona(lin3.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) - 2 * StrokeThickness) >= _minwidth)
                            {
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                            }
                        }
                        break;
                    case 2:
                        {
                            LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                            LineSegment lin2 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as LineSegment;
                            LineSegment lin3 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment + 1] as LineSegment;
                            //
                            if ((LenghtStorona(lin3.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) - 2 * StrokeThickness) > _minheight)
                            {
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                            }
                        }
                        break;
                    case 3:
                        {
                            LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                            LineSegment lin2 = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment - 1] as LineSegment;
                            LineSegment lin3 = _figure.Figures[Index.IndexFigure].Segments[0] as LineSegment;
                            if ((LenghtStorona(lin3.Point, new Point(lin.Point.X + deltax, lin.Point.Y + deltay)) - 2 * StrokeThickness) >= _minwidth)
                            {
                                lin.Point = new Point(lin.Point.X + deltax, lin.Point.Y + deltay);
                                lin2.Point = new Point(lin2.Point.X + deltax, lin2.Point.Y + deltay);
                                _figure.Figures[Index.IndexFigure].StartPoint = new Point(_figure.Figures[Index.IndexFigure].StartPoint.X + deltax, _figure.Figures[Index.IndexFigure].StartPoint.Y + deltay);
                                _text.Margin = new Thickness(_text.Margin.Left + deltax, _text.Margin.Top + deltay, 0, 0);
                            }
                        }
                        break;
                }  
                //изменяем положение текста
                if (_figure.Figures.Count > 0)
                {
                    LineSegment lin1 = _figure.Figures[0].Segments[0] as LineSegment;
                    if (lin1 != null)
                        _text.MaxWidth = LenghtStorona(_figure.Figures[0].StartPoint, lin1.Point)- 2*StrokeThickness;
                    //
                    LineSegment lin2 = _figure.Figures[0].Segments[1] as LineSegment;
                    if (lin2 != null)
                        _text.MaxHeight = LenghtStorona(lin1.Point, lin2.Point) - 2 * StrokeThickness;
                }
                //
            }
        }
        /// <summary>
        /// определяемдлину отрезка между двумя точками
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
                    if (lin.Point.X == CurrentLine.X2 && lin.Point.Y == CurrentLine.Y2)
                    {
                        if (Math.Abs(CurrentLine.X1 - CurrentLine.X2) <= Math.Abs(CurrentLine.Y1 - CurrentLine.Y2))
                            return new Index() { IndexFigure = i, IndexSegment = j, XY = true };
                        else 
                           return new Index() { IndexFigure = i, IndexSegment = j, XY = false };                           
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
                        if (Math.Abs(CurrentLine.X1 - CurrentLine.X2) <= Math.Abs(CurrentLine.Y1 - CurrentLine.Y2))
                            return AligmentStroke.horizontal;
                        else
                            return AligmentStroke.vertical;
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
            heightdefult *= OperationsGrafic.SaveScroll;
            widthdefult *= OperationsGrafic.SaveScroll;
            //
            PathGeometry geometry = new PathGeometry();
            PathFigure newfigure = new PathFigure() { StartPoint = new Point(X, Y) , IsClosed = true};
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X + widthdefult, Y) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X + widthdefult, Y + heightdefult) });
            newfigure.Segments.Add(new LineSegment() { Point = new Point(X, Y + heightdefult) });
            newfigure.Segments.Add(new LineSegment() { Point = newfigure.StartPoint });
            geometry.Figures.Add(newfigure);
            _figure = geometry;
            //
            Stroke = _colordefultstroke;
            Fill = _colordefult;
            StrokeThickness = strokethickness;
            //
            _text.FontSize = heighttext;
            _text.MaxWidth =  widthdefult - StrokeThickness;
            _text.MaxHeight = heightdefult - StrokeThickness;
             //
            _text.Margin = new Thickness(X+ StrokeThickness, Y + StrokeThickness, 0, 0);
        }

        private void GeometryFigureCopy(PathGeometry geometry, double strokethickness, TextBlock textcopy)
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
            Stroke = _colordefultstroke;
            StrokeThickness = strokethickness;
            //настраиваем тест названия
            _text.MaxHeight = textcopy.MaxHeight;
            _text.MaxWidth = textcopy.MaxWidth;
            _text.Margin = new Thickness(textcopy.Margin.Left, textcopy.Margin.Top, 0, 0);
            if(textcopy.RenderTransform is RotateTransform)
                _text.RenderTransform = new RotateTransform((textcopy.RenderTransform as RotateTransform).Angle);
            _text.FontSize = textcopy.FontSize;
            //
            _minwidth *= OperationsGrafic.CurrentScroll;
            _minheight *= OperationsGrafic.CurrentScroll;
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
            //if (_text.RenderTransform is TransformGroup)
            //{
            //    TransformGroup group_transform = _text.RenderTransform as TransformGroup;
            //    if (group_transform.Children.Count > 0)
            //    {
            //        if (group_transform.Children[0] is TranslateTransform)
            //        {
            //            TranslateTransform move_transform = group_transform.Children[0] as TranslateTransform;
            //            move_transform.X += deltaX;
            //            move_transform.Y += deltaY;
            //        }
            //    }
            //}
            _text.Margin = new Thickness(_text.Margin.Left + deltaX, _text.Margin.Top + deltaY, 0, 0);
        }

        /// <summary>
        /// масштабироание объекта
        /// </summary>
        /// <param name="scale">масштаб</param>
        public void ScrollFigure(ScaleTransform scaletransform, double scale) { }

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
                //
                Point NewMargin = rotate.Transform(new Point(_text.Margin.Left, _text.Margin.Top));
                _text.RenderTransform = new RotateTransform(RotateText);
                _text.Margin = new Thickness(NewMargin.X, NewMargin.Y, 0, 0); 
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
        ///<summary>
        ///установливаем цвет при выборе объекта
        ///</summary>
        public void SelectColor()
        {
            if (_selectstroke)
                Stroke = _colorselectrstroke;
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
