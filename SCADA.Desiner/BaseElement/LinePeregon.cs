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
using SCADA.Common.Enums;
using SCADA.Desiner.WindowWork;

namespace SCADA.Desiner.BaseElement
{
    /// <summary>
    /// класс описывающий путь перегона
    /// </summary>
    class LinePeregon : Shape, IGraficObejct, IDeleteElement, IShowSettings, IFreeAngle
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
        TransformGroup groupTransform = new TransformGroup();
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
        /// толщина линии  по умолчанию
        /// </summary>
        double _strokethickness = 14;
        /// <summary>
        /// цвет выделенного контура
        /// </summary>
        Brush _colorselectstroke = new SolidColorBrush(Color.FromArgb(190, 170, 0, 170));
        /// <summary>
        /// цвет контура по умолчанию
        /// </summary>
        Brush _colordefultstroke = new SolidColorBrush(Color.FromRgb(170, 170, 170));

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
        /// коэффициент увеличения
        /// </summary>
        private double _sctollrectangle = 1;
        public double RamkaAllocation
        {
            get
            {
                return _sctollrectangle;
            }
        }
        ViewElement viewElement = ViewElement.otrezok;
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
        public LinePeregon(int stationnumber)
        {
            RenderTransform = groupTransform;
            StationNumber = stationnumber;
            StartSize();
            GeometryFigure();
            ScrollRamka();
        }

        public LinePeregon(int stationnumber, PathGeometry geometry, string name)
        {
            RenderTransform = groupTransform;
            NameObject = name;
            StationNumber = stationnumber;
            StartSize();
            GeometryFigureCopy(geometry, _strokethickness);
            ScrollRamka();
        }

        public LinePeregon(int stationnumber, PathGeometry geometry, double strokethickness, string name)
        {
            RenderTransform = groupTransform;
            NameObject = name;
            StationNumber = stationnumber;
            GeometryFigureCopy(geometry, strokethickness);
            StartSize();
            ScrollRamka();
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
            if (Index != null)
            {
                if (Index.IndexSegment == -1)
                {
                    Point point = WorkGrafic.RoundPoint(_figure.Figures[Index.IndexFigure].StartPoint, deltax, deltay);
                    deltax = point.X - _figure.Figures[Index.IndexFigure].StartPoint.X;
                    deltay = point.Y - _figure.Figures[Index.IndexFigure].StartPoint.Y;
                    _figure.Figures[Index.IndexFigure].StartPoint = point;
                }
                else
                {
                    LineSegment lin = _figure.Figures[Index.IndexFigure].Segments[Index.IndexSegment] as LineSegment;
                    Point point = WorkGrafic.RoundPoint(lin.Point, deltax, deltay);
                    deltax = point.X - lin.Point.X;
                    deltay = point.Y - lin.Point.Y;
                    lin.Point = point;
                }
                OperationsGrafic.FramePoint.Margin = new Thickness(OperationsGrafic.FramePoint.Margin.Left + deltax, OperationsGrafic.FramePoint.Margin.Top + deltay, 0, 0);
            }
        }

        private void StartSize()
        {
            double buffer = 0;
            if (ConfigurationManager.AppSettings["thickness_signal"] != null && double.TryParse(ConfigurationManager.AppSettings["thickness_signal"], out buffer))
            {
                double thickness = double.Parse(ConfigurationManager.AppSettings["thickness_signal"]);
                if (thickness > 1 && thickness < 1000)
                    _strokethickness = thickness * OperationsGrafic.SaveScroll;
            }
        }

