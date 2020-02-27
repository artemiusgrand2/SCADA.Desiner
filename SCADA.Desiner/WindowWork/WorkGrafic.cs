using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Controls.Primitives;
using SCADA.Desiner.Constanst;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Enums;
using SCADA.Desiner.GeometryTransform;
using SCADA.Desiner.BaseElement;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.CommandElement;
using SCADA.Desiner.MainMenu;
using SCADA.Desiner.Select;
using SCADA.Desiner.Delegate;
using SCADA.Desiner.Tools;
using SCADA.Desiner.WorkForms;
using SCADA.Common.SaveElement;
using SCADA.Common.Enums;

namespace SCADA.Desiner.WindowWork
{
    public class WorkGrafic
    {
        #region Геометрические объекты

        /// <summary>
        /// коллекция для рисования сложных образов
        /// </summary>
        Polygon lineramkadraw = new Polygon() { Stroke = Brushes.Black, StrokeThickness = 0.7, StrokeDashArray = new DoubleCollection() { 20 } };
      
        private List<Rectangle> collection_frame_point = new List<Rectangle>();
        /// <summary>
        /// коллекция  рамок выделения при попадании в точку
        /// </summary>
        public List<Rectangle> CollectionFramePoint
        {
            get { return collection_frame_point; }
            set { collection_frame_point = value; }
        }
        /// <summary>
        /// элементы  сетки
        /// </summary>
        List<UIElement> _setka = new List<UIElement>();
        /// <summary>
        /// панель инструментов индикации
        /// </summary>
        ToolsGraphics toolspanel;
        /// <summary>
        /// панель инструментов управляющие элементы
        /// </summary>
        ToolsManagement toolspanelcommand;
        /// <summary>
        /// панель свойствами элемента
        /// </summary>
        SettingsFigure toolsetingsfigure;
        /// <summary>
        /// выбранные активные элементы
        /// </summary>
        List<IGraficObejct> activeel = new List<IGraficObejct>();
        /// <summary>
        /// выбранные элементы
        /// </summary>
        public List<IGraficObejct> ActiveEl
        {
            get
            {
                return activeel;
            }
        }
        /// <summary>
        /// текущие отрисованные объекты
        /// </summary>
        Dictionary<int, IGraficObejct> currentDrawElement = new Dictionary<int, IGraficObejct>();
        /// <summary>
        /// текущий элемент рисования
        /// </summary>
        IGraficObejct currentDraw;
        /// <summary>
        /// главное окно
        /// </summary>
        Window window = null;
        /// <summary>
        /// область рисования
        /// </summary>
        Canvas draw_canvas = null;
        //------------------
        /// <summary>
        /// группа трансформаций
        /// </summary>
        TransformGroup groupTransform = new TransformGroup();

        #endregion

        #region Геометричесике данные

        public static double StepValue { get; set;}

        static double _ktextweight = 1.6;
        public static double Kwtext
        {
            get
            {
                return _ktextweight;
            }
            set
            {
                _ktextweight = value;
            }
        }
        static double _ktextheight = 1.00;
        public static double Khtext
        {
            get
            {
                return _ktextheight;
            }
            set
            {
                _ktextheight = value;
            }
        }
        /// <summary>
        /// основное отрицательное изменение масштаба 
        /// </summary>
        double scrollminus = 0.95;
        /// <summary>
        /// основное положительное изменение масштаба
        /// </summary>
        double scrollplus = 1.05;
        /// <summary>
        /// ускоренное отрицательное изменение масштаба 
        /// </summary>
        double scrollminusspeed = 0.83;
        /// <summary>
        /// ускоренное положительное изменение масштаба 
        /// </summary>
        double scrollplusspeed = 1.2;
        /// <summary>
        /// шаг хода клавишь
        /// </summary>
        int step_key = 10;
        /// <summary>
        /// Текущая высота окна
        /// </summary>
        public static double height;
        /// <summary>
        /// Текущая высота окна
        /// </summary>
        public static double width;
     
        #endregion

        #region Флаги

        /// <summary>
        /// окончание рисование рамки
        /// </summary>
        bool limeramktrue = false;
        /// <summary>
        /// рисовать ли перпендикуляр
        /// </summary>
        bool perpendikulirX = true;
        /// <summary>
        /// включени ли режим копирования
        /// </summary>
        bool copy = false;
        /// <summary>
        /// флаг закрытия
        /// </summary>
        public static bool Close = false;

        #endregion

        #region Оперативные значения

        string name_action = string.Empty;
        /// <summary>
        /// файл сохранения графики
        /// </summary>
        string filenamesave = string.Empty;
        /// <summary>
        /// Словарь соответствий типов разъеденителей                   
        /// </summary>
        public static Dictionary<string, TypeDisconnectors> TypeDisconnectorsDic = new Dictionary<string, TypeDisconnectors>() { { "Без нормального положения", TypeDisconnectors.notNormal },
                                                                                                                   { "Нормально включенный", TypeDisconnectors.normalOn },    
                                                                                                                   { "Нормально отключенный", TypeDisconnectors.normalOff}, };

        #endregion

        #region События

        /// <summary>
        /// такты фазы времени
        /// </summary>
        public static event NewTaktEvent NewTart;
        /// <summary>
        /// список имен цветов
        /// </summary>
        public static List<NameColors> NameColors = new List<NameColors>();

        #endregion

        #region Контроллы

        Menu mainmenu;
        StatusBar statuskoordinate;
        Dictionary<string, MenuItem> menuItems;
        Slider step;

        #endregion

        #region Работа с графикой

        /// <summary>
        /// тактовый таймер для индикации времени
        /// </summary>
        System.Timers.Timer Takt = new System.Timers.Timer(1000);
        /// <summary>
        /// класс по работе с графикой
        /// </summary>
        OperationsGrafic operationsGrafic = null;

        #endregion

        public WorkGrafic(Window window, Canvas canvas, Menu mainmenu, Dictionary<string, MenuItem> menuItems, StatusBar statuskoordinate,StatusBarItem pointinsert,
                         StatusBarItem elementsize, StatusBarItem infokoordinate,StatusBarItem mousekoordinate, StatusBarItem NameObjectement, StatusBarItem countselectelement, Slider step)
        {
            SettingsDefult();
            this.window = window;
            this.mainmenu = mainmenu;
            this.statuskoordinate = statuskoordinate;
            this.menuItems = menuItems;
            this.step = step;
            //создаем обработчики событий меню
            foreach (KeyValuePair<string, MenuItem> items in menuItems)
                items.Value.Click += MenuItem_Click;
            //
            draw_canvas = canvas;
            operationsGrafic = new OperationsGrafic(pointinsert, elementsize, infokoordinate, mousekoordinate, NameObjectement, countselectelement);
            Takt.Elapsed += new System.Timers.ElapsedEventHandler(Takt_Elapsed);
            Takt.Start();
            //настраиваем обработчики событий
            window.MouseWheel += Window_MouseWheel;
            window.SizeChanged += Window_SizeChanged;
            window.MouseDown += Window_MouseDown;
            window.MouseUp += Window_MouseUp;
            window.MouseMove += Window_MouseMove;
            window.StateChanged += Window_StateChanged;
            window.KeyDown += Window_KeyDown;
            window.KeyUp += Window_KeyUp;
            window.Closing += Window_Closing;
            window.Activated += Window_Activated;
            window.Deactivated += Window_Deactivated;
        }

