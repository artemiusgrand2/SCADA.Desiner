using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using SCADA.Desiner.Tools;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Constanst;
using SCADA.Desiner.BaseElement;
using SCADA.Desiner.CommandElement;
using SCADA.Desiner.Enums;
using SCADA.Desiner.Select;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.WindowWork;
using SCADA.Desiner.Delegate;
using SCADA.Common.SaveElement;
using SCADA.Common.Enums;

namespace SCADA.Desiner.GeometryTransform
{
    class OperationsGrafic
    {

        #region Словари

        Dictionary<ViewElement, ViewGroupElement> groupElement = new Dictionary<ViewElement, ViewGroupElement>()
        {
            {ViewElement.signal, ViewGroupElement.baseElement},{ViewElement.arrowmove, ViewGroupElement.baseElement},{ ViewElement.chiefroad, ViewGroupElement.baseElement},
            {ViewElement.buttonstation, ViewGroupElement.baseElement},{ViewElement.buttoncommand, ViewGroupElement.baseElement},{ViewElement.disconnectors, ViewGroupElement.baseElement},
            {ViewElement.help_element, ViewGroupElement.baseElement},{ViewElement.analogCell, ViewGroupElement.baseElement},{ViewElement.diagnostikCell, ViewGroupElement.baseElement},
            { ViewElement.kgu, ViewGroupElement.baseElement},{ViewElement.ktcm,ViewGroupElement.baseElement},
            {ViewElement.lightShunting, ViewGroupElement.baseElement},{ViewElement.lightTrain, ViewGroupElement.baseElement},{ViewElement.move, ViewGroupElement.baseElement},
            {ViewElement.tablenumbertrain, ViewGroupElement.baseElement},{ViewElement.tableautopilot, ViewGroupElement.baseElement},{ViewElement.namestation, ViewGroupElement.baseElement},
            {ViewElement.texthelp, ViewGroupElement.baseElement},{ ViewElement.time, ViewGroupElement.baseElement},{ViewElement.numbertrain, ViewGroupElement.baseElement},
            {ViewElement.ramka, ViewGroupElement.lineSegmentClosed},{ViewElement.line, ViewGroupElement.lineSegmentOpend},{ViewElement.otrezok, ViewGroupElement.lineSegmentOpend},
            {ViewElement.area, ViewGroupElement.baseElement}
        };

        /// <summary>
        /// Словарь групп элементов
        /// </summary>
        public Dictionary<ViewElement, ViewGroupElement> GroupElement
        {
            get
            {
                return groupElement;
            }
        }

        #endregion

        #region Геометричесике данные

        StrageProject projectopen;
        /// <summary>
        /// последний открытый проект
        /// </summary>
        public StrageProject ProjectOpen
        {
            get
            {
                return projectopen;
            }
            set
            {
                projectopen = value;
            }
        }

        List<History> history = new List<History>();
        /// <summary>
        /// история рисования
        /// </summary>
        public List<History> History
        {
            get
            {
                return history;
            }
        }
        private Rectangle frameRec = null;
        /// <summary>
        /// Рамка выделения
        /// </summary>
        public Rectangle FrameRec
        {
            get
            {
                return frameRec;
            }
            set
            {
                frameRec = value;
            }
        }
      
        static Rectangle framePoint;
        /// <summary>
        /// рамка выделения при попадании в точку
        /// </summary>
        public static Rectangle FramePoint
        {
            get
            {
                return framePoint;
            }
            set
            {
                framePoint = value;
            }
        }
        IGraficObejct element_select;
        /// <summary>
        /// выделенный элемент
        /// </summary>
        public IGraficObejct ElementSelect
        {
            get
            {
                return element_select;
            }
        }
        /// <summary>
        /// координата курсора по оси X
        /// </summary>
        public static double CursorX { get; set; }
        /// <summary>
        /// координата курсора по оси Y
        /// </summary>
        public static double CursorY { get; set; }
        /// <summary>
        /// текущий коэффициент масштаба
        /// </summary>
        public static double CurrentScroll { get; set; }
        /// <summary>
        /// координата относительного центра координат 
        /// </summary>
       private static Point _po = new Point(0, 0);
        public static Point Po
        {
            get
            {
                return _po;
            }
        }

        static double savescroll = 1;
        /// <summary>
        /// Масштабный коэффициент
        /// </summary>
        public static double SaveScroll
        {
            get
            {
                return savescroll;
            }
            set
            {
                savescroll = value;
            }
        }

        /// <summary>
        /// последние перемещене по оси X по клавишам
        /// </summary>
        double _deltaXKey;
        /// <summary>
        /// последние перемещение по оси Y по клавишам
        /// </summary>
        double _deltaYKey;
        /// <summary>
        /// суммарное перемещение по оси Х для перпендикулярного перемещения мышью
        /// </summary>
        double _deltaXMouse;
        /// <summary>
        /// суммарное перемещение по оси У для перпендикулярного перемещения мышью
        /// </summary>
        double _deltaYMouse;

        /// <summary>
        /// текущее значение высоты для рамки выделения
        /// </summary>
        double _heightramka;
        /// <summary>
        /// текущее значение ширины для рамки выделения
        /// </summary>
        double _widthramka;

        #endregion

        #region Контроллы

        StatusBarItem pointinsert;
        StatusBarItem elementsize;
        StatusBarItem infokoordinate;
        StatusBarItem mousekoordinate;
        StatusBarItem NameObjectement;
        StatusBarItem countselectelement;

        #endregion

        #region События

        /// <summary>
        /// добавление объектов
        /// </summary>
        public static event AddObject AddObject;
        /// <summary>
        /// удаление объектов
        /// </summary>
        public static event RemoveObject RemoveObject;
        /// <summary>
        /// обновление объектов
        /// </summary>
        public static event UpdateObject UpdateObject;
        /// <summary>
        /// обновление геометрии объектов
        /// </summary>
        public static event UpdateGeometry UpdateGeometry;
        /// <summary>
        /// список выделенных  объектов
        /// </summary>
        public static event SelectObject SelectObject;

        #endregion

        #region Флаги

       
        bool update ;
        /// <summary>
        /// проверяем было ли перемещение объектов
        /// </summary>
        public bool Update
        {
            get
            {
                return update;
            }
            set
            {
                update = value;
            }
        }

        #endregion

        #region Оперативные данные

        /// <summary>
        /// количество уровней в истории во время последнего сохранения
        /// </summary>
       public int LastCountLevelHistory { get; set; }

        int currentFreeId = 0;
        /// <summary>
        /// текущий свободный Id
        /// </summary>
        public int CurrentFreeId
        {
            get
            {
                return currentFreeId;
            }
            set
            {
                currentFreeId = value;
            }
        }
        /// <summary>
        /// количество возможных шагов назад
        /// </summary>
        int count_step_back = 10;

        #endregion

        public OperationsGrafic(StatusBarItem pointinsert, StatusBarItem elementsize, StatusBarItem infokoordinate, 
                                StatusBarItem mousekoordinate, StatusBarItem NameObjectement, StatusBarItem countselectelement)
        {
            this.pointinsert = pointinsert;
            this.elementsize = elementsize;
            this.infokoordinate = infokoordinate;
            this.mousekoordinate = mousekoordinate;
            this.NameObjectement = NameObjectement;
            this.countselectelement = countselectelement;
            CurrentScroll = 1;
            history.Add(new History());
            LastCountLevelHistory = history.Count;
        }

        public void EventUpdateGeometry(List<IGraficObejct> activeel)
        {
            if (activeel.Count > 0)
            {
                if (UpdateGeometry != null)
                    UpdateGeometry(activeel);
            }
        }

        public void EventAddobject(List<IGraficObejct> listobjects)
        {
            if (AddObject != null)
                AddObject(listobjects);
        }

        public void EventUpdateObject(List<IGraficObejct> listobjects)
        {
            if (UpdateObject != null)
                UpdateObject(listobjects);
        }

        public void EventSelectobject(List<IGraficObejct> listobjects)
        {
            if (SelectObject != null)
                SelectObject(listobjects);
        }

        public void EmptyDeltaX()
        {
            //обнуление данных
            _deltaXMouse = 0;
            _deltaYMouse = 0;
        }

        /// <summary>
        /// добавить в историю элемент
        /// </summary>
        public void AddHistory(Dictionary<int, IGraficObejct> currentDrawElement)
        {
            element_select = null;
            //проверяю количество сделанных шагов
            if (history.Count > count_step_back)
                history.RemoveAt(0);
            List<IGraficObejct> newcollection = new List<IGraficObejct>();
            foreach (KeyValuePair<int, IGraficObejct> el in currentDrawElement)
                FullGrafic(newcollection, el.Value, true);
            //
            using (History story = new History())
            {
                story.Elements = newcollection;
                //  FullTransformGroup(story.GroupTransform, groupTransform);
                history.Add(story);
            }
        }

        /// <summary>
        /// удаляем текущее состояние графики
        /// </summary>
        public void DeleteCurrentState(Canvas drawcanvas, ToolsGraphics toolspanel, ToolsManagement toolspanelcommand, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            if (toolspanel.CurrentDraw == ViewElement.line)
                toolspanel.EnabledForm();
            toolspanel.CurrentDraw = ViewElement.none;
            toolspanelcommand.CurrentDraw = ViewElement.none;
            activeel.Clear();
            if (SelectObject != null)
                SelectObject(activeel);
            drawcanvas.Children.Remove(framePoint);
            //удаляем текущее состояние
            for (int i = 0; i < drawcanvas.Children.Count; i++)
            {
                IGraficObejct graf = drawcanvas.Children[i] as IGraficObejct;
                if (graf != null)
                {
                    drawcanvas.Children.Remove(drawcanvas.Children[i]);
                    IText itext = graf as IText;
                    if (itext != null)
                    {
                        drawcanvas.Children.Remove(itext.Text);
                        i--;
                    }
                    i--;
                }
            }
            //
           currentDrawElement.Clear();
        }