        /// <summary>
        /// обратный отражение точек перегона
        /// </summary>
        public void Reverse()
        {
            try
            {
                if (_figure.Figures.Count > 0)
                {
                    for (int i = 0; i < _figure.Figures.Count; i++)
                    {
                        if (_figure.Figures[i].Segments.Count > 1)
                        {
                            PathFigure reversefigure = new PathFigure() { StartPoint = new Point(((LineSegment)_figure.Figures[i].Segments[_figure.Figures[i].Segments.Count - 1]).Point.X, ((LineSegment)_figure.Figures[i].Segments[_figure.Figures[i].Segments.Count - 1]).Point.Y) };
                            //
                            for (int j = _figure.Figures[i].Segments.Count - 2; j >= 0; j--)
                                reversefigure.Segments.Add(_figure.Figures[i].Segments[j]);
                            //
                            reversefigure.Segments.Add(new LineSegment() { Point = new Point(_figure.Figures[i].StartPoint.X, _figure.Figures[i].StartPoint.Y) });
                            _figure.Figures[i] = reversefigure;
                        }
                        //
                        if (_figure.Figures[i].Segments.Count == 1)
                        {
                            PathFigure reversefigure = new PathFigure() { StartPoint = new Point(((LineSegment)_figure.Figures[i].Segments[_figure.Figures[i].Segments.Count - 1]).Point.X, ((LineSegment)_figure.Figures[i].Segments[_figure.Figures[i].Segments.Count - 1]).Point.Y) };
                            reversefigure.Segments.Add(new LineSegment() { Point = new Point(_figure.Figures[i].StartPoint.X, _figure.Figures[i].StartPoint.Y) });
                            _figure.Figures[i] = reversefigure;
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// удаление точек линии
        /// </summary>
        public void DeletePoint()
        {
            try
            {
                if (Index != null)
                {
                    if (Index.IndexSegment == -1)
                    {
                        if (_figure.Figures[Index.IndexFigure].Segments.Count >= 2)
                        {
                            LineSegment line = _figure.Figures[Index.IndexFigure].Segments[0] as LineSegment;
                            if (line != null)
                            {
                                _figure.Figures[Index.IndexFigure].StartPoint = new Point(line.Point.X, line.Point.Y);
                                _figure.Figures[Index.IndexFigure].Segments.RemoveAt(0);
                            }
                        }
                    }
                    else
                    {
                        if (_figure.Figures[Index.IndexFigure].Segments.Count >= 2)
                            _figure.Figures[Index.IndexFigure].Segments.RemoveAt(Index.IndexSegment);
                    }
                }
            }
            catch { }
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
                if (_figure.Figures[i].StartPoint.X == CurrentLine.X1 && _figure.Figures[i].StartPoint.Y == CurrentLine.Y1)
                {
                    OperationsGrafic.FramePoint = new Rectangle() { Width = _sctollrectangle * StrokeThickness, Height = _sctollrectangle * StrokeThickness, Stroke = Brushes.Green, StrokeThickness = 2 * OperationsGrafic.SaveScroll };
                    OperationsGrafic.FramePoint.Margin = new Thickness(_figure.Figures[i].StartPoint.X - (_sctollrectangle / 2) * StrokeThickness, _figure.Figures[i].StartPoint.Y - (_sctollrectangle / 2) * StrokeThickness, 0, 0);
                    return new Index() { IndexFigure = i, IndexSegment = -1 };
                }
                //
                for (int j = 0; j < _figure.Figures[i].Segments.Count; j++)
                {
                    LineSegment lin = _figure.Figures[i].Segments[j] as LineSegment;
                    if (lin.Point.X == CurrentLine.X1 && lin.Point.Y == CurrentLine.Y1)
                    {
                        OperationsGrafic.FramePoint = new Rectangle() { Width = _sctollrectangle * StrokeThickness, Height = _sctollrectangle * StrokeThickness, Stroke = Brushes.Green, StrokeThickness = 2 * OperationsGrafic.SaveScroll };
                        OperationsGrafic.FramePoint.Margin = new Thickness(lin.Point.X - (_sctollrectangle / 2) * StrokeThickness, lin.Point.Y - (_sctollrectangle / 2) * StrokeThickness, 0, 0);
                        return new Index() { IndexFigure = i, IndexSegment = j };
                    }
                }
            }
            return null;
            //
        }

        public Index FindIndexLine(int figure, int segment)
        {
            try
            {
                if (segment == SCADA.Desiner.Tools.SettingsFigure.NSP)
                {
                    OperationsGrafic.FramePoint = new Rectangle() { Width = _sctollrectangle * StrokeThickness, Height = _sctollrectangle * StrokeThickness, Stroke = Brushes.Green, StrokeThickness = 2 * OperationsGrafic.SaveScroll };
                    OperationsGrafic.FramePoint.Margin = new Thickness(_figure.Figures[figure].StartPoint.X - (_sctollrectangle / 2) * StrokeThickness, _figure.Figures[figure].StartPoint.Y - (_sctollrectangle / 2) * StrokeThickness, 0, 0);
                    return new Index() { IndexFigure = figure, IndexSegment = -1 };
                }
                else
                {
                    LineSegment lin = _figure.Figures[figure].Segments[segment] as LineSegment;
                    OperationsGrafic.FramePoint = new Rectangle() { Width = _sctollrectangle * StrokeThickness, Height = _sctollrectangle * StrokeThickness, Stroke = Brushes.Green, StrokeThickness = 2 * OperationsGrafic.SaveScroll };
                    OperationsGrafic.FramePoint.Margin = new Thickness(lin.Point.X - (_sctollrectangle / 2) * StrokeThickness, lin.Point.Y - (_sctollrectangle / 2) * StrokeThickness, 0, 0);
                    return new Index() { IndexFigure = figure, IndexSegment = segment };
                }
            }
            catch { return null; }
        }

        public AligmentStroke FindLineAligment(Line CurrentLine)
        {
            return AligmentStroke.none;
        }

        /// <summary>
        /// добавляем новую точку если не попал в вершину
        /// </summary>
        /// <param name="CurrentLine">линия попадания</param>
        /// <param name="Click">тоска касания</param>
        public void AddNewPointIndex(Line CurrentLine, Point Click)
        {
            for (int i = 0; i < _figure.Figures.Count; i++)
            {
                //
                for (int j = 0; j < _figure.Figures[i].Segments.Count; j++)
                {
                    LineSegment lin = _figure.Figures[i].Segments[j] as LineSegment;
                    if ( (lin.Point.X == CurrentLine.X2 && lin.Point.Y == CurrentLine.Y2))
                    {
                        _figure.Figures[i].Segments.Insert(j, new LineSegment() { Point = new Point(Math.Round(Click.X), Math.Round(Click.Y)) });
                        OperationsGrafic.FramePoint = new Rectangle() { Width = _sctollrectangle * StrokeThickness, Height = _sctollrectangle * StrokeThickness, Stroke = Brushes.Red, StrokeThickness = 2 * OperationsGrafic.SaveScroll };
                        OperationsGrafic.FramePoint.Margin = new Thickness(Click.X - (_sctollrectangle / 2) * StrokeThickness, Click.Y - (_sctollrectangle / 2) * StrokeThickness, 0, 0);
                        Index =   new Index() { IndexFigure = i, IndexSegment = j };
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// формирование геометрии 
        /// </summary>
        private void GeometryFigure()
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure newfigure = new PathFigure() {IsClosed = false};
            geometry.Figures.Add(newfigure);
            _figure = geometry;
            Stroke = _colordefultstroke;
            StrokeThickness = _strokethickness;
        }

        private void GeometryFigureCopy(PathGeometry geometry, double strokethickness)
        {
            foreach (PathFigure geo in geometry.Figures)
            {
                PathFigure newfigure = new PathFigure() {IsClosed = false };
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
            //if (groupTransform.Children.Count > 0 && groupTransform.Children[0] is TranslateTransform)
            //{
            //    (groupTransform.Children[0] as TranslateTransform).X += deltaX;
            //    (groupTransform.Children[0] as TranslateTransform).Y += deltaY;
            //}
            //else
            //{
            //    groupTransform.Children.Add(new TranslateTransform(deltaX, deltaY));
            //}
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
            if (_selectstroke || _selectobject)
                Stroke = _colorselectstroke;
        }

        private void ScrollRamka()
        {
            if (_strokethickness >= 7)
            {
                _sctollrectangle = 2;
                return;
            }
            //
            if (_strokethickness == 1)
            {
                _sctollrectangle = 10;
                return;
            }
            //
            if (_strokethickness >= 2 && _strokethickness <= 4)
            {
                _sctollrectangle = 5;
                return;
            }
            //
            if (_strokethickness >= 5 && _strokethickness <= 6)
            {
                _sctollrectangle = 4;
                return;
            }
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