        private void SettingsDefult()
        {
            double buffer = 0;
            if (ConfigurationManager.AppSettings["defult_scroll"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["defult_scroll"]) && double.TryParse(ConfigurationManager.AppSettings["defult_scroll"], out buffer))
            {
                double scroll = Math.Abs(double.Parse(ConfigurationManager.AppSettings["defult_scroll"]));
                if (scroll > 0 && scroll <= 100)
                {
                    scrollplus = scroll / 100 + 1;
                    scrollminus = 1 / scrollplus;
                }
            }
            //ускоренный ход масштаба
            if (ConfigurationManager.AppSettings["speed_scroll"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["speed_scroll"]) && double.TryParse(ConfigurationManager.AppSettings["speed_scroll"], out buffer))
            {
                double scroll = Math.Abs(double.Parse(ConfigurationManager.AppSettings["speed_scroll"]));
                if (scroll > 0 && scroll <= 100)
                {
                    scrollplusspeed = scroll / 100 + 1;
                    scrollminusspeed = 1 / scrollplusspeed;
                }
            }
            //ускоренный ход клавишь
            if (ConfigurationManager.AppSettings["step_keydown"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["step_keydown"]) && double.TryParse(ConfigurationManager.AppSettings["step_keydown"], out buffer))
            {
                int step = Math.Abs(int.Parse(ConfigurationManager.AppSettings["step_keydown"]));
                if (step > 0 && step <= 1000)
                    step_key = step;
            }
        }

        /// <summary>
        /// время
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Takt_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (NewTart != null)
                NewTart();
        }

        /// <summary>
        /// создание сетки
        /// </summary>
        private void CreateSetka()
        {
            _setka = new List<UIElement>();
            //рисуем горизональ сетки
            //рисуем горизональ сетки
            for (double i = 0; i < height; i += 20)
            {
                Line lin = new Line()
                {
                    X1 = 0,
                    Y1 = i * OperationsGrafic.CurrentScroll,
                    X2 = width * OperationsGrafic.CurrentScroll,
                    Y2 = i * OperationsGrafic.CurrentScroll,
                    Stroke = MainColors.Grid,
                    StrokeThickness = 0.5,
                };
                _setka.Add(lin);
            }
            //рисуем вертикаль сетки
            for (double i = 0; i < width; i += 20)
            {
                Line lin = new Line()
                {
                    X1 = i * OperationsGrafic.CurrentScroll,
                    Y1 = 0,
                    X2 = i * OperationsGrafic.CurrentScroll,
                    Y2 = height * OperationsGrafic.CurrentScroll,
                    Stroke = MainColors.Grid,
                    StrokeThickness = 0.5,
                };
                _setka.Add(lin);
            }
            //
            foreach (UIElement el in _setka)
            {
                draw_canvas.Children.Add(el);
                Canvas.SetZIndex(el, -1);
            }
        }

        private void FullScreenOption()
        {
            if (window.WindowStyle == System.Windows.WindowStyle.None)
            {
                window.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                menuItems["FullScreen"].IsChecked = false;
                mainmenu.Visibility = System.Windows.Visibility.Visible;
                statuskoordinate.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                window.WindowStyle = System.Windows.WindowStyle.None;
                menuItems["FullScreen"].IsChecked = true;
                mainmenu.Visibility = System.Windows.Visibility.Hidden;
                statuskoordinate.Visibility = System.Windows.Visibility.Hidden;
                window.WindowState = System.Windows.WindowState.Maximized;
                menuItems["FullScreen"].IsChecked = false;
            }
        }