        /// <summary>
        /// откат на шаг назад
        /// </summary>
        public void Setback(ToolsGraphics toolspanel, ToolsManagement toolspanelcommand, Canvas drawcanvas, Polygon lineramkadraw,
                             Dictionary<int, IGraficObejct> currentDrawElement, List<IGraficObejct> activeel, TransformGroup groupTransform)
        {
            try
            {
                if (history.Count >= 2 && toolspanel.CurrentDraw == ViewElement.none)
                {
                    //удаляем текущее состояние
                    DeleteCurrentState(drawcanvas, toolspanel, toolspanelcommand, activeel, currentDrawElement);
                    //если масштаб основной для сохраненных объектов
                    List<IGraficObejct> newposition = history[history.Count - 2].Elements;
                    //
                    List<IGraficObejct> update = new List<IGraficObejct>();
                    //
                    for (int i = 0; i < newposition.Count; i++)
                    {
                        drawcanvas.Children.Add((UIElement)newposition[i]);
                        Canvas.SetZIndex((UIElement)newposition[i], newposition[i].ZIndex);
                        currentDrawElement.Add(newposition[i].Id, newposition[i]);
                        IText itext = newposition[i] as IText;
                        if (itext != null)
                        {
                            itext.Text.RenderTransform = new RotateTransform(newposition[i].RotateText);
                            drawcanvas.Children.Add(itext.Text);
                            Canvas.SetZIndex(itext.Text, newposition[i].ZIndex +1);
                        }
                        update.Add(newposition[i]);
                    }
                    //обновляем список объектов
                    EventUpdateObject(update);
                    //удаляем предыдущую историю
                    history.RemoveAt(history.Count - 1);
                }
                //
                TransformGroup buffergroup = new TransformGroup();
                FullTransformGroup(buffergroup, groupTransform);
                OriginalSizeCurrent(false, activeel, groupTransform, lineramkadraw);
                groupTransform.Children.Clear();
                foreach (Transform trans in buffergroup.Children)
                    groupTransform.Children.Add(trans);
            }
            catch { }
        }

        private void SetSettingsObject(IGraficObejct data, IGraficObejct olddata, IList<IGraficObejct> collection, bool isSaveId)
        {
            if(isSaveId)
                data.Id = olddata.Id;
            if (olddata is IText)
                data.RotateText = olddata.RotateText;
            data.ZIndex = olddata.ZIndex;
            data.IsVisible = olddata.IsVisible;
            data.Notes = olddata.Notes;
            data.StationNumber = olddata.StationNumber;
            data.StationNumberRight = olddata.StationNumberRight;
            if (olddata is IScrollElement && data is IScrollElement)
                (data as IScrollElement).CurrencyScroll = (olddata as IScrollElement).CurrencyScroll;
            data.DefaultColor();
            collection.Add(data);
        }

        private void FullGrafic(List<IGraficObejct> collection, IGraficObejct data, bool isSaveId)
        {
            switch (data.ViewElement)
            {
                case ViewElement.signal:
                    {
                        Signal newelement = new Signal(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.area:
                    {
                        Area area = data as Area;
                        Area newelement = new Area(data.Figure, data.StrokeThickness, area.View, area.Path, area.Angle, area.NameObject, area.StationNumber, area.ZoomLevelIncrement, area.ZoomLevel);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.arrowmove:
                    {
                        ArrowMove arrowMove = data as ArrowMove;
                        ArrowMove newelement = new ArrowMove(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject, (data as IScrollElement).CurrencyScroll);
                        newelement.StationNumberRight = arrowMove.StationNumberRight;
                        newelement.Graniza = arrowMove.Graniza;
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.disconnectors:
                    {
                        Disconnectors newelement = new Disconnectors(data.StationNumber, data.StationNumberRight, data.Figure, data.StrokeThickness, data.NameObject, (data as Disconnectors).Type, (data as IScrollElement).CurrencyScroll);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.kgu:
                    {
                        KGU newelement = new KGU(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject, (data as IScrollElement).CurrencyScroll);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.ktcm:
                    {
                        KTCM newelement = new KTCM(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject, (data as IScrollElement).CurrencyScroll);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.move:
                    {
                        Move newelement = new Move(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject, (data as IScrollElement).CurrencyScroll);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.buttonstation:
                    {
                        ButtonStation newelement = new ButtonStation(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject, (data as IScrollElement).CurrencyScroll);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.diagnostikCell:
                    {
                        var command = data as DiagnostikCell;
                        var newelement = new DiagnostikCell(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.chiefroad:
                    {
                        Traintrack chiefroad = data as Traintrack;
                        Traintrack newelement = new Traintrack(data.StationNumber, data.Figure, data.StrokeThickness, chiefroad.Text.Margin.Left,
                             chiefroad.Text.Margin.Top, data.NameObject, data.RotateText, chiefroad.Text.FontSize, chiefroad.View);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.buttoncommand:
                    {
                        ButtonCommand command = data as ButtonCommand;
                        ButtonCommand newelement = new ButtonCommand(data.Figure, data.StrokeThickness, command.Text.Margin.Left, command.Text.Margin.Top, data.NameObject, command.HelpText, command.Parameters,
                           data.RotateText, command.Text.FontSize, command.ViewCommand, command.ViewPanel, command.StationNumber);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.lightShunting:
                    {
                        LightShunting newelement = new LightShunting(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject, (data as IScrollElement).CurrencyScroll);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.lightTrain:
                    {
                        LightTrain newelement = new LightTrain(data.StationNumber, data.Figure, data.StrokeThickness, (data as LightTrain).IsInput, data.NameObject, (data as IScrollElement).CurrencyScroll);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.ramka:
                    {
                        RamkaStation newelement = new RamkaStation(data.StationNumber, data.Figure, data.StrokeThickness, data.NameObject);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.namestation:
                    {
                        NameStation newelement = new NameStation(data.StationNumber, data.Figure, data.NameObject, data.StrokeThickness, (data as IText).Text);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.texthelp:
                    {
                        TextHelp newelement = new TextHelp(data.Figure, data.NameObject, data.StrokeThickness, (data as IText).Text);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.line:
                    {
                        LineHelp line = data as LineHelp;
                        LineHelp newelement = new LineHelp(data.StationNumber, data.StationNumberRight, data.NameObject, data.Figure, line.StrokeCurrent, line.NameViewColor.NameColor, line.NameViewColor.R, line.NameViewColor.G, line.NameViewColor.B, line.IsFillInside);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                        foreach (double value in line.StrokeDashArray)
                            newelement.StrokeDashArray.Add(value);
                    }
                    break;
                case ViewElement.otrezok:
                    {
                        LinePeregon peregon = data as LinePeregon;
                        LinePeregon newelement = new LinePeregon(data.StationNumber, data.Figure, data.NameObject);
                        newelement.StationNumberRight = peregon.StationNumberRight;
                        newelement.Graniza = peregon.Graniza;
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.time:
                    {
                        TimeForm newelement = new TimeForm(data.Figure, data.StrokeThickness, (data as IText).Text);
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
                case ViewElement.numbertrain:
                    {
                        NumberTrain numberTrain = data as NumberTrain;
                        NumberTrain newelement = new NumberTrain(data.StationNumber, data.Figure, data.StrokeThickness, numberTrain.RotateText, data.NameObject);
                        newelement.StationNumberRight = numberTrain.StationNumberRight;
                        newelement.Graniza = numberTrain.Graniza;
                        SetSettingsObject(newelement, data, collection, isSaveId);
                    }
                    break;
            }
        }

        /// <summary>
        /// формирование геометрии объектов перед сохранением
        /// </summary>
        /// <param name="figures">перечень фигур</param>
        /// <returns></returns>
        private List<Figure> GetFiguresSave(PathFigureCollection figures, ViewSave view)
        {
            List<Figure> _figures = new List<Figure>();

            foreach (PathFigure geo in figures)
            {
                Figure newfigure = new Figure()
                {
                    StartPoint = new Point(AnalisSaveOpen(geo.StartPoint.X, view, 1/System.Windows.SystemParameters.CaretWidth),  
                                           AnalisSaveOpen(geo.StartPoint.Y, view, 1/System.Windows.SystemParameters.CaretWidth))
                };
                //
                foreach (PathSegment seg in geo.Segments)
                {
                    //сегмент линия
                    LineSegment lin = seg as LineSegment;
                    if (lin != null)
                    {
                        newfigure.Segments.Add(new Segment()
                        {
                            Point = new Point(AnalisSaveOpen(lin.Point.X, view, 1/System.Windows.SystemParameters.CaretWidth),  
                                           AnalisSaveOpen(lin.Point.Y, view, 1/System.Windows.SystemParameters.CaretWidth))
                        });
                        continue;
                    }
                    //сегмент арка
                    ArcSegment arc = seg as ArcSegment;
                    if (arc != null)
                    {
                        newfigure.Segments.Add(new Segment()
                        {
                            Point = new Point
                            (AnalisSaveOpen(arc.Point.X, view, 1 / System.Windows.SystemParameters.CaretWidth),
                                           AnalisSaveOpen(arc.Point.Y, view, 1 / System.Windows.SystemParameters.CaretWidth)),
                            RadiusX = AnalisSaveOpen(arc.Size.Width, view, 1 / System.Windows.SystemParameters.CaretWidth),
                            RadiusY = AnalisSaveOpen(arc.Size.Height, view, 1 / System.Windows.SystemParameters.CaretWidth)
                        });
                        continue;
                    }
                }
                _figures.Add(newfigure);
            }
            //
            return _figures;
        }

        private void FullTransformGroup(TransformGroup TransformHistory, TransformGroup Transform)
        {
            TransformHistory.Children.Clear();
            //
            foreach (Transform trans in Transform.Children)
            {
                if (trans is TranslateTransform)
                    TransformHistory.Children.Add(new TranslateTransform() { X = (trans as TranslateTransform).X, Y = (trans as TranslateTransform).Y });
                //
                if (trans is ScaleTransform)
                    TransformHistory.Children.Add(new ScaleTransform()
                    {
                        ScaleX = (trans as ScaleTransform).ScaleX,
                        ScaleY = (trans as ScaleTransform).ScaleY,
                        CenterX = (trans as ScaleTransform).CenterX,
                        CenterY = (trans as ScaleTransform).CenterY
                    });
            }
        }

        private List<IGraficObejct> CopyAllElement(UIElementCollection history)
        {
            List<IGraficObejct> newcollection = new List<IGraficObejct>();
            if (history.Count > 0)
            {
                foreach (UIElement el in history)
                    FullGrafic(newcollection, (IGraficObejct)el, true);
            }
            return newcollection;
        }

        public void OprtaObejctMove(ToolsGraphics toolspanel, Point newposition, List<IGraficObejct> activeel, bool ismouse)
        {
            if (toolspanel.Orta)
            {
                _deltaXMouse += Math.Abs(newposition.X - CursorX);
                _deltaYMouse += Math.Abs(newposition.Y - CursorY);
                //
                if (_deltaXMouse > _deltaYMouse)
                    ActiveObejctModify(newposition.X - CursorX, 0, activeel, ismouse);
                else ActiveObejctModify(0, newposition.Y - CursorY, activeel, ismouse);
            }
            else
                ActiveObejctModify(newposition.X - OperationsGrafic.CursorX, newposition.Y - CursorY, activeel, ismouse);
        }

        /// <summary>
        /// выделение нескольких элементов рамкой
        /// </summary>
        /// <param name="ramka">рамка выделения</param>
        public void ActiveElements(Rectangle ramka, Dictionary<int, IGraficObejct> currentDrawElement, List<IGraficObejct> activeel)
        {
            SelectOperation.SelectRamka(currentDrawElement, ramka, _widthramka, activeel);
            CountSelectElement(activeel.Count);
            ShowGeometryElement(activeel);
            if (SelectObject != null)
                SelectObject(activeel);
        }

        public void NewPointAddLineHelp(IGraficObejct graf, Canvas canvas, ToolsGraphics toolspanel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            if (graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Count > 0)
            {
                if (!toolspanel.Orta)
                    graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Add(new LineSegment() { Point = new Point(CursorX, CursorY) });
                else
                {
                    LineSegment linesegpoint = graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments[graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Count - 1] as LineSegment;
                    graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Add(new LineSegment() { Point = new Point(linesegpoint.Point.X, linesegpoint.Point.Y) });
                }
                //измегяем историю 
                AddHistory(currentDrawElement);
            }
            else
            {
                //   graf.StrokeThickness *= _currentscroll;
                graf.Id = currentFreeId;
                canvas.Children.Add((UIElement)graf);
                currentDrawElement.Add(graf.Id, graf);
                currentFreeId++;
                if (AddObject != null)
                    AddObject(new List<IGraficObejct> { graf });
                //
                graf.Figure.Figures[graf.Figure.Figures.Count - 1].StartPoint = new Point(CursorX, CursorY);
                graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Add(new LineSegment() { Point = new Point(CursorX, CursorY) });
            }
        }

        public void ClearHistory()
        {
            history.Clear();
            history = new List<History>();
            history.Add(new History());
            LastCountLevelHistory = history.Count;
        }

        /// <summary>
        /// проверяем попали ли в какой - нибудь активный элемент 
        /// </summary>
        /// <param name="cliicpoint">точка нажатия мыши</param>
        public void ActiveElementOne(Window window, Point pointclick, Canvas drawcanvas, ToolsManagement toolspanelcommand, ToolsGraphics toolspanel, List<IGraficObejct> activeel, 
                                     Dictionary<int, IGraficObejct> currentDrawElement)
        {
            EmptyDeltaX();
            EmptyHelpStatus();
            //
            drawcanvas.Children.Remove(framePoint);
            switch (toolspanelcommand.CurrentDraw)
            {
                case ViewElement.area_message:
                    CreateRamka(MainColors.AreaMessage, drawcanvas, activeel, toolspanel);
                    break;
                case ViewElement.area_station:
                    CreateRamka(MainColors.AreaStation, drawcanvas, activeel, toolspanel);
                    break;
                case ViewElement.tablenumbertrain:
                    CreateRamka(MainColors.TableTrainRamka, drawcanvas, activeel, toolspanel);
                    break;
                case ViewElement.tableautopilot:
                    CreateRamka(MainColors.TableAutoPilot, drawcanvas, activeel, toolspanel);
                    break;
                default:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            SelectOperation.SelectObject(currentDrawElement, pointclick, activeel);
                            CountSelectElement(activeel.Count);
                            if (SelectObject != null)
                            {
                                ShowGeometryElement(activeel);
                                SelectObject(activeel);
                                drawcanvas.Focus();
                            }
                        }
                        else
                            CreateRamka(MainColors.SelectRamka, drawcanvas, activeel, toolspanel);
                    }
                    break;
            }
            //
            ZeroKoordinate();
        }

        /// <summary>
        /// создаем рамку выделения
        /// </summary>
        /// <param name="colorramka"></param>
        private void CreateRamka(Brush colorramka, Canvas drawcanvas, List<IGraficObejct> activeel, ToolsGraphics toolspanel)
        {
            DefaultColor(activeel, drawcanvas, toolspanel);
            frameRec = new Rectangle() { Stroke = colorramka, StrokeThickness = savescroll, Width = 0, Height = 0 };
            frameRec.Margin = new Thickness(CursorX, CursorY, 0, 0);
            _heightramka = 0;
            _widthramka = 0;
            drawcanvas.Children.Add(frameRec);
        }

        /// <summary>
        /// устанавливаем цет по умочанию для всех и очищаем коллекцию активных элементов
        /// </summary>
        public void DefaultColor(List<IGraficObejct> activeel, Canvas drawCanvas, ToolsGraphics toolspanel)
        {
            foreach (IGraficObejct el in activeel)
                el.DefaultColor();
            activeel.Clear();
            toolspanel.SelectNotTable();
            drawCanvas.Children.Remove(framePoint);
        }

        public void ScrollElement(double Scroll, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            foreach (IGraficObejct el in activeel)
            {
                if (el is IScrollElement)
                {
                    IScrollElement scroll = el as IScrollElement;
                    Point centre = scroll.CentreFigure();
                    ScaleTransform transform = new ScaleTransform(Scroll, Scroll, centre.X, centre.Y);
                    scroll.ScrollFigure(transform, Scroll);
                }
            }
            //
            ShowGeometryElement(activeel);
            AddHistory(currentDrawElement);
        }

        public void SelectObjectNew(List<int> id, PointFigure figure, List<IGraficObejct> activeel, Canvas drawcanvas, Window window, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            foreach (IGraficObejct graf in activeel)
            {
                graf.DefaultColor();
            }
            //
            activeel.Clear();
            drawcanvas.Children.Remove(OperationsGrafic.framePoint);
            //
            try
            {
                foreach (int el in id)
                {
                    activeel.Add(currentDrawElement[el]);
                    if (id.Count == 1 && figure != null && currentDrawElement[el] is IShowSettings)
                    {
                        (currentDrawElement[el] as IShowSettings).Index = (currentDrawElement[el] as IShowSettings).FindIndexLine(figure.IdFigure, figure.IdSegment);
                        currentDrawElement[el].SelectStroke = true;
                        drawcanvas.Children.Add(OperationsGrafic.framePoint);
                    }
                    else
                        currentDrawElement[el].Selectobject = true;
                    currentDrawElement[el].SelectColor();
                }
                EmptyHelpStatus();
                CountSelectElement(activeel.Count);
                ShowGeometryElement(activeel);
                //
                //if (SelectObject != null && figure == null)
                //    SelectObject(_activeel);
                //window.Focus();
            }
            catch { }
        }

        public void DeleteActiveObejct(List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement, Canvas drawcanvas)
        {
            if (activeel.Count > 1 || (activeel.Count == 1 && activeel[activeel.Count - 1].Selectobject))
            {
                foreach (IGraficObejct el in activeel)
                {
                    drawcanvas.Children.Remove((UIElement)el);
                    currentDrawElement.Remove(el.Id);
                    if (el is IText)
                        drawcanvas.Children.Remove((el as IText).Text);
                }
                //
                if (RemoveObject != null)
                    RemoveObject(activeel);
                //
            }
            else
            {
                if (activeel.Count == 1 && activeel[activeel.Count - 1].SelectStroke)
                {
                    if (activeel[activeel.Count - 1] is IDeleteElement)
                    {
                        IDeleteElement el = activeel[activeel.Count - 1] as IDeleteElement;
                        el.DeletePoint();
                        activeel[activeel.Count - 1].DefaultColor();
                        drawcanvas.Children.Remove(framePoint);
                    }
                }
            }
            //очищаем коллекцию выделенных элементов
            activeel.Clear();
            //
            if (SelectObject != null)
                SelectObject(activeel);
            //дополняем историю
            AddHistory(currentDrawElement);
        }

        /// <summary>
        /// реверсивуем координаты перегона
        /// </summary>
        public void ReversePeregon(List<IGraficObejct> activeel)
        {
            LinePeregon peregon = activeel[activeel.Count - 1] as LinePeregon;
            if (peregon != null)
                peregon.Reverse();
            ShowGeometryElement(activeel);
        }

        public void HatchLine(IList<double> arrayhatch, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            LineHelp linehelp = activeel[activeel.Count - 1] as LineHelp;
            if (linehelp != null)
            {
                linehelp.StrokeDashArray.Clear();
                foreach (double step in arrayhatch)
                    linehelp.StrokeDashArray.Add(step);
            }
            AddHistory(currentDrawElement);
        }

        public void NewColorElement(NameColors NameColor, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            foreach (IGraficObejct el in activeel)
            {
                if (el is LineHelp)
                {
                    LineHelp line = el as LineHelp;
                    line.NameViewColor.NameColor = NameColor.NameColor;
                    line.NameViewColor.R = NameColor.R;
                    line.NameViewColor.G = NameColor.G;
                    line.NameViewColor.B = NameColor.B;
                    line.ColorStrokeDefult = new SolidColorBrush(Color.FromRgb(NameColor.R, NameColor.G, NameColor.B));
                }
            }
            //
            AddHistory(currentDrawElement);
        }

        public void WeightElement(double Weight, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            foreach (IGraficObejct el in activeel)
            {
                if (el is LineHelp)
                {
                    LineHelp line = el as LineHelp;
                    line.SetStroke(Weight);
                }
            }
            //
            EventUpdateGeometry(activeel);
            ShowGeometryElement(activeel);
            AddHistory(currentDrawElement);
        }

        /// <summary>
        /// поворачиваем элементы на 90 градусов
        /// </summary>
        public void RotateElement(double angle, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            foreach (IGraficObejct graf in activeel)
            {
                if (graf is IFreeAngle)
                    graf.Rotate(angle);
                else
                {
                    if (graf is IText)
                    {
                        if (angle > 0)
                            graf.Rotate(90);
                        else graf.Rotate(-90);
                    }
                    //if ((angle % 90) == 0)
                    //    graf.Rotate(angle);
                }
            }
            //
            AddHistory(currentDrawElement);
        }

        /// <summary>
        /// заливаем цветом внутри
        /// </summary>
        public void IsFillInside(List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            foreach (IGraficObejct graf in activeel)
            {
                if (graf is LineHelp)
                {
                    (graf as LineHelp).SetillInside();
                }
            }
            //
            AddHistory(currentDrawElement);
        }

        public void SizeElement(double width, double height, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            foreach (IGraficObejct el in activeel)
            {
                if (el is IResize)
                {
                    (el as IResize).Newsize(width, height);
                }
            }
            //
            ShowGeometryElement(activeel);
            AddHistory(currentDrawElement);
        }

        public void AligmentElement(double deltaX, double deltaY, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            try
            {
                foreach (IGraficObejct el in activeel)
                {
                    double delta_x = 0, delta_y = 0;
                    if (deltaX >= 1)
                        delta_x = ((Math.Round((el.Figure.Figures[0].StartPoint.X - element_select.Figure.Figures[0].StartPoint.X) / deltaX) * deltaX) + element_select.Figure.Figures[0].StartPoint.X) - el.Figure.Figures[0].StartPoint.X;
                    //
                    if (deltaY >= 1)
                        delta_y = ((Math.Round((el.Figure.Figures[0].StartPoint.Y - element_select.Figure.Figures[0].StartPoint.Y) / deltaY) * deltaY) + element_select.Figure.Figures[0].StartPoint.Y) - el.Figure.Figures[0].StartPoint.Y;
                    el.SizeFigure(delta_x, delta_y );
                }
                //
                ShowGeometryElement(activeel);
                AddHistory(currentDrawElement);
            }
            catch { }
        }

        /// <summary>
        /// Изменяем размер шрифтов
        /// </summary>
        /// <param name="fontsize"></param>
        /// <param name="activeel"></param>
        /// <param name="currentDrawElement"></param>
        public void FontSize(double fontsize, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            foreach (IGraficObejct el in activeel)
            {
                if (el is IFontSize)
                {
                    IFontSize text = el as IFontSize;
                    text.Text.FontSize = fontsize;

                }
            }
            //
            ShowGeometryElement(activeel);
            AddHistory(currentDrawElement);
        }

        /// <summary>
        /// привидение картинки к оригинальным размерам
        /// </summary>
        public void OriginalSizeCurrent(bool IsUpdate, List<IGraficObejct> activeel, TransformGroup groupTransform, Polygon lineramkadraw)
        {
            for (int i = 0; i < groupTransform.Children.Count; i++)
            {
                if (groupTransform.Children[i] is TranslateTransform)
                {
                    (groupTransform.Children[i] as TranslateTransform).X = 0;
                    (groupTransform.Children[i] as TranslateTransform).Y = 0;
                }
                //
                if (groupTransform.Children[i] is ScaleTransform)
                {
                    groupTransform.Children.RemoveAt(i);
                    i--;
                }
            }
            //если произошло изменение к оригинальным размерам
            if (IsUpdate)
            {
                CurrentScroll = 1;
                _po.X = 0; _po.Y = 0;
                lineramkadraw.StrokeDashArray = new DoubleCollection() { 20 };
                EventUpdateGeometry(activeel);
            }
        }

        public void SetPathPicture(string path, List<IGraficObejct> activeel, ViewArea viewArea)
        {
            foreach (IGraficObejct el in activeel)
            {
                if (el is Area)
                {
                    Area area_picture = el as Area;
                    if (area_picture.View == Common.Enums.ViewArea.area_picture || area_picture.View == ViewArea.webBrowser)
                    {
                        area_picture.Path = path;
                        if (area_picture.View == Common.Enums.ViewArea.area_picture)
                            area_picture.SetFillColor(path);
                    }
                }
            }
        }

        /// <summary>
        /// обнуление координат перемещения
        /// </summary>
        public void ZeroKoordinate()
        {
            _deltaXKey = 0;
            _deltaYKey = 0;
            infokoordinate.Content = string.Format("X := {0}; Y := {1}", _deltaXKey.ToString(), _deltaYKey.ToString());
        }

        public void MovePointElement(Point newposition, IGraficObejct graf, ToolsGraphics toolspanel, Canvas canvas)
        {
            try
            {
                if (graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Count > 0)
                {
                    if (!toolspanel.Orta)
                    {
                        LineSegment linenew = graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments[graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Count - 1] as LineSegment;
                        linenew.Point = new Point(newposition.X, newposition.Y);
                    }
                    else
                    {
                        if (graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Count > 1)
                        {
                            Point p = graf.Figure.Figures[graf.Figure.Figures.Count - 1].StartPoint;

                            LineSegment lineminus1 = graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments[graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Count - 1] as LineSegment;
                            LineSegment lineminus2 = graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments[graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Count - 2] as LineSegment;
                            //
                            if (Math.Abs(newposition.X - lineminus2.Point.X) > Math.Abs(newposition.Y - lineminus2.Point.Y))
                                lineminus1.Point = new Point(newposition.X, lineminus2.Point.Y);
                            else
                                lineminus1.Point = new Point(lineminus2.Point.X, newposition.Y);
                        }
                        else
                        {
                            LineSegment lineminus1 = graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments[graf.Figure.Figures[graf.Figure.Figures.Count - 1].Segments.Count - 1] as LineSegment;
                            //
                            if (Math.Abs(newposition.X - graf.Figure.Figures[graf.Figure.Figures.Count - 1].StartPoint.X) > Math.Abs(newposition.Y - graf.Figure.Figures[graf.Figure.Figures.Count - 1].StartPoint.Y))
                                lineminus1.Point = new Point(newposition.X, graf.Figure.Figures[graf.Figure.Figures.Count - 1].StartPoint.Y);
                            else
                                lineminus1.Point = new Point(graf.Figure.Figures[graf.Figure.Figures.Count - 1].StartPoint.X, newposition.Y);
                        }
                    }
                }
            }
            catch
            {
                CancelDrawLine(canvas, toolspanel, graf);
            }
        }

        public void CancelDrawRamkaStation(Canvas drawcanvas, ToolsGraphics toolspanel, IGraficObejct framestation, Polygon lineramkadraw, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            toolspanel.CurrentDraw = ViewElement.none;
            toolspanel.EnabledForm();
            lineramkadraw.Points.RemoveAt(lineramkadraw.Points.Count - 1);
            if (lineramkadraw.Points.Count >= 3)
            {
                if (lineramkadraw.Points.Count == 3)
                    lineramkadraw.Points.Add(new Point(lineramkadraw.Points[0].X, lineramkadraw.Points[lineramkadraw.Points.Count - 1].Y));
                else
                {
                    if (lineramkadraw.Points[lineramkadraw.Points.Count - 1].Y == lineramkadraw.Points[lineramkadraw.Points.Count - 2].Y)
                        lineramkadraw.Points[lineramkadraw.Points.Count - 1] = new Point(lineramkadraw.Points[0].X, lineramkadraw.Points[lineramkadraw.Points.Count - 1].Y);
                    else
                        lineramkadraw.Points[lineramkadraw.Points.Count - 1] = new Point(lineramkadraw.Points[lineramkadraw.Points.Count - 1].X, lineramkadraw.Points[0].Y);
                }
                //
                framestation = new RamkaStation(1, toolspanel.CurrentNumberStation, lineramkadraw.Points);
                drawcanvas.Children.Remove(lineramkadraw);
                lineramkadraw.Points.Clear();
                //
                framestation.Id = currentFreeId;
                drawcanvas.Children.Add((UIElement)framestation);
                currentDrawElement.Add(framestation.Id, framestation);
                //
                framestation.ZIndex = -1;
                Canvas.SetZIndex((UIElement)framestation, -1);
                currentFreeId++;
                //
                if (AddObject != null)
                    AddObject(new List<IGraficObejct> { framestation });
                //заносим объект в историю
                AddHistory(currentDrawElement);
            }
            else
            {
                drawcanvas.Children.Remove(lineramkadraw);
                lineramkadraw.Points.Clear();
            }
            //
        }

        public void CancelDrawLine(Canvas canvas, ToolsGraphics toolspanel, IGraficObejct line)
        {
            toolspanel.CurrentDraw = ViewElement.none;
            if (line is LineHelp)
                toolspanel.EnabledForm();
            line.Figure.Figures[line.Figure.Figures.Count - 1].Segments.RemoveAt(line.Figure.Figures[line.Figure.Figures.Count - 1].Segments.Count - 1);
            if (line.Figure.Figures[line.Figure.Figures.Count - 1].Segments.Count < 1)
            {
                canvas.Children.Remove((UIElement)line);
                if (RemoveObject != null)
                    RemoveObject(new List<IGraficObejct> { line });
            }
        }

        /// <summary>
        /// перемещение прямоугольника рамки выделения объектов
        /// </summary>
        /// <param name="newposition">новая точка кооринаты курсора</param>
        public void RectangleRamkaModify(Point newposition)
        {
            if (frameRec != null)
            {
                //для ширины
                _widthramka += (newposition.X - CursorX);
                frameRec.Width = Math.Abs(_widthramka);
                if (_widthramka < 0)
                    frameRec.Margin = new Thickness(newposition.X, frameRec.Margin.Top, 0, 0);
                else
                    frameRec.Margin = new Thickness(newposition.X - frameRec.Width, frameRec.Margin.Top, 0, 0);
                //для высоты
                _heightramka += (newposition.Y - CursorY);
                frameRec.Height = Math.Abs(_heightramka);
                if (_heightramka < 0)
                    frameRec.Margin = new Thickness(frameRec.Margin.Left, newposition.Y, 0, 0);
                else
                    frameRec.Margin = new Thickness(frameRec.Margin.Left, newposition.Y - frameRec.Height, 0, 0);
                //
            }
        }


        public void GetNameSelectObejct(Dictionary<int, IGraficObejct> currentDrawElement, Point newposition)
        {
            List<IGraficObejct> info = SelectOperation.FindElement(currentDrawElement, newposition);
            if (info != null)
                NameObjectement.Content = info[info.Count - 1].NameObject;
            mousekoordinate.Content = string.Format("({0:F2}, {1:F2})", newposition.X / System.Windows.SystemParameters.CaretWidth, newposition.Y / System.Windows.SystemParameters.CaretWidth);
        }

        /// <summary>
        /// показываем количество выделенных элементов
        /// </summary>
        /// <param name="count">количество выделенных элементов</param>
        public void CountSelectElement(int count)
        {
            countselectelement.Content = string.Format("Количество выделенных элементов: {0}", count.ToString());
        }

        public void EmptyHelpStatus()
        {
            pointinsert.Content = string.Empty;
            elementsize.Content = string.Empty;
        }

        /// <summary>
        /// показываем геометрию объекта
        /// </summary>
        /// <param name="count">количество выделенных элементов</param>
        public void ShowGeometryElement(List<IGraficObejct> elemnets)
        {
            //если выделенна группа элементов
            if (elemnets.Count > 1)
            {
                bool X = true;
                bool Y = true;
                bool Width = true;
                bool Height = true;
                EqualElement(ref X, ref Y, ref Height, ref Width, elemnets);
                if (X && Y)
                    pointinsert.Content = string.Format("Точка вставки - ({0};{1})", Math.Round((elemnets[elemnets.Count - 1].PointInsert().X)), Math.Round((elemnets[elemnets.Count - 1].PointInsert().Y)));
                if (X && !Y)
                    pointinsert.Content = string.Format("Точка вставки - ({0};{1})", Math.Round((elemnets[elemnets.Count - 1].PointInsert().X)), "-");
                if (!X && Y)
                    pointinsert.Content = string.Format("Точка вставки - ({0};{1})", "-", Math.Round((elemnets[elemnets.Count - 1].PointInsert().Y)));
                if (Height && Width)
                    elementsize.Content = string.Format("Ширина - {0}, высота - {1}", elemnets[elemnets.Count - 1].SizeGeometry().Widht, elemnets[elemnets.Count - 1].SizeGeometry().Height);
                if (!Height && Width)
                    elementsize.Content = string.Format("Ширина - {0}", elemnets[elemnets.Count - 1].SizeGeometry().Widht);
                if (Height && !Width)
                    elementsize.Content = string.Format("Высота - {0}", elemnets[elemnets.Count - 1].SizeGeometry().Widht);
            }
            //
            if (elemnets.Count == 1)
            {
                double Xinsert = Math.Round((elemnets[elemnets.Count - 1].PointInsert().X), 2);
                double Yinsert = Math.Round((elemnets[elemnets.Count - 1].PointInsert().Y), 2);
                //главный путь
                if (elemnets[elemnets.Count - 1] is Traintrack)
                {
                    SizeElement size = elemnets[elemnets.Count - 1].SizeGeometry();
                    pointinsert.Content = string.Format("Главный путь: точка вставки - ({0}; {1}), толщина - {2}", Xinsert, Yinsert, Math.Round(elemnets[elemnets.Count - 1].StrokeThickness/savescroll, 0));
                    elementsize.Content = string.Format("Ширина - {0}, высота - {1}", size.Widht, size.Height);
                    return;
                }
                //кнопка управления
                if (elemnets[elemnets.Count - 1] is ButtonCommand)
                {
                    SizeElement size = elemnets[elemnets.Count - 1].SizeGeometry();
                    pointinsert.Content = string.Format("Кнопка управления: точка вставки - ({0}; {1})", Xinsert, Yinsert);
                    elementsize.Content = string.Format("Ширина - {0}, высота - {1}", size.Widht, size.Height);
                    return;
                }
                //эскиз таблицы поездов
                if (elemnets[elemnets.Count - 1] is Area)
                {
                    pointinsert.Content = string.Format("Таблица поездов: точка вставки - ({0}; {1})", Xinsert, Yinsert);
                    elementsize.Content = string.Format("Ширина - {0}, высота - {1}", elemnets[elemnets.Count - 1].SizeGeometry().Widht, elemnets[elemnets.Count - 1].SizeGeometry().Height);
                    return;
                }
                //путь примыкания
                if (elemnets[elemnets.Count - 1] is Signal)
                {
                    pointinsert.Content = string.Format("Сигнал: точка вставки - ({0}; {1}), толщина - {2}", Xinsert, Yinsert, Math.Round(elemnets[elemnets.Count - 1].StrokeThickness/savescroll, 0));
                    return;
                }
                //маштабируемые элементы
                if (elemnets[elemnets.Count - 1] is IScrollElement)
                {
                    pointinsert.Content = string.Format("{0}: точка вставки - ({1}; {2}), масштаб - {3}", (elemnets[elemnets.Count - 1] as IScrollElement).NameEl, Xinsert, Yinsert, (elemnets[elemnets.Count - 1] as IScrollElement).CurrencyScroll);
                    return;
                }
                //название станции
                if (elemnets[elemnets.Count - 1] is NameStation)
                {
                    SizeElement size = elemnets[elemnets.Count - 1].SizeGeometry();
                    pointinsert.Content = string.Format("Название станции: точка вставки - ({0}; {1})", Xinsert, Yinsert);
                    elementsize.Content = string.Format("Ширина - {0}, высота - {1}", size.Widht, size.Height);
                    return;
                }
                //справочный текст
                if (elemnets[elemnets.Count - 1] is TextHelp)
                {
                    SizeElement size = elemnets[elemnets.Count - 1].SizeGeometry();
                    pointinsert.Content = string.Format("Текст: точка вставки - ({0}; {1})", Xinsert, Yinsert);
                    elementsize.Content = string.Format("Ширина - {0}, высота - {1}", size.Widht, size.Height);
                    return;
                }
                //часы
                if (elemnets[elemnets.Count - 1] is TimeForm)
                {
                    SizeElement size = elemnets[elemnets.Count - 1].SizeGeometry();
                    pointinsert.Content = string.Format("Название станции: точка вставки - ({0}; {1})", Xinsert, Yinsert);
                    elementsize.Content = string.Format("Ширина - {0}, высота - {1}", size.Widht, size.Height);
                    return;
                }
                //вспомагательная линия
                if (elemnets[elemnets.Count - 1] is LineHelp)
                {
                    pointinsert.Content = string.Format("Вспомагательная линия: точка вставки - ({0}; {1}), толщина - {2}", Xinsert, Yinsert, Math.Round((elemnets[elemnets.Count - 1] as LineHelp).StrokeCurrent/savescroll, 0));
                    return;
                }
                //линия перегона
                if (elemnets[elemnets.Count - 1] is LinePeregon)
                {
                    pointinsert.Content = string.Format("Перегон: точка вставки - ({0}; {1})", Xinsert, Yinsert);
                    return;
                }
                //поле ввода номеров поездов
                if (elemnets[elemnets.Count - 1] is NumberTrain)
                {
                    SizeElement size = elemnets[elemnets.Count - 1].SizeGeometry();
                    pointinsert.Content = string.Format("Индикация поездов: точка вставки - ({0}; {1})", Xinsert, Yinsert);
                    elementsize.Content = string.Format("Ширина - {0}, высота - {1}", size.Widht, size.Height);
                    return;
                }
                //рамка станции
                if (elemnets[elemnets.Count - 1] is RamkaStation)
                {
                    pointinsert.Content = string.Format("Рамка станции: точка вставки - ({0}; {1})", Xinsert, Yinsert);
                }
            }
        }

        /// <summary>
        /// просматриваем повторяющиеся элементы станции
        /// </summary>
        private void EqualElement(ref bool X, ref bool Y, ref bool Height, ref bool Width, List<IGraficObejct> select)
        {
            if (select.Count > 1)
            {
                double x = Math.Round((select[0].PointInsert().X));
                double y = Math.Round((select[0].PointInsert().Y));
                double height = select[0].SizeGeometry().Height;
                double width = select[0].SizeGeometry().Widht;
                //
                for (int i = 1; i < select.Count; i++)
                {
                    if (Math.Round((select[i].PointInsert().X)) != x)
                        X = false;
                    //
                    if (Math.Round((select[i].PointInsert().Y)) != y)
                        Y = false;
                    //
                    if (select[i].SizeGeometry().Height != height || select[i].SizeGeometry().Height == 0)
                        Height = false;
                    //
                    if (select[i].SizeGeometry().Widht != width || select[i].SizeGeometry().Widht == 0)
                        Width = false;
                }
            }
        }


        /// <summary>
        /// перемещаем и изменяем выбранные объекты
        /// </summary>
        /// <param name="deltax">изменения по координате x</param>
        /// <param name="deltay">изменения по координате y</param>
        /// <param name="activeel">перечень захваченных элементов</param>
        public void ActiveObejctModify(double deltax, double deltay,  List<IGraficObejct> activeel, bool ismouse)
        {
            if ((!Keyboard.IsKeyUp(Key.LeftCtrl) || !Keyboard.IsKeyUp(Key.RightCtrl)) && ismouse)
                return;
            //
            if (activeel.Count > 0)
            {
                //уточняем delta переноса
                foreach (PathFigure geo in activeel[0].Figure.Figures)
                {
                    Point point = WorkGrafic.RoundPoint(geo.StartPoint, deltax, deltay);
                    deltax = point.X - geo.StartPoint.X;
                    deltay = point.Y - geo.StartPoint.Y;
                    break;
                }
                //
                foreach (IGraficObejct el in activeel)
                {
                    //если выбран объект 
                    if (el.Selectobject)
                        el.SizeFigure(deltax, deltay);
                    //если выбрана рамка
                    if (el.SelectStroke)
                        el.Resize(deltax, deltay);
                }
                //
                UpdateDeltaElement(deltax, deltay);
            }
            //
            ShowGeometryElement(activeel);
            update = true;
            //
            if (UpdateGeometry != null)
                UpdateGeometry(activeel);
        }

        private void UpdateDeltaElement(double deltaX, double deltaY)
        {
            _deltaXKey += deltaX;
            _deltaYKey += deltaY;
            infokoordinate.Content = string.Format("X := {0}; Y := {1}", _deltaXKey.ToString(), _deltaYKey.ToString());
        }

        /// <summary>
        /// получаем геометрию объекта
        /// </summary>
        /// <param name="Figures"></param>
        /// <returns></returns>
        private PathGeometry GetPathGeometry(List<Figure> Figures, ViewSave view)
        {
            PathGeometry geo = new PathGeometry();
            foreach (Figure figure in Figures)
            {
                PathSegmentCollection segment = new PathSegmentCollection();
                foreach (Segment seg in figure.Segments)
                {
                    if (seg.RadiusX != 0 && seg.RadiusY != 0)
                        segment.Add(new ArcSegment()
                        {
                            Point = new Point(AnalisSaveOpen(seg.Point.X, view, System.Windows.SystemParameters.CaretWidth),
                                AnalisSaveOpen(seg.Point.Y, view, System.Windows.SystemParameters.CaretWidth)),
                            Size = new Size(AnalisSaveOpen(seg.RadiusX, view, System.Windows.SystemParameters.CaretWidth),
                                AnalisSaveOpen(seg.RadiusY, view, System.Windows.SystemParameters.CaretWidth))
                        });
                    else
                        segment.Add(new LineSegment()
                        {
                            Point = new Point(AnalisSaveOpen(seg.Point.X, view, System.Windows.SystemParameters.CaretWidth),
                                AnalisSaveOpen(seg.Point.Y, view, System.Windows.SystemParameters.CaretWidth))
                        });
                }
                //
                geo.Figures.Add(new PathFigure()
                {
                    StartPoint = new Point(AnalisSaveOpen(figure.StartPoint.X, view, System.Windows.SystemParameters.CaretWidth),
                                AnalisSaveOpen(figure.StartPoint.Y, view, System.Windows.SystemParameters.CaretWidth)),
                    Segments = segment
                });

            }
            return geo;
        }

        private double AnalisSaveOpen(double x, ViewSave view, double k)
        {

            if (view == ViewSave.save)
            {
                return (x * k);
                //return Round(x * k);
            }
            else
            {
                return x;
              //  return Round(x);
            }
        }

        private void CreateNewGraficObject(IGraficObejct element, BaseSave baseSave, List<IGraficObejct> update, ViewSave view, List<IGraficObejct> activeel, Canvas drawcanvas,  Dictionary<int, IGraficObejct> currentDrawElement)
        {
            element.DefaultColor();
            element.Notes = baseSave.Notes;
            element.Id = currentFreeId; currentFreeId++;
            element.ZIndex = baseSave.ZIndex;
            element.IsVisible = baseSave.IsVisible;
            element.StationNumber = baseSave.StationNumber;
            element.StationNumberRight = baseSave.StationNumberRight;
            update.Add(element);
            //если вставляем из буфера обмена Windows
            if (view == ViewSave.copy)
            {
               element.Selectobject = true;
               element.SelectColor();
               activeel.Add(element);
            }
            //
            if (element is IScrollElement)
            {
                (element as IScrollElement).CurrencyScroll = baseSave.CurrencyScroll;
            }
            //
            drawcanvas.Children.Add((UIElement)element);
            currentDrawElement.Add(element.Id, element);
            Canvas.SetZIndex((UIElement)element, baseSave.ZIndex);
            if (element is IText)
            {
                drawcanvas.Children.Add((element as IText).Text);
                Canvas.SetZIndex((UIElement)(element as IText).Text, baseSave.ZIndex + 1);
            }
        }

        /// <summary>
        /// анализируем открытый файл
        /// </summary>
        /// <param name="Project">класс окрытого проекта</param>
        private void AnalisOpen(StrageProject Project, ViewSave view, List<IGraficObejct> activeel, Canvas drawcanvas, Dictionary<int, IGraficObejct> currentDrawElement)
        {
            List<IGraficObejct> update = new List<IGraficObejct>();
            //рисуем главные пути
            foreach (BaseSave el in Project.GraficObjects)
            {
                if (el.CurrencyScroll == 0)
                    el.CurrencyScroll = 1;
                try
                {
                    switch (el.ViewElement)
                    {
                        case ViewElement.signal:
                            {
                                Signal newelement = new Signal(el.StationNumber, GetPathGeometry(el.Figures, view), Signal.strokethickness, el.Name);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.area:
                            {
                                AreaSave area = el as AreaSave;
                                Area newelement = new Area(GetPathGeometry(area.Figures, view), Area.strokethickness, area.View, area.Path, area.Angle, area.Name, area.StationNumber, area.ZoomLevelIncrement, area.ZoomLevel);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.arrowmove:
                            {
                                ArrowMoveSave arrow = el as ArrowMoveSave;
                                ArrowMove newelement = new ArrowMove(el.StationNumber, GetPathGeometry(el.Figures, view), ArrowMove.strokethickness * el.CurrencyScroll, el.Name, el.CurrencyScroll) { StationNumberRight = arrow.StationNumberRight, Graniza = arrow.Graniza };
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.chiefroad:
                            {
                                RoadStation track = el as RoadStation;
                                Traintrack newelement = new Traintrack(el.StationNumber, GetPathGeometry(el.Figures, view), Traintrack.strokethickness,
                                                                   view == ViewSave.save ? track.Xinsert * System.Windows.SystemParameters.CaretWidth : track.Xinsert,
                                                                   view == ViewSave.save ? track.Yinsert * System.Windows.SystemParameters.CaretWidth : track.Yinsert, el.Name, track.Angle,
                                                                   view == ViewSave.save ? track.TextSize * System.Windows.SystemParameters.CaretWidth : track.TextSize, track.View);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.diagnostikCell:
                            {
                                var cell = el as RoadStation;
                                var newelement = new DiagnostikCell(el.StationNumber, GetPathGeometry(el.Figures, view), Traintrack.strokethickness, el.Name);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.buttonstation:
                            {
                                ButtonStation newelement = new ButtonStation(el.StationNumber, GetPathGeometry(el.Figures, view), ButtonStation.strokethickness * el.CurrencyScroll, el.Name, el.CurrencyScroll);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.buttoncommand:
                            {
                                ButtonCommandSave commandbutton = el as ButtonCommandSave;
                                ButtonCommand newelement = new ButtonCommand(GetPathGeometry(commandbutton.Figures, view), ButtonCommand._strokethickness,
                                                                             view == ViewSave.save ? commandbutton.Xinsert * System.Windows.SystemParameters.CaretWidth : commandbutton.Xinsert,
                                                                             view == ViewSave.save ? commandbutton.Yinsert * System.Windows.SystemParameters.CaretWidth : commandbutton.Yinsert,
                                                                             commandbutton.Name, commandbutton.HelpText, commandbutton.Parametrs, commandbutton.Angle,
                                                                             view == ViewSave.save ? commandbutton.TextSize * System.Windows.SystemParameters.CaretWidth : commandbutton.TextSize,
                                                                             commandbutton.ViewCommand, commandbutton.ViewPanel, commandbutton.StationNumber);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.disconnectors:
                            {
                                Disconnectors newelement = new Disconnectors(el.StationNumber,el.StationNumberRight, GetPathGeometry(el.Figures, view), Move.strokethickness * el.CurrencyScroll, el.Name, el.TypeDisconnector, el.CurrencyScroll);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.kgu:
                            {
                                KGU newelement = new KGU(el.StationNumber, GetPathGeometry(el.Figures, view), KGU.strokethickness * el.CurrencyScroll, el.Name, el.CurrencyScroll);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.ktcm:
                            {
                                KTCM newelement = new KTCM(el.StationNumber, GetPathGeometry(el.Figures, view), KTCM.strokethickness * el.CurrencyScroll, el.Name, el.CurrencyScroll);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.lightShunting:
                            {
                                LightShunting newelement = new LightShunting(el.StationNumber, GetPathGeometry(el.Figures, view), LightShunting.strokethickness * el.CurrencyScroll, el.Name, el.CurrencyScroll);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.lightTrain:
                            {
                                LightTrain newelement = new LightTrain(el.StationNumber, GetPathGeometry(el.Figures, view), LightTrain.strokethickness * el.CurrencyScroll, (el as LightTrainSave).IsInput, el.Name, el.CurrencyScroll);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.line:
                            {
                                LineHelpSave linehp = el as LineHelpSave;
                                if (linehp.WeightStroke == 0)
                                    linehp.WeightStroke = 1;
                                if (SCADA.Desiner.WorkForms.NewColorForm.Contains(linehp.NameColor))
                                    WorkGrafic.NameColors.Add(new NameColors() { NameColor = linehp.NameColor, R = linehp.R, G = linehp.G, B = linehp.B });
                                LineHelp newelement = new LineHelp(el.StationNumber, el.StationNumberRight, GetPathGeometry(el.Figures, view), linehp.WeightStroke, linehp.NameColor, linehp.R, linehp.G, linehp.B, linehp.IsFillInside) { NameObject = el.Name};
                                foreach (double step in linehp.StrokeDashArray)
                                    newelement.StrokeDashArray.Add(step);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.move:
                            {
                                Move newelement = new Move(el.StationNumber, GetPathGeometry(el.Figures, view), Move.strokethickness * el.CurrencyScroll, el.Name, el.CurrencyScroll);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.namestation:
                            {
                                NameStationSave textform = el as NameStationSave;
                                NameStation newelement = new NameStation(el.StationNumber, GetPathGeometry(el.Figures, view), el.Name, NameStation.strokethickness,
                                                                   new TextBlock()
                                                                            {
                                                                             MaxWidth = (view == ViewSave.save ? textform.Width * System.Windows.SystemParameters.CaretWidth : textform.Width),
                                                                             MaxHeight = (view == ViewSave.save ? textform.Height * System.Windows.SystemParameters.CaretWidth : textform.Height),
                                                                             Margin = new Thickness((view == ViewSave.save ? textform.Left * System.Windows.SystemParameters.CaretWidth : textform.Left),
                                                                                                    (view == ViewSave.save ? textform.Top * System.Windows.SystemParameters.CaretWidth : textform.Top),0, 0),
                                                                            FontSize = (view == ViewSave.save ? textform.FontSize * System.Windows.SystemParameters.CaretWidth : textform.FontSize),
                                                                            RenderTransform = new RotateTransform(textform.Angle)});
                                newelement.RotateText = textform.Angle;
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.otrezok:
                            {
                                LinePeregonSave otrezok = el as LinePeregonSave;
                                LinePeregon newelement = new LinePeregon(el.StationNumber, GetPathGeometry(el.Figures, view), el.Name) {StationNumberRight = otrezok.StationNumberRight,Graniza = otrezok.Graniza};
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.ramka:
                            {
                                RamkaStation newelement = new RamkaStation(el.StationNumber, GetPathGeometry(el.Figures, view), RamkaStation.strokethickness, el.Name);
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.texthelp:
                            {
                                TextHelpSave texthelp = el as TextHelpSave;
                                TextHelp newelement = new TextHelp(GetPathGeometry(el.Figures, view), texthelp.Text, TextHelp.strokethickness,
                                                                  new TextBlock()
                                                                  {
                                                                      MaxWidth = (view == ViewSave.save ? texthelp.Width * System.Windows.SystemParameters.CaretWidth : texthelp.Width),
                                                                      MaxHeight = (view == ViewSave.save ? texthelp.Height * System.Windows.SystemParameters.CaretWidth : texthelp.Height),
                                                                      Margin = new Thickness((view == ViewSave.save ? texthelp.Left * System.Windows.SystemParameters.CaretWidth : texthelp.Left),
                                                                                            (view == ViewSave.save ? texthelp.Top * System.Windows.SystemParameters.CaretWidth : texthelp.Top), 0, 0),
                                                                      FontSize = (view == ViewSave.save ? texthelp.FontSize * System.Windows.SystemParameters.CaretWidth : texthelp.FontSize),
                                                                      RenderTransform = new RotateTransform(texthelp.Angle)
                                                                  }) {StationNumber = el.StationNumber };
                                newelement.RotateText = texthelp.Angle;
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.time:
                            {
                                TimeSave timer = el as TimeSave;
                                TimeForm newelement = new TimeForm(GetPathGeometry(el.Figures, view), NameStation.strokethickness,
                                                                   new TextBlock()
                                                                   {
                                                                       MaxWidth = (view == ViewSave.save ? timer.Width * System.Windows.SystemParameters.CaretWidth : timer.Width),
                                                                       MaxHeight = (view == ViewSave.save ? timer.Height * System.Windows.SystemParameters.CaretWidth : timer.Height),
                                                                       Margin = new Thickness((view == ViewSave.save ? timer.Left * System.Windows.SystemParameters.CaretWidth : timer.Left),
                                                                                             (view == ViewSave.save ? timer.Top * System.Windows.SystemParameters.CaretWidth : timer.Top), 0, 0),
                                                                       FontSize = (view == ViewSave.save ? timer.FontSize * System.Windows.SystemParameters.CaretWidth : timer.FontSize),
                                                                       RenderTransform = new RotateTransform(timer.Angle)
                                                                   });
                                newelement.RotateText = timer.Angle;
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                        case ViewElement.numbertrain:
                            {
                                NumberTrainSave number = el as NumberTrainSave;
                                NumberTrain newelement = new NumberTrain(el.StationNumber, GetPathGeometry(el.Figures, view), NumberTrain.strokethickness, number.RotateText, el.Name) { StationNumberRight = number.StationNumberRight, Graniza = number.Graniza };
                                CreateNewGraficObject(newelement, el, update, view, activeel, drawcanvas, currentDrawElement);
                            }
                            break;
                    }
                }
                catch (Exception error) { MessageBox.Show(error.Message); }
            }
            //
            if (view != ViewSave.copy)
            {
                //обновляем список объектов
                EventUpdateObject(update);
            }
        }

        //отрисовываем открытый проект
        public void DrawOpen(StrageProject Project, Canvas drawcanvas, List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentElementDraw, 
                             ToolsGraphics toolpanel, ToolsManagement toolcommand,TransformGroup groupTransform, Polygon linedraw)
        {
            try
            {
                //возвращаем толщину к исходному состоянию
                ScrollStrokeThickess.SetScrollStrokeThickess(savescroll, 1);
                //получаем новый коэффициент масштаба
                if (Project.Scroll <= 0)
                    Project.Scroll = 1;
                savescroll = Project.Scroll * System.Windows.SystemParameters.CaretWidth;
                //находим новую толщину
                ScrollStrokeThickess.SetScrollStrokeThickess(1, savescroll);
            }
            catch { }
            //удаляем текущее состояние
            DeleteCurrentState(drawcanvas, toolpanel, toolcommand, activeel, currentElementDraw);
            //приводим к оригинальным координатам
            OriginalSizeCurrent(true, activeel, groupTransform, linedraw);
            ////удаляем историю
            ClearHistory();
            //отрисовываем проект
            AnalisOpen(Project, ViewSave.save, activeel, drawcanvas, currentElementDraw);
            //обновляем историю
            AddHistory(currentElementDraw);
            LastCountLevelHistory = history.Count;
        }

        /// <summary>
        /// вставка объектов из буфера обмена Windows
        /// </summary>
        public void Paste(List<IGraficObejct> activeel, Dictionary<int, IGraficObejct> currentDrawElement, Canvas drawcanvas, ToolsGraphics toolpanel)
        {
            if (Clipboard.GetDataObject().GetDataPresent(typeof(StrageProject)))
            {
                try
                {
                    StrageProject copygraf = (StrageProject)Clipboard.GetDataObject().GetData(typeof(StrageProject));
                    if (copygraf != null)
                    {
                        DefaultColor(activeel, drawcanvas, toolpanel);
                        //отрисовываем проект
                        AnalisOpen(copygraf, ViewSave.copy, activeel, drawcanvas, currentDrawElement);
                        //обновляем историю
                        AddHistory(currentDrawElement);
                        //
                        if (AddObject != null)
                        {
                            AddObject(activeel);
                            //отправляем выделенные элементы
                            if (SelectObject != null)
                                SelectObject(activeel);
                        }
                    }
                }
                catch (Exception error) { MessageBox.Show(error.Message); }
            }
        }

        /// <summary>
        /// копирование выделенных объектов
        /// </summary>
        public void Copy(Canvas DrawCanvas, List<IGraficObejct> activeel)
        {
            DrawCanvas.Children.Remove(framePoint);
            List<IGraficObejct> copyelement = new List<IGraficObejct>();
            if (activeel.Count > 0)
            {
                try
                {
                    if (activeel.Count == 1)
                        element_select = activeel[activeel.Count - 1];
                }
                catch { }
                //
                foreach (IGraficObejct graf in activeel)
                    FullGrafic(copyelement, graf, false);
                //добавляем в буфер обмена элементы
                Clipboard.SetDataObject(SaveAnalis(ViewSave.copy, copyelement));
            }
        }

        private void SaveBaseElement(IGraficObejct el, ViewSave view, StrageProject Project)
        {
            BaseSave newelement = new BaseSave() { Name = el.NameObject, StationNumber = el.StationNumber, Figures =  GetFiguresSave(el.Figure.Figures, view), 
                                                  ViewElement = el.ViewElement, ZIndex = el.ZIndex};
            Project.GraficObjects.Add(newelement);
        }


        private void AddBaseSaveObject(ViewSave view, IGraficObejct graficObject, BaseSave newelement, StrageProject Project)
        {
            SetBaseSaveObject(view, graficObject, newelement);
            Project.GraficObjects.Add(newelement);
        }

        private void SetBaseSaveObject(ViewSave view, IGraficObejct graficObject, BaseSave newelement)
        {
            newelement.Name = graficObject.NameObject;
            newelement.Notes = graficObject.Notes;
            newelement.IsVisible = graficObject.IsVisible;
            newelement.StationNumber = graficObject.StationNumber;
            newelement.StationNumberRight = graficObject.StationNumberRight;
            newelement.ViewElement = graficObject.ViewElement;
            newelement.ZIndex = graficObject.ZIndex;
            if (graficObject is IScrollElement)
                newelement.CurrencyScroll = (graficObject as IScrollElement).CurrencyScroll;
            newelement.Figures = GetFiguresSave(graficObject.Figure.Figures, view);
        }

        /// <summary>
        /// анализ объектов перед сохранением
        /// </summary>
        public StrageProject SaveAnalis(ViewSave view, List<IGraficObejct> Elements)
        {
            //количество нарисованных часов
           // int count_time = 0;
            StrageProject Project = new StrageProject();
            //если необходимо сохранять
            if (view == ViewSave.save)
            {
                Project.Scroll = savescroll / System.Windows.SystemParameters.CaretWidth;
                //
                if (Project.Scroll <= 0)
                    Project.Scroll = 1;
            }
            //
            foreach (IGraficObejct el in Elements)
            {
                switch (el.ViewElement)
                {
                    case ViewElement.signal:
                        {
                            BaseSave newelement = new BaseSave();
                            AddBaseSaveObject(view, el, newelement, Project);
                        }
                        break;
                    case ViewElement.area:
                        {
                            Area area = el as Area;
                            AreaSave newelement = new AreaSave() { Angle = area.Angle, Path = area.Path, View = area.View, ZoomLevel = area.ZoomLevel, ZoomLevelIncrement = area.ZoomLevelIncrement };
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.arrowmove:
                        {
                            ArrowMove arrow = el as ArrowMove;
                            ArrowMoveSave newelement = new ArrowMoveSave() { Graniza = arrow.Graniza };
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.buttonstation:
                        {
                            BaseSave newelement = new BaseSave();
                            AddBaseSaveObject(view, el, newelement, Project);
                        }
                        break;
                    case ViewElement.buttoncommand:
                        {
                            ButtonCommand command = el as ButtonCommand;
                            ButtonCommandSave newelement = new ButtonCommandSave() { ViewCommand = command.ViewCommand, ViewPanel = command.ViewPanel, HelpText = command.HelpText, Parametrs = command.Parameters};
                            newelement.Angle = command.RotateText;
                            newelement.TextSize = (view == ViewSave.save ? command.Text.FontSize / System.Windows.SystemParameters.CaretWidth : command.Text.FontSize);
                            newelement.Xinsert = (view == ViewSave.save ? command.Text.Margin.Left / System.Windows.SystemParameters.CaretWidth : command.Text.Margin.Left);
                            newelement.Yinsert = (view == ViewSave.save ? command.Text.Margin.Top / System.Windows.SystemParameters.CaretWidth : command.Text.Margin.Top);
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.disconnectors:
                        {
                            BaseSave newelement = new BaseSave() {TypeDisconnector =  (el as Disconnectors).Type};
                            AddBaseSaveObject(view, el, newelement, Project);
                        }
                        break;
                    case ViewElement.chiefroad:
                        {
                            Traintrack track = el as Traintrack;
                            RoadStation newelement = new RoadStation() { View = track.View, Angle = track.RotateText };
                            newelement.TextSize = (view == ViewSave.save ? track.Text.FontSize / System.Windows.SystemParameters.CaretWidth : track.Text.FontSize);
                            newelement.Xinsert = (view == ViewSave.save ? track.Text.Margin.Left / System.Windows.SystemParameters.CaretWidth : track.Text.Margin.Left);
                            newelement.Yinsert = (view == ViewSave.save ? track.Text.Margin.Top / System.Windows.SystemParameters.CaretWidth : track.Text.Margin.Top);
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.diagnostikCell:
                        {
                            var track = el as DiagnostikCell;
                            RoadStation newelement = new RoadStation() { View = ViewTrack.diagnostikCell };
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.kgu:
                        {
                            BaseSave newelement = new BaseSave();
                            AddBaseSaveObject(view, el, newelement, Project);
                        }
                        break;
                    case ViewElement.ktcm:
                        {
                            BaseSave newelement = new BaseSave();
                            AddBaseSaveObject(view, el, newelement, Project);
                        }
                        break;
                    case ViewElement.lightShunting:
                        {
                            BaseSave newelement = new BaseSave();
                            AddBaseSaveObject(view, el, newelement, Project);
                        }
                        break;
                    case ViewElement.lightTrain:
                        {
                            LightTrainSave newelement = new LightTrainSave() { IsInput = (el as LightTrain).IsInput };
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.line:
                        {
                            //вспомагательная линия
                            LineHelp linehp = el as LineHelp;
                            LineHelpSave newelement = new LineHelpSave()
                            {
                                NameColor = linehp.NameViewColor.NameColor,
                                R = linehp.NameViewColor.R,
                                G = linehp.NameViewColor.G,
                                B = linehp.NameViewColor.B, 
                                WeightStroke = linehp.StrokeCurrent / SaveScroll,
                                IsFillInside = linehp.IsFillInside
                            };
                            foreach (double step in linehp.StrokeDashArray)
                                newelement.StrokeDashArray.Add(step);
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.move:
                        {
                            BaseSave newelement = new BaseSave();
                            AddBaseSaveObject(view, el, newelement, Project);
                        }
                        break;
                    case ViewElement.namestation:
                        {
                            NameStation textform = el as NameStation;
                            NameStationSave newelement = new NameStationSave()
                            {
                                Height = (view == ViewSave.save ? textform.Text.MaxHeight / System.Windows.SystemParameters.CaretWidth : textform.Text.MaxHeight),
                                Width = (view == ViewSave.save ? textform.Text.MaxWidth / System.Windows.SystemParameters.CaretWidth : textform.Text.MaxWidth),
                                Left = (view == ViewSave.save ? textform.Text.Margin.Left / System.Windows.SystemParameters.CaretWidth : textform.Text.Margin.Left),
                                Top = (view == ViewSave.save ? textform.Text.Margin.Top / System.Windows.SystemParameters.CaretWidth : textform.Text.Margin.Top),
                                Angle = textform.RotateText,
                                FontSize = (view == ViewSave.save ? textform.Text.FontSize / System.Windows.SystemParameters.CaretWidth : textform.Text.FontSize)
                            };
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.otrezok:
                        {
                            //линии перегона
                            LinePeregon lineper = el as LinePeregon;
                            LinePeregonSave newelement = new LinePeregonSave() {Graniza = lineper.Graniza };
                            newelement.Key = string.Format("{0}-{1}-{2}", newelement.StationNumber, newelement.StationNumberRight, newelement.Graniza);
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.ramka:
                        {
                            BaseSave newelement = new BaseSave();
                            AddBaseSaveObject(view, el, newelement, Project);
                        }
                        break;
                    case ViewElement.texthelp:
                        {
                            TextHelp _texthelp = el as TextHelp;
                            TextHelpSave newelement = new TextHelpSave()
                            {
                                Height = (view == ViewSave.save ? _texthelp.Text.MaxHeight / System.Windows.SystemParameters.CaretWidth : _texthelp.Text.MaxHeight),
                                Width = (view == ViewSave.save ? _texthelp.Text.MaxWidth / System.Windows.SystemParameters.CaretWidth : _texthelp.Text.MaxWidth),
                                Left = (view == ViewSave.save ? _texthelp.Text.Margin.Left / System.Windows.SystemParameters.CaretWidth : _texthelp.Text.Margin.Left),
                                Top = (view == ViewSave.save ? _texthelp.Text.Margin.Top / System.Windows.SystemParameters.CaretWidth : _texthelp.Text.Margin.Top),
                                Angle = _texthelp.RotateText,
                                FontSize = (view == ViewSave.save ? _texthelp.Text.FontSize / System.Windows.SystemParameters.CaretWidth : _texthelp.Text.FontSize),
                                Text = _texthelp.Text.Text
                            };
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                    case ViewElement.time:
                        {
                            //часы
                            TimeForm _timeform = el as TimeForm;
                            //if (count_time <= 1)
                            //{
                                TimeSave newelement = new TimeSave()
                                {
                                    Height = (view == ViewSave.save ? _timeform.Text.MaxHeight / System.Windows.SystemParameters.CaretWidth : _timeform.Text.MaxHeight),
                                    Width = (view == ViewSave.save ? _timeform.Text.MaxWidth / System.Windows.SystemParameters.CaretWidth : _timeform.Text.MaxWidth),
                                    Left = (view == ViewSave.save ? _timeform.Text.Margin.Left / System.Windows.SystemParameters.CaretWidth : _timeform.Text.Margin.Left),
                                    Top = (view == ViewSave.save ? _timeform.Text.Margin.Top / System.Windows.SystemParameters.CaretWidth : _timeform.Text.Margin.Top),
                                    Angle = _timeform.RotateText,
                                    FontSize = (view == ViewSave.save ? _timeform.Text.FontSize / System.Windows.SystemParameters.CaretWidth : _timeform.Text.FontSize),
                                };
                                SetBaseSaveObject(view, el, newelement);
                                Project.GraficObjects.Add(newelement);
                            //}
                            //count_time++;
                        }
                        break;
                    case ViewElement.numbertrain:
                        {
                            //поле ввода номеров поездов
                            NumberTrain number = el as NumberTrain;
                            NumberTrainSave newelement = new NumberTrainSave() { Graniza = number.Graniza,  RotateText = number.RotateText };
                            SetBaseSaveObject(view, el, newelement);
                            Project.GraficObjects.Add(newelement);
                        }
                        break;
                }
            }
            return Project;
        }


        /// <summary>
        /// перемещаем модель
        /// </summary>
        /// <param name="deltax">изменение по оси x</param>
        /// <param name="deltaY">измененеие по оси y</param>
        public void ModelSize(double deltax, double deltay, TransformGroup groupTransform)
        {
            _po.X += deltax;
            _po.Y += deltay;
            //
            if (groupTransform.Children.Count > 0 && groupTransform.Children[0] is TranslateTransform)
            {
                (groupTransform.Children[0] as TranslateTransform).X += deltax;
                (groupTransform.Children[0] as TranslateTransform).Y += deltay;
            }
        }

        /// <summary>
        /// масштабируем все объекты
        /// </summary>
        /// <param name="scale_factor"></param>
        public void ModelScaleWheel(double scale_factor, double cursorX, double cursorY, TransformGroup groupTransform)
        {
            if (groupTransform.Children.Count > 0)
            {
                ScaleTransform new_scale_trans = new ScaleTransform(scale_factor, scale_factor, cursorX, cursorY);
                groupTransform.Children.Add(new_scale_trans);
                CurrentScroll *= scale_factor;
                //
                ScaleTransform scale = new ScaleTransform(scale_factor, scale_factor, cursorX, cursorY);
                _po = scale.Transform(Po);
            }
        }

    }
}