        private void FirstSettings()
        {
            //активируем панель управления элементами индикации
            if (toolspanel == null)
            {
                draw_canvas.RenderTransform = groupTransform;
                groupTransform.Children.Add(new TranslateTransform()); 
                /// <summary>
                /// файл первоначальных настроек
                /// </summary>
                Settings _settings_start = null;
                //
                try
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["file_settings"]) && new FileInfo(ConfigurationManager.AppSettings["file_settings"]).Exists)
                    {
                        var reader = new XmlTextReader(ConfigurationManager.AppSettings["file_settings"]);
                        var deserializer = new XmlSerializer(typeof(Settings));
                        _settings_start = (Settings)deserializer.Deserialize(reader);
                        reader.Close();
                    }
                }
                catch { }
                //
                toolspanel = new ToolsGraphics(_settings_start);
                toolspanel.Show();
                toolspanel.EventAdd += EventAddFigure;
                toolspanel.EventSetka += EventSetkaUpdate;
                toolspanel.EventOriginalSize += EventFirstSize;
                toolspanel.SpisokId += EventSelectObjectNew;
                toolspanel.UpdateElement += EventUpdateSettings;
                toolspanel.Visibility = System.Windows.Visibility.Hidden;
                CreateSetka();
                //активируем панель командных элементов
                if (toolspanelcommand == null)
                {
                    toolspanelcommand = new ToolsManagement(toolspanel.Height, _settings_start);
                    toolspanelcommand.Show();
                    toolspanelcommand.EventAdd += EventAddCommand;
                    toolspanelcommand.UpdateElement += EventUpdateSettingsCommand;
                    toolspanelcommand.Visibility = System.Windows.Visibility.Hidden;
                }
                //активируем панель свойств
                if (toolsetingsfigure == null)
                {
                    toolsetingsfigure = new SettingsFigure(_settings_start);
                    toolsetingsfigure.Show();
                    toolsetingsfigure.SpisokId += EventSelectObjectNew;
                    toolsetingsfigure.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

      

        /// <summary>
        /// изменяем свойства объекта управления
        /// </summary>
        private void EventUpdateSettingsCommand(string Name, string Help,  ViewCommand viewcommad, ViewPanel viewpanel, int id)
        {
            try
            {
                IGraficObejct graf = currentDrawElement[id] as IGraficObejct;
                if (graf != null)
                {
                    graf.NameObject = Name;
                    //
                    ButtonCommand command = currentDrawElement[id] as ButtonCommand;
                    if (command != null)
                    {
                        //настраиваем тескт блок
                        command.Text.Text = command.NameObject.Split(new char[] { ';' })[0];
                        command.ViewCommand = viewcommad;
                        command.ViewPanel = viewpanel;
                        command.HelpText = Help;
                        double width = HelpesCalculation.LenghtSide(((ArcSegment)command.Figure.Figures[0].Segments[command.Figure.Figures[0].Segments.Count - 2]).Point, command.Figure.Figures[0].StartPoint);
                        double height = HelpesCalculation.LenghtSide(((ArcSegment)command.Figure.Figures[0].Segments[2]).Point, command.Figure.Figures[0].StartPoint);
                        command.Text.FontSize = HelpesCalculation.FontSizeText(_ktextweight, _ktextheight,
                            new Rectangle()
                            {
                                Width = width,
                                Height = height
                            },
                         command.Text.Text, command.RotateText);

                        command.Text.Margin = HelpesCalculation.AlingmentCenter(command.Figure.Figures[0].StartPoint.X, command.Figure.Figures[0].StartPoint.Y, width, height, command.Text, graf);
                    }
                    NameStation text = currentDrawElement[id] as NameStation;
                    if (text != null)
                        text.Text.Text = text.NameObject;
                }
                //Добавляем изменения в историю
                operationsGrafic.AddHistory(currentDrawElement);
            }
            catch { }
        }

        /// <summary>
        /// действие при добавлении коммандного элемента
        /// </summary>
        private void EventAddCommand()
        {
            window.Cursor = Cursors.None;
            switch (toolspanelcommand.CurrentDraw)
            {
                case ViewElement.buttoncommand:
                    {
                        currentDraw = new ButtonCommand(OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
            }

        }

        /// <summary>
        /// изменяем свойства объекта индикации
        /// </summary>
        /// <param name="Id">ади</param>
        /// <param name="Name">название объекта</param>
        /// <param name="numberstationleft">номер станции</param>
        private void EventUpdateSettings(List<MyTable> tables)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(NewThreadingUpdate), tables);
        }

        private void NewThreadingUpdate(object sender)
        {
            lock (this)
            {
                window.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        List<MyTable> tables = (List<MyTable>)sender;
                        //
                        foreach (MyTable row in tables)
                        {
                            try
                            {
                                IGraficObejct graf = currentDrawElement[row.Id] as IGraficObejct;
                                if (graf != null)
                                {
                                    //данные по умолчанию
                                    graf.NameObject = row.Name;
                                    graf.Notes = row.Notes;
                                    graf.IsVisible = row.IsVisible;
                                    graf.StationNumber = int.Parse(row.Numberstationleft);
                                    graf.StationNumberRight = int.Parse(row.Numberstationright);
                                    //
                                    Traintrack path = currentDrawElement[row.Id] as Traintrack;
                                    if (path != null)
                                    {
                                        //настраиваем тескт блок
                                        path.Text.Text = path.NameObject.Split(new char[]{';'})[0];
                                        double width = HelpesCalculation.LenghtSide(((ArcSegment)path.Figure.Figures[0].Segments[path.Figure.Figures[0].Segments.Count - 2]).Point, path.Figure.Figures[0].StartPoint);
                                        double height = HelpesCalculation.LenghtSide(((ArcSegment)path.Figure.Figures[0].Segments[2]).Point, path.Figure.Figures[0].StartPoint);
                                        path.Text.FontSize = HelpesCalculation.FontSizeText(_ktextweight, _ktextheight,
                                            new Rectangle()
                                            {
                                                Width = width,
                                                Height = height
                                            },
                                         path.Text.Text, path.RotateText);

                                        path.Text.Margin = HelpesCalculation.AlingmentCenter(path.Figure.Figures[0].StartPoint.X, path.Figure.Figures[0].StartPoint.Y, width, height, path.Text, graf);
                                    }
                                    //
                                    NameStation text = currentDrawElement[row.Id] as NameStation;
                                    if (text != null)
                                        text.Text.Text = text.NameObject;
                                    //
                                    TextHelp texthelp = currentDrawElement[row.Id] as TextHelp;
                                    if (texthelp != null)
                                        texthelp.Text.Text = texthelp.NameObject;
                                    //
                                    LightTrain light = currentDrawElement[row.Id] as LightTrain;
                                    if (light != null)
                                        light.IsInput = row.IsInput;
                                    //
                                    Disconnectors disconnectors = currentDrawElement[row.Id] as Disconnectors;
                                    if (disconnectors != null)
                                    {
                                        if (TypeDisconnectorsDic.ContainsKey(row.Type))
                                        {
                                            disconnectors.Type = TypeDisconnectorsDic[row.Type];
                                            disconnectors.DefaultColor();
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                        //Добавляем изменения в историю
                        operationsGrafic.AddHistory(currentDrawElement);
                    }
                    catch { }
                }));
            }
        }

        /// <summary>
        /// Выделение объекта
        /// </summary>
        /// <param name="id">список выделенных объектов</param>
        private void EventSelectObjectNew(List<int> id, PointFigure figure)
        {
            operationsGrafic.SelectObjectNew(id, figure, activeel, draw_canvas, window, currentDrawElement);
            window.Focus();
        }

        /// <summary>
        /// действия при отображеннии или затемнении сетки
        /// </summary>
        private void EventSetkaUpdate(bool Checked)
        {
            if (Checked)
            {
                foreach (UIElement el in _setka)
                    el.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                foreach (UIElement el in _setka)
                    el.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SetSettingsFigure()
        {
            currentDraw.Id = operationsGrafic.CurrentFreeId;
            draw_canvas.Children.Add((UIElement)currentDraw);
            currentDrawElement.Add(currentDraw.Id, currentDraw);
            operationsGrafic.CurrentFreeId++;
            if (currentDraw is IText)
                draw_canvas.Children.Add((currentDraw as IText).Text);
            //
            operationsGrafic.EventAddobject(new List<IGraficObejct> { currentDraw });
        }

        /// <summary>
        /// действия при добавлении фигуры
        /// </summary>
        private void EventAddFigure()
        {
            window.Cursor = Cursors.None;

            switch (toolspanel.CurrentDraw)
            {
                case ViewElement.chiefroad:
                    {
                        currentDraw = new Traintrack(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY, ViewTrack.track);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.help_element:
                    {
                        currentDraw = new Traintrack(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY, ViewTrack.helpelement);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.analogCell:
                    {
                        currentDraw = new Traintrack(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY, ViewTrack.analogCell);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.diagnostikCell:
                    {
                        currentDraw = new DiagnostikCell(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.buttonstation:
                    {
                        currentDraw = new ButtonStation(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.signal:
                    {
                        currentDraw = new Signal(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.kgu:
                    {
                        currentDraw = new KGU(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.ktcm:
                    {
                        currentDraw = new KTCM(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.move:
                    {
                        currentDraw = new Move(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.disconnectors:
                    {
                        currentDraw = new Disconnectors(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.namestation:
                    {
                        currentDraw = new NameStation(1, toolspanel.CurrentNumberStation, toolspanel.CurrentNameStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.texthelp:
                    {
                        currentDraw = new TextHelp(1, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.lightShunting:
                    {
                        currentDraw = new LightShunting(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.lightTrain:
                    {
                        currentDraw = new LightTrain(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                       SetSettingsFigure();
                    }
                    break;
                case ViewElement.time:
                    {
                        currentDraw = new TimeForm(1, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.ramka:
                    {
                        draw_canvas.Children.Add(lineramkadraw);
                        limeramktrue = false;
                        window.Cursor = Cursors.Arrow;
                    }
                    break;
                case ViewElement.line:
                    {
                        window.Cursor = Cursors.Arrow;
                        currentDraw = new LineHelp(toolspanel.CurrentNumberStation);
                    }
                    break;
                case ViewElement.otrezok:
                    {
                        window.Cursor = Cursors.Arrow;
                        currentDraw = new LinePeregon(toolspanel.CurrentNumberStation);
                    }
                    break;
                case ViewElement.arrowmove:
                    {
                        currentDraw = new ArrowMove(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
                case ViewElement.numbertrain:
                    {
                        currentDraw = new NumberTrain(1, toolspanel.CurrentNumberStation, OperationsGrafic.CursorX, OperationsGrafic.CursorY);
                        SetSettingsFigure();
                    }
                    break;
            }
        }

        private void EventMenuItemsClick(RoutedEventArgs eventInfo)
        {
            switch ((eventInfo.Source as MenuItem).Name)
            {
                case "FullScreen":
                    {
                        if (window.WindowStyle == System.Windows.WindowStyle.None)
                        {
                            window.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                            menuItems["FullScreen"].IsChecked = false;
                            mainmenu.Visibility = System.Windows.Visibility.Visible;
                            statuskoordinate.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            window.WindowStyle = System.Windows.WindowStyle.None;
                            menuItems["FullScreen"].IsChecked = true;
                            mainmenu.Visibility = System.Windows.Visibility.Hidden;
                            statuskoordinate.Visibility = System.Windows.Visibility.Hidden;
                            window.WindowState = System.Windows.WindowState.Maximized;
                            menuItems["FullScreen"].IsChecked = false;
                        }
                    }
                    break;
                case "SettingsStation":
                    {
                        WindowShowHide(menuItems["SettingsStation"], toolspanel);
                    }
                    break;
                case "SettingsCommand":
                    {
                        WindowShowHide(menuItems["SettingsCommand"], toolspanelcommand);
                    }
                    break;
                case "SettingsFigure":
                    {
                        WindowShowHide(menuItems["SettingsFigure"], toolsetingsfigure);
                    }
                    break;
                case "NewProject":
                    {
                        //проверяем были ли изменения в графики
                        if (operationsGrafic.History.Count != operationsGrafic.LastCountLevelHistory)
                        {
                            if ((bool)(new CloseProgramm().ShowDialog()))
                                SaveOperation(false);
                        }
                        operationsGrafic.ProjectOpen = null;
                        filenamesave = string.Empty;
                        ScrollStrokeThickess.SetScrollStrokeThickess(OperationsGrafic.SaveScroll, 1);
                        OperationsGrafic.SaveScroll = 1;
                        window.Title = filenamesave;
                        operationsGrafic.DeleteCurrentState(draw_canvas, toolspanel, toolspanelcommand, activeel, currentDrawElement);
                        operationsGrafic.OriginalSizeCurrent(true, activeel, groupTransform, lineramkadraw);
                        operationsGrafic.ClearHistory();
                        operationsGrafic.EventUpdateObject(null);
                    }
                    break;
                case "OpenProject":
                    {
                        //проверяем были ли изменения в графики
                        if (operationsGrafic.History.Count != operationsGrafic.LastCountLevelHistory)
                        {
                            if ((bool)(new CloseProgramm().ShowDialog()))
                                SaveOperation(false);
                        }
                        //
                        OpenOperation();
                    }
                    break;
                case "SaveProject":
                    {
                        SaveOperation(false);
                    }
                    break;
                case "SaveAsProject":
                    {
                        SaveOperation(true);
                    }
                    break;
                case "ExitProject":
                    {
                        window.Close();
                    }
                    break;
            }
        }

        private void OpenOperation()
        {
            System.Windows.Forms.OpenFileDialog openFileDial = new System.Windows.Forms.OpenFileDialog();
            openFileDial.Filter = "Xml files (*.xml)|*.xml";
            //
            try
            {
                if (openFileDial.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Open(openFileDial.FileName);
                    filenamesave = openFileDial.FileName;
                    window.Title = "Проект находится: " + filenamesave;

                }
            }
            catch (Exception error)
            {
                Close = true;
                MessageBox.Show(error.Message);
            }
        }

        private void SaveOperation(bool saveAS)
        {
            System.Windows.Forms.SaveFileDialog saveFileDial = new System.Windows.Forms.SaveFileDialog();
            saveFileDial.Filter = "Xml files (*.xml)|*.xml";
            var Project = operationsGrafic.SaveAnalis(ViewSave.save, currentDrawElement.Values.ToList());
            if (operationsGrafic.ProjectOpen != null)
                Project.Transform = operationsGrafic.ProjectOpen.Transform;
            //
            if (saveAS || (!saveAS && filenamesave.Length == 0))
            {
                if (saveFileDial.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Save(saveFileDial.FileName, Project);
                    window.Title = "Проект находится: " + saveFileDial.FileName;
                    if (!saveAS)
                        filenamesave = saveFileDial.FileName;
                }
            }
            else
            {
                if (!saveAS && filenamesave.Length > 0)
                {
                    Save(filenamesave, Project);
                    window.Title = "Проект находится: " + filenamesave;
                }
            }
        }

        private void Save(string filename, StrageProject Project)
        {
            try
            {
                name_action = "Сохранение проекта !"; 
                Close = false;
                System.Threading.Thread potoksave = new System.Threading.Thread(Wait);
                potoksave.SetApartmentState(System.Threading.ApartmentState.STA);
                potoksave.Start();
                System.Threading.Thread.Sleep(200);
                using (Stream savestream = new FileStream(filename, FileMode.Create))
                {
                    // Указываем тип того объекта, который сериализуем
                    XmlSerializer xml = new XmlSerializer(typeof(StrageProject));
                    // Сериализуем
                    xml.Serialize(savestream, Project);
                    savestream.Close();
                }
                //
                operationsGrafic.LastCountLevelHistory = operationsGrafic.History.Count;
                Close = true;
            }
            catch (Exception error)
            {
                Close = true;
                MessageBox.Show(error.Message);
            }
        }

        /// <summary>
        /// открываем ранее сохраненный проект
        /// </summary>
        /// <param name="filename">путь к проету</param>
        private void Open(string filename)
        {
            Close = false;
            name_action = "Загрузка проекта !";
            System.Threading.Thread potokopen = new System.Threading.Thread(Wait);
            potokopen.SetApartmentState(System.Threading.ApartmentState.STA);
            potokopen.Start();
            System.Threading.Thread.Sleep(200);
            var reader = new XmlTextReader(filename);
            // reader.WhitespaceHandling = WhitespaceHandling.None;
            var deserializer = new XmlSerializer(typeof(StrageProject));
            operationsGrafic.ProjectOpen = (StrageProject)deserializer.Deserialize(reader);
            reader.Close();
            //первоначальное значение счетчика Id
            operationsGrafic.CurrentFreeId = 0;
            //отрисовываем сохраненный проект
            operationsGrafic.DrawOpen(operationsGrafic.ProjectOpen, draw_canvas, activeel, currentDrawElement, toolspanel, toolspanelcommand, groupTransform, lineramkadraw);
            Close = true;
        }

        private void Wait()
        {
            SaveOpenFormsWpf open = new SaveOpenFormsWpf(name_action);
            open.ShowDialog();
        }

        private void WindowShowHide(MenuItem menu, Window win)
        {
            if (win.Visibility == System.Windows.Visibility.Visible)
            {
                win.Visibility = System.Windows.Visibility.Hidden;
                menu.IsChecked = false;
            }
            else
            {
                win.Visibility = System.Windows.Visibility.Visible;
                menu.IsChecked = true;
            }
        }

        public void EventAddHistoty()
        {
            operationsGrafic.AddHistory(currentDrawElement);
        }

        private void EventFirstSize()
        {
            operationsGrafic.OriginalSizeCurrent(true, activeel, groupTransform, lineramkadraw);
        }

        private void EventNewSource(string path, ViewArea viewArea)
        {
            operationsGrafic.SetPathPicture(path, activeel, viewArea);
        }

        private void EventNewLayer(ViewLayer viewlayer, int layer)
        {
            foreach (IGraficObejct el in activeel)
            {
                switch (viewlayer)
                {
                    case ViewLayer.bottom:
                        {
                            SetLayer(el, el.ZIndex - 1);
                        }
                        break;
                    case ViewLayer.top:
                        {
                            SetLayer(el, el.ZIndex + 1);
                        }
                        break;
                    case ViewLayer.setvalue:
                        {
                            SetLayer(el, layer);
                        }
                        break;
                }
            }
            //
            operationsGrafic.AddHistory(currentDrawElement);
        }

        private void SetLayer(IGraficObejct el, int layer)
        {
            Canvas.SetZIndex((UIElement)el, el.ZIndex = layer);
            if (el is IText)
                Canvas.SetZIndex((el as IText).Text, layer + 1);
        }

        public  static double Round(double x)
        {
            if (x % StepValue == 0)
                return x;
            else
            {
                return Math.Round(x / StepValue) * StepValue;
            }
        }

        public static Point RoundPoint(Point point, double deltax, double deltay)
        {
            return new Point(Round(point.X + deltax), Round(point.Y + deltay));
        }

        private void EventMouseDown(MouseButtonEventArgs eventInfo)
        {
            window.Focus();
            //добавляем историю перемещения объектов
            if (activeel.Count > 0 && operationsGrafic.Update)
            {
                operationsGrafic.AddHistory(currentDrawElement);
                operationsGrafic.Update = false;
            }
            //запоминаем позицию курсора
            OperationsGrafic.CursorX = Round(eventInfo.GetPosition(draw_canvas).X);
            OperationsGrafic.CursorY = Round(eventInfo.GetPosition(draw_canvas).Y);
            //
            switch (eventInfo.ChangedButton)
            {
                case MouseButton.Left:
                    {
                        copy = false;
                        //проверяем необходимо ли рисовать объект индикации и какой
                        if (toolspanel.CurrentDraw == ViewElement.ramka)
                        {
                            if (!limeramktrue)
                            {
                                if (lineramkadraw.Points.Count > 0)
                                {
                                    if (perpendikulirX)
                                        lineramkadraw.Points.Add(new Point(OperationsGrafic.CursorX, lineramkadraw.Points[lineramkadraw.Points.Count - 1].Y));
                                    else
                                        lineramkadraw.Points.Add(new Point(lineramkadraw.Points[lineramkadraw.Points.Count - 1].X, OperationsGrafic.CursorY));
                                    perpendikulirX = !perpendikulirX;
                                }
                                else
                                {
                                    lineramkadraw.Points.Add(new Point(OperationsGrafic.CursorX, OperationsGrafic.CursorY));
                                    lineramkadraw.Points.Add(new Point(OperationsGrafic.CursorX, OperationsGrafic.CursorY));
                                }

                            }
                        }
                        else
                        {
                            if (toolspanel.CurrentDraw == ViewElement.line || toolspanel.CurrentDraw == ViewElement.otrezok)
                            {
                                if (toolspanel.CurrentDraw == ViewElement.line)
                                    operationsGrafic.NewPointAddLineHelp(currentDraw, draw_canvas, toolspanel, currentDrawElement);
                                else
                                    operationsGrafic.NewPointAddLineHelp(currentDraw, draw_canvas, toolspanel, currentDrawElement);
                            }
                            else
                            {
                                if (toolspanel.CurrentDraw != ViewElement.none)
                                    operationsGrafic.AddHistory(currentDrawElement);
                                //
                                if (toolspanel.CurrentDraw != ViewElement.area_picture && toolspanel.CurrentDraw != ViewElement.webBrowser)
                                    toolspanel.CurrentDraw = ViewElement.none;
                                window.Cursor = Cursors.Arrow;
                            }
                        }
                        //управляющая графика
                        if (toolspanelcommand.CurrentDraw == ViewElement.buttoncommand)
                        {
                            operationsGrafic.AddHistory(currentDrawElement);
                            toolspanelcommand.CurrentDraw = ViewElement.none;
                            window.Cursor = Cursors.Arrow;
                        }
                        //
                       // if (toolspanelcommand.CurrentDraw == ViewElement.none && toolspanel.CurrentDraw == ViewElement.none)
                            operationsGrafic.ActiveElementOne(window, eventInfo.GetPosition(draw_canvas), draw_canvas, toolspanelcommand, toolspanel, activeel,
                                                              currentDrawElement);
                    }
                    break;
                case MouseButton.Right:
                    {
                        EventContexMenu(eventInfo.GetPosition(draw_canvas));
                        break;
                    }
            }
        }

        private void EventContexMenu(Point pointclick)
        {
            operationsGrafic.EmptyDeltaX();
            if (window.ContextMenu != null)
                window.ContextMenu = null;
            //
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                double number = -1;
                if (activeel.Count == 1)
                    number = activeel[activeel.Count - 1].StationNumber;
                MainContextMenu menu = new MainContextMenu(currentDrawElement, number, operationsGrafic.ElementSelect, this);
                window.ContextMenu = menu.GetContextMenu();
                menu.SpisokId += EventSelectObjectNew;
                menu.Delete += EventDeleteObject;
                menu.OriginalSize += EventFirstSize;
                menu.NewSource += EventNewSource;
                menu.NewLayer += EventNewLayer;
                menu.Rotate += EventRotateElement;
                menu.HatchLine += EventHatchLine;
                menu.Reverse += EventReversePeregon;
                menu.NewColor += EventNewColorElement;
                menu.NewWeight += EventNewWeightElement;
                menu.IsFillInside += EventIsFillInside;
                menu.NewScroll += EventNewScrollElement;
                menu.NewSize += EventNewSizeElement;
                menu.NewAligment += EventAligmentElement;
                menu.NewFontSize += EventNewFontSize;
            }
            else
            {
                operationsGrafic.EmptyHelpStatus();
                if (activeel.Count ==0 ||(activeel.Count >0 &&  draw_canvas.Children.Contains(OperationsGrafic.FramePoint)))
                {
                    //if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    //{
                    draw_canvas.Children.Remove(OperationsGrafic.FramePoint);
                    SelectOperation.SelectStroke(currentDrawElement, pointclick, activeel);
                    //
                    if (activeel.Count == 1 && activeel[0].SelectStroke)
                    {
                        operationsGrafic.ShowGeometryElement(activeel);
                        if (activeel[0] is IShowSettings)
                            draw_canvas.Children.Add(OperationsGrafic.FramePoint);
                    }
                    //
                    operationsGrafic.EventSelectobject(activeel);
                }
               // }
            }
        }

        private void EventIsFillInside()
        {
            operationsGrafic.IsFillInside(activeel, currentDrawElement);
        }

        public void EventDeleteObject()
        {
            operationsGrafic.DeleteActiveObejct(activeel, currentDrawElement, draw_canvas);
        }

        public void EventRotateElement(double angle)
        {
            operationsGrafic.RotateElement(angle, activeel, currentDrawElement);
        }

        public void EventHatchLine(IList<double> arrayhatch)
        {
            operationsGrafic.HatchLine(arrayhatch, activeel, currentDrawElement);
        }

        private void EventReversePeregon()
        {
            operationsGrafic.ReversePeregon(activeel);
        }

        private void EventNewColorElement(NameColors NameColor)
        {
            operationsGrafic.NewColorElement(NameColor, activeel, currentDrawElement);
        }

        private void EventNewWeightElement(double Weight)
        {
            operationsGrafic.WeightElement(Weight, activeel, currentDrawElement);
        }

        private void EventNewScrollElement(double Scroll)
        {
            foreach (IGraficObejct el in activeel)
            {
                if (el is IScrollElement)
                {
                    IScrollElement scroll = el as IScrollElement;
                    Point centre = scroll.CentreFigure();
                    ScaleTransform transform = new ScaleTransform(Scroll / scroll.CurrencyScroll, Scroll / scroll.CurrencyScroll, centre.X, centre.Y);
                    scroll.ScrollFigure(transform, Scroll);
                }
            }
            //
            operationsGrafic.ShowGeometryElement(activeel);
            operationsGrafic.AddHistory(currentDrawElement);
        }

        private void EventNewSizeElement(double width, double height)
        {
            operationsGrafic.SizeElement(width, height, activeel, currentDrawElement);
        }

        private void EventAligmentElement(double deltaX, double deltaY)
        {
            operationsGrafic.AligmentElement(deltaX, deltaY, activeel, currentDrawElement);
        }

        private void EventNewFontSize(double fontsize)
        {
            operationsGrafic.FontSize(fontsize, activeel, currentDrawElement);
        }

        private void EventMouseMove(MouseEventArgs eventInfo)
        {
            ////получаем текущую позицию мыши
            Point newposition = new Point(Round(eventInfo.GetPosition(draw_canvas).X), Round(eventInfo.GetPosition(draw_canvas).Y));
            operationsGrafic.GetNameSelectObejct(currentDrawElement, newposition);
            //рисуем новый объект индикации
            if (operationsGrafic.GroupElement.ContainsKey(toolspanel.CurrentDraw))
            {
                switch (operationsGrafic.GroupElement[toolspanel.CurrentDraw])
                {
                    case ViewGroupElement.baseElement:
                        currentDraw.SizeFigure(newposition.X - OperationsGrafic.CursorX, newposition.Y - OperationsGrafic.CursorY);
                        break;
                    case ViewGroupElement.lineSegmentClosed:
                        if (lineramkadraw.Points.Count > 1)
                        {
                            if (perpendikulirX)
                                lineramkadraw.Points[lineramkadraw.Points.Count - 1] = new Point(newposition.X, lineramkadraw.Points[lineramkadraw.Points.Count - 2].Y);
                            else
                                lineramkadraw.Points[lineramkadraw.Points.Count - 1] = new Point(lineramkadraw.Points[lineramkadraw.Points.Count - 2].X, newposition.Y);
                        }
                        break;
                    case ViewGroupElement.lineSegmentOpend:
                        operationsGrafic.MovePointElement(newposition, currentDraw, toolspanel, draw_canvas);
                        break;
                }
            }
            //рисуем новый объект управления
            switch (toolspanelcommand.CurrentDraw)
            {
                case ViewElement.buttoncommand:
                    currentDraw.SizeFigure(newposition.X - OperationsGrafic.CursorX, newposition.Y - OperationsGrafic.CursorY);
                    break;
            }
            //перемещаем захваченный объект
            if (eventInfo.LeftButton == MouseButtonState.Pressed)
                operationsGrafic.RectangleRamkaModify(newposition);
            //
            if (eventInfo.RightButton == MouseButtonState.Pressed)
            {
                if (activeel.Count > 0)
                {
                    if (activeel[activeel.Count - 1].Selectobject)
                    {
                        if (window.Cursor != Cursors.SizeAll)
                            window.Cursor = Cursors.SizeAll;
                    }
                    //
                   operationsGrafic.OprtaObejctMove(toolspanel, newposition, activeel, true);
                }
            }
            //
            if (eventInfo.MiddleButton == MouseButtonState.Pressed && operationsGrafic.FrameRec == null)
            {
                window.Cursor = Cursors.SizeAll;
                operationsGrafic.ModelSize(newposition.X - OperationsGrafic.CursorX, newposition.Y - OperationsGrafic.CursorY, groupTransform);
                operationsGrafic.EventUpdateGeometry(activeel);
            }
            //заканчиваем рисовать рамку для множественного выделения
            if (eventInfo.LeftButton == MouseButtonState.Released && eventInfo.RightButton == MouseButtonState.Released && eventInfo.MiddleButton == MouseButtonState.Released)
            {
                if (operationsGrafic.FrameRec != null)
                {
                    draw_canvas.Children.Remove(operationsGrafic.FrameRec);
                    operationsGrafic.FrameRec = null;
                }
                else
                {
                    if (toolspanelcommand.CurrentDraw == ViewElement.none && toolspanel.CurrentDraw == ViewElement.none)
                    {
                        MouseSelect cursormouse = SelectOperation.IsSelectElementMouse(currentDrawElement, eventInfo.GetPosition(draw_canvas));
                        //
                        if (cursormouse.mousefill || cursormouse.mousestroke)
                        {
                            if (cursormouse.mousefill)
                            {
                                window.Cursor = Cursors.Hand;
                            }
                            else
                            {
                                switch (cursormouse.aligment)
                                {
                                    case AligmentStroke.horizontal:
                                        window.Cursor = Cursors.SizeWE;
                                        break;
                                    case AligmentStroke.vertical:
                                        window.Cursor = Cursors.SizeNS;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (window.Cursor != Cursors.Arrow)
                                window.Cursor = Cursors.Arrow;
                        }
                    }
                }
            }
            //меняем текущие координаты
            OperationsGrafic.CursorX = Round(eventInfo.GetPosition(draw_canvas).X);
            OperationsGrafic.CursorY = Round(eventInfo.GetPosition(draw_canvas).Y);
        }

        private void EventMouseWhel(MouseWheelEventArgs eventInfo)
        {
            double Xnew = eventInfo.GetPosition(draw_canvas).X; //(eventInfo.GetPosition(draw_canvas).X - OperationsGrafic.Po.X) / OperationsGrafic.CurrentScroll;
            double Ynew = eventInfo.GetPosition(draw_canvas).Y; // (eventInfo.GetPosition(draw_canvas).Y - OperationsGrafic.Po.Y) / OperationsGrafic.CurrentScroll;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (eventInfo.Delta > 0)
                    operationsGrafic.ModelScaleWheel(scrollplusspeed, Xnew, Ynew, groupTransform);
                else
                    operationsGrafic.ModelScaleWheel(scrollminusspeed, Xnew, Ynew, groupTransform);
            }
            else
            {
                if (eventInfo.Delta > 0)
                    operationsGrafic.ModelScaleWheel(scrollplus, Xnew, Ynew, groupTransform);
                else
                    operationsGrafic.ModelScaleWheel(scrollminus, Xnew, Ynew, groupTransform);
            }
        }

        private void EventMouseUp(MouseButtonEventArgs eventInfo)
        {
            if (eventInfo.LeftButton == MouseButtonState.Released)
            {
                //добавляем историю перемещения объектов
                if (activeel.Count > 0 && operationsGrafic.Update)
                {
                    operationsGrafic.AddHistory(currentDrawElement);
                    operationsGrafic.Update = false;
                }
                //
                if (toolspanelcommand.CurrentDraw == ViewElement.tablenumbertrain || toolspanelcommand.CurrentDraw == ViewElement.area_message
                     || toolspanelcommand.CurrentDraw == ViewElement.area_station || toolspanelcommand.CurrentDraw == ViewElement.tableautopilot || toolspanel.CurrentDraw == ViewElement.area_picture || toolspanel.CurrentDraw == ViewElement.webBrowser)
                {
                    if (operationsGrafic.FrameRec != null)
                    {
                        if (window.Cursor != Cursors.Arrow)
                            window.Cursor = Cursors.Arrow;
                        Area area = null;
                        //
                        switch (toolspanel.CurrentDraw)
                        {
                            case ViewElement.area_picture:
                                area = new Area(operationsGrafic.FrameRec, ViewArea.area_picture, "Область поездов");
                                break;
                            case ViewElement.webBrowser:
                                area = new Area(operationsGrafic.FrameRec, ViewArea.webBrowser, "Область браузера");
                                break;
                            default:
                                {
                       
                                    switch (toolspanelcommand.CurrentDraw)
                                    {
                                        case ViewElement.tablenumbertrain:
                                            area = new Area(operationsGrafic.FrameRec, ViewArea.table_train, "Область поездов");
                                            break;
                                        case ViewElement.area_station:
                                            area = new Area(operationsGrafic.FrameRec, ViewArea.area_station, "Область станции");
                                            break;
                                        case ViewElement.area_message:
                                            area = new Area(operationsGrafic.FrameRec, ViewArea.area_message, "Область справки");
                                            break;
                                        case ViewElement.tableautopilot:
                                            area = new Area(operationsGrafic.FrameRec, ViewArea.table_autopilot, "Область автодействия");
                                            break;
                                            //default:
                                            //    area = new Area(operationsGrafic.FrameRec, ViewArea.table_train, "Область поездов");
                                            //    break;
                                    }
                                }
                                break;
                        }
                        toolspanelcommand.CurrentDraw = ViewElement.none;
                        toolspanel.CurrentDraw = ViewElement.none;
                        currentDraw = area;
                        SetSettingsFigure();
                        //area.Id = operationsGrafic.CurrentFreeId;
                        //draw_canvas.Children.Add(area);
                        //currentDrawElement.Add(area.Id, area);
                        //operationsGrafic.CurrentFreeId++;
                        operationsGrafic.AddHistory(currentDrawElement);
                        draw_canvas.Children.Remove(operationsGrafic.FrameRec);
                        operationsGrafic.FrameRec = null;
                       
                    }
                }
                else
                {
                    if (operationsGrafic.FrameRec != null)
                    {
                        operationsGrafic.ActiveElements(operationsGrafic.FrameRec, currentDrawElement, activeel);
                        draw_canvas.Children.Remove(operationsGrafic.FrameRec);
                        operationsGrafic.FrameRec = null;
                    }
                }
            }
        }

        private void EventWindowClosing(System.ComponentModel.CancelEventArgs eventInfo)
        {
            if (App.Close)
            {
                if (operationsGrafic.LastCountLevelHistory != operationsGrafic.History.Count)
                {
                    if ((bool)(new CloseProgramm().ShowDialog()))
                    {
                        SaveOperation(false);
                        CloseProgramm();
                    }
                    else
                        CloseProgramm();
                }
                else
                {
                    CloseProgramm();
                }
            }
        }

        private void CloseProgramm()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["file_settings"]))
            {
                using (Stream savestream = new FileStream(ConfigurationManager.AppSettings["file_settings"], FileMode.Create))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(Settings));
                    xml.Serialize(savestream, new Settings()
                    {
                        StartCommand_X = toolspanelcommand.Left,
                        StartCommand_Y = toolspanelcommand.Top,
                        StartCommand_Height = toolspanelcommand.Height,
                        StartCommand_Width = toolspanelcommand.Width,
                        StartGrafic_X = toolspanel.Left,
                        StartGrafic_Y = toolspanel.Top,
                        StartGrafic_Height = toolspanel.Height,
                        StartGrafic_Widht = toolspanel.Width,
                        StartSettings_X = toolsetingsfigure.Left,
                        StartSettings_Y = toolsetingsfigure.Top,
                        StartSettings_Height = toolsetingsfigure.Height,
                        StartSettings_Width = toolsetingsfigure.Width
                    });
                    savestream.Close();
                }
            }
            //SaveTakt.Stop();
            Takt.Stop();
            toolspanel.ClosingTools = false;
            toolspanel.Close();
            //
            toolspanelcommand.ClosingCommand = false;
            toolspanelcommand.Close();
            //
            toolsetingsfigure.ClosingSettings = false;
            toolsetingsfigure.Close();
        }

        private void EventWindowKeyDown(KeyEventArgs eventInfo)
        {
            switch (eventInfo.Key)
            {
                case Key.Enter:
                    switch (toolspanel.CurrentDraw)
                    {
                        case ViewElement.line:
                            operationsGrafic.CancelDrawLine(draw_canvas, toolspanel, currentDraw);
                            break;
                        case ViewElement.ramka:
                            {
                                limeramktrue = true;
                                perpendikulirX = true;
                                operationsGrafic.CancelDrawRamkaStation(draw_canvas, toolspanel, currentDraw, lineramkadraw, currentDrawElement);
                            }
                            break;
                        case ViewElement.otrezok:
                            operationsGrafic.CancelDrawLine(draw_canvas, toolspanel, currentDraw);
                            break;
                    }
                    break;
                case Key.Space:
                    {
                        FullScreenOption();
                        operationsGrafic.ZeroKoordinate();
                    }
                    break;
                case Key.N:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            foreach (IGraficObejct graf in activeel)
                            {
                                if (graf is IScrollElement)
                                {
                                    (graf as IScrollElement).Normal();
                                }
                            }
                        }
                    }
                    break;
                case Key.Add:
                    operationsGrafic.ModelScaleWheel(scrollplus, OperationsGrafic.CursorX, OperationsGrafic.CursorY, groupTransform);
                    break;
                case Key.Subtract:
                    operationsGrafic.ModelScaleWheel(scrollminus, OperationsGrafic.CursorX, OperationsGrafic.CursorY, groupTransform);
                    break;
                case Key.Q:
                    operationsGrafic.OriginalSizeCurrent(true, activeel, groupTransform, lineramkadraw);
                    break;
                case Key.Escape:
                    operationsGrafic.DefaultColor(activeel, draw_canvas, toolspanel);
                    break;
                case Key.A:
                    operationsGrafic.RotateElement(45, activeel, currentDrawElement);
                    break;
                case Key.D:
                    operationsGrafic.RotateElement(-45, activeel, currentDrawElement);
                    break;
                case Key.Delete:
                    operationsGrafic.DeleteActiveObejct(activeel, currentDrawElement, draw_canvas);
                    break;
                case Key.Left:
                    //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftCtrl))
                    //{
                    //    /*if (Keyboard.IsKeyDown(Key.LeftShift))
                    //        operationsGrafic.ActiveObejctModify(-OperationsGrafic.CurrentScroll * step_key, 0, activeel, false);
                    //    else*/ operationsGrafic.ActiveObejctModify(-StepValue, 0, activeel, false); 
                    //}
                    //else
                    //    operationsGrafic.ActiveObejctModify(-OperationsGrafic.CurrentScroll, 0, activeel, false);
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        step.Value--;
                    }
                    else
                    {
                        operationsGrafic.ActiveObejctModify(-StepValue, 0, activeel, false);
                    }
                    break;
                case Key.Right:
                    //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftCtrl))
                    //{
                    //    if (Keyboard.IsKeyDown(Key.LeftShift))
                    //        operationsGrafic.ActiveObejctModify(OperationsGrafic.CurrentScroll * step_key, 0, activeel, false);
                    //    else operationsGrafic.ActiveObejctModify(1, 0, activeel, false);
                    //}
                    //else operationsGrafic.ActiveObejctModify(OperationsGrafic.CurrentScroll, 0, activeel, false);
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        step.Value++;
                    }
                    else
                    {
                        operationsGrafic.ActiveObejctModify(StepValue, 0, activeel, false); 
                    }
                    
                    break;
                case Key.Up:
                    //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftCtrl))
                    //{
                    //    if (Keyboard.IsKeyDown(Key.LeftShift))
                    //        operationsGrafic.ActiveObejctModify(0, -OperationsGrafic.CurrentScroll * step_key, activeel, false);
                    //    else operationsGrafic.ActiveObejctModify(0, -1, activeel, false);
                    //}
                    //else operationsGrafic.ActiveObejctModify(0, -OperationsGrafic.CurrentScroll, activeel, false);
                    operationsGrafic.ActiveObejctModify(0, -StepValue, activeel, false); 
                    break;
                case Key.Down:
                    //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftCtrl))
                    //{
                    //    if (Keyboard.IsKeyDown(Key.LeftShift))
                    //        operationsGrafic.ActiveObejctModify(0, OperationsGrafic.CurrentScroll * step_key, activeel, false);
                    //    else operationsGrafic.ActiveObejctModify(0, 1, activeel, false);
                    //}
                    //else operationsGrafic.ActiveObejctModify(0, OperationsGrafic.CurrentScroll, activeel, false);
                    operationsGrafic.ActiveObejctModify(0, StepValue, activeel, false); 
                    break;
                case Key.S:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        SaveOperation(false);
                    break;
                case Key.O:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        OpenOperation();
                    break;
              case Key.Z:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        operationsGrafic.Setback(toolspanel, toolspanelcommand, draw_canvas, lineramkadraw, currentDrawElement, activeel, groupTransform);
                    break;

                case Key.C:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        operationsGrafic.Copy(draw_canvas, activeel);
                    break;
                case Key.V:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        copy = !copy;
                        operationsGrafic.Paste(activeel, currentDrawElement, draw_canvas, toolspanel);
                    }
                    break;
            }
        }

        private void EventWindowStateChanged()
        {
            switch (window.WindowState)
            {
                case System.Windows.WindowState.Maximized:
                    if (menuItems["SettingsStation"].IsChecked)
                        toolspanel.Show();
                    if (menuItems["SettingsCommand"].IsChecked)
                        toolspanelcommand.Show();
                    if (menuItems["SettingsFigure"].IsChecked)
                        toolsetingsfigure.Show();
                    break;
                case System.Windows.WindowState.Minimized:
                    toolspanel.Hide();
                    toolspanelcommand.Hide();
                    toolsetingsfigure.Hide();
                    break;
                case System.Windows.WindowState.Normal:
                    if (menuItems["SettingsStation"].IsChecked)
                        toolspanel.Show();
                    if (menuItems["SettingsCommand"].IsChecked)
                        toolspanelcommand.Show();
                    if (menuItems["SettingsFigure"].IsChecked)
                        toolsetingsfigure.Show();
                    break;
            }
        }

        private void EventWindowKeyUp(KeyEventArgs eventInfo)
        {
            if (activeel.Count > 0)
            {
                if (eventInfo.Key == Key.Left || eventInfo.Key == Key.Right || eventInfo.Key == Key.Up || eventInfo.Key == Key.Down)
                {
                    window.Cursor = Cursors.Arrow;
                    //добавляем историю перемещения объектов
                    if (operationsGrafic.Update)
                    {
                        operationsGrafic.AddHistory(currentDrawElement);
                        operationsGrafic.Update = !operationsGrafic.Update;
                    }
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            EventMouseDown(e);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            EventMouseMove(e);
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            EventMouseWhel(e);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            EventMouseUp(e);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EventWindowClosing(e);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;
            //
            FirstSettings();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            toolspanel.Topmost = true;
            toolspanelcommand.Topmost = true;
            toolsetingsfigure.Topmost = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            toolspanel.Topmost = false;
            toolspanelcommand.Topmost = false;
            toolsetingsfigure.Topmost = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            EventWindowKeyDown(e);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            EventWindowStateChanged();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            EventWindowKeyUp(e);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            EventMenuItemsClick(e);
        }
    }
}
