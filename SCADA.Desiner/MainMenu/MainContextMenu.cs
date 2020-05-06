using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.BaseElement;
using SCADA.Desiner.Enums;
using SCADA.Desiner.Delegate;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.GeometryTransform;
using SCADA.Desiner.WindowWork;
using SCADA.Desiner.WorkForms;
using SCADA.Common.SaveElement;

namespace SCADA.Desiner.MainMenu
{

    class MainContextMenu
    {

        #region События 

        public event Delete Delete;
        public event Delete OriginalSize;
        public event Rotate Rotate;
        public event Reverse Reverse;
        public event NewColor NewColor;
        public event NewWeight NewWeight;
        public event IsFillInside IsFillInside;
        public event NewScroll NewScroll;
        public event NewSize NewSize;
        public event NewSize NewAligment;
        public event NewScroll NewFontSize;
        public event NewFamilyFont NewFamilyFont;
        public event NewSourceForArea NewSource;
        public event NewLayer NewLayer;
        public event HatchLine HatchLine;

        #endregion

        #region Переменные
        /// <summary>
        /// коллекция графических элементов
        /// </summary>
        Dictionary<int, IGraficObejct> _collectionElement;
        /// <summary>
        /// передаем список выделенных элементов
        /// </summary>
        public event SpisokId SpisokId;
        /// <summary>
        /// выбранный номер станции
        /// </summary>
        double _selectStationnumber;
        
       
        bool isVisiblityHeight = true;
        /// <summary>
        /// толщина линий
        /// </summary>
        double _weight = 1;
        /// <summary>
        /// ориентация симметричного переноса
        /// </summary>
        AligmentMirror _mirror;
        IGraficObejct _elementselect;
        WorkGrafic _mainwindow;

        #endregion

        public MainContextMenu(Dictionary<int,IGraficObejct>  elements, double stationnumber, IGraficObejct element, WorkGrafic mainwindow)
        {
            _collectionElement = elements;
            _selectStationnumber = stationnumber;
            _elementselect = element;
            _mainwindow = mainwindow;
        }

        public ContextMenu GetContextMenu()
        {
            ContextMenu result = new ContextMenu();
            result.Background = Brushes.LightGray;
            result.BorderThickness = new Thickness(2);
            result.BorderBrush = Brushes.Brown;

            if (_mainwindow.ActiveEl.Count == 1)
            {
                //Меню показать все элементы данной станции
                MenuItem itemelementstationall = new MenuItem();
                itemelementstationall.Header = "Меню показать все элементы данной станции";
                itemelementstationall.Click += AllElementStationControlClick;
                result.Items.Add(itemelementstationall);
                //Создать шаблон проекта станции
                MenuItem itemelementcreateproject = new MenuItem();
                itemelementcreateproject.Header = "Создать шаблон проекта станции";
                itemelementcreateproject.Click += CreateProjectControlClick;
                result.Items.Add(itemelementcreateproject);
                //Меню показать реверса линии перегона
                switch (_mainwindow.ActiveEl[_mainwindow.ActiveEl.Count - 1].ViewElement)
                {
                    case Common.Enums.ViewElement.otrezok:
                        {
                            MenuItem itemelementreverse = new MenuItem();
                            itemelementreverse.Header = "Реверсирование координат";
                            itemelementreverse.Click += ReverseControlClick;
                            result.Items.Add(itemelementreverse);
                        }
                        break;
                    case Common.Enums.ViewElement.line:
                        {
                            MenuItem itemlinehatch = new MenuItem();
                            itemlinehatch.Header = "Штриховая линия";
                            itemlinehatch.Click += HatchLineClick;
                            result.Items.Add(itemlinehatch);
                        }
                        break;
                }
            }
            if (_mainwindow.ActiveEl.Count > 0)
            {
                ////Меню удалить объекты
                MenuItem itemelementsdelete = new MenuItem();
                itemelementsdelete.Header = "Удалить объекты";
                itemelementsdelete.Click += ElementsdeleteControlClick;
                result.Items.Add(itemelementsdelete);
                ////Меню повернуть влево
                MenuItem itemrotateleft = new MenuItem();
                itemrotateleft.Header = "Повернуть влево";
                itemrotateleft.Click += RotateLeftClick;
                result.Items.Add(itemrotateleft);
                ////Меню повернуть вправо
                MenuItem itemrotateright = new MenuItem();
                itemrotateright.Header = "Повернуть вправо";
                itemrotateright.Click += RotateRightClick;
                result.Items.Add(itemrotateright);
                if (YesRotate())
                {
                    ////Меню повернуть на произвольный угол
                    MenuItem item_free_rotate = new MenuItem();
                    item_free_rotate.Header = "Повернуть на угол";
                    item_free_rotate.Click += FreeRotateClick;
                    result.Items.Add(item_free_rotate);
                }
                //провереям есть ли в буфере элементы выравнивания 
                if (_elementselect != null)
                {
                    MenuItem item_aligment = new MenuItem();
                    item_aligment.Header = "Выравнять";
                    item_aligment.Click += AligmentClick;
                    result.Items.Add(item_aligment);
                }
                //провереям есть ли в буфере элемент относительно которого производить симметричный поворот 
                if (YesMirrorElements())
                {
                    MenuItem item_mirror = new MenuItem();
                    item_mirror.Header = "Симметричный перенос";
                    item_mirror.Click += MirrorClick;
                    result.Items.Add(item_mirror);
                }
            }
            //проверяем есть область вставки картинки
            if (IsAreaPicture())
            {
                MenuItem itemenewcolor = new MenuItem();
                itemenewcolor.Header = "Добавить картинку";
                itemenewcolor.Click += AddPicture;
                result.Items.Add(itemenewcolor);
            }
            if (IsAreaBrowser())
            {
                MenuItem itemenewcolor = new MenuItem();
                itemenewcolor.Header = "Установить страницу";
                itemenewcolor.Click += AddLink;
                result.Items.Add(itemenewcolor);
            }
            //провереям все ли в выделенных элементах вспомагательные линии
            if (YesHelpLine())
            {
                var itemenewcolor = new MenuItem();
                itemenewcolor.Header = "Изменить цвет вспомагательной линии";
                itemenewcolor.Click += NewColorControlClick;
                result.Items.Add(itemenewcolor);
                //
                var itemeneweight = new MenuItem();
                itemeneweight.Header = "Изменить толщину вспомагательной линии";
                itemeneweight.Click += NewWeightrControlClick;
                result.Items.Add(itemeneweight);
                //
                var itemeIsFill = new MenuItem();
                itemeIsFill.Header = "Заливать внутри";
                itemeIsFill.Click += IsFillInsideControlClick;
                result.Items.Add(itemeIsFill);
            }
            //провереям все ли в выделенных элементах для масштабирования
            if (YesScrollObejct())
            {
                MenuItem itemescroll_arrowmove = new MenuItem();
                itemescroll_arrowmove.Header = "Изменить масштаб";
                itemescroll_arrowmove.Click += NewScrollClick;
                result.Items.Add(itemescroll_arrowmove);
            }
            //провереям есть среди выделенных элементов пригодные для изменения размеров
            if (YesResizeObejct())
            {
                MenuItem itemesize_arrowmove = new MenuItem();
                itemesize_arrowmove.Header = "Изменить размеры";
                itemesize_arrowmove.Click += NewSizeClick;
                result.Items.Add(itemesize_arrowmove);
            }
            //провереям все ли в выделенных элементах (часы, справочный текст и т.д.)
            if (YesTextElement())
            {
                MenuItem itemetime = new MenuItem();
                itemetime.Header = "Изменить размер шрифта";
                itemetime.Click += NewSizeFontClick;
                result.Items.Add(itemetime);
            }
            //опция для изменения слоя вниз
            {
                MenuItem items_before_view = new MenuItem();
                items_before_view.Header = "На передний план";
                {
                    MenuItem items_before_view_all = new MenuItem();
                    items_before_view_all.Header = "На передний план (общий)";
                    items_before_view_all.Click += NewLayerTopAllClick;
                    items_before_view.Items.Add(items_before_view_all);
                    //
                    MenuItem items_before_view_step = new MenuItem();
                    items_before_view_step.Header = "На передний план (1 шаг)";
                    items_before_view_step.Click += NewLayerTopStepClick;
                    items_before_view.Items.Add(items_before_view_step);
                }
                result.Items.Add(items_before_view);
            }
            //опция для изменения слоя вверх
            {
                MenuItem item_back_view = new MenuItem();
                item_back_view.Header = "На задний план";
                {
                    MenuItem item_back_view_all = new MenuItem();
                    item_back_view_all.Header = "На задний план (общий)";
                    item_back_view_all.Click += NewLayerBottomAllClick;
                    item_back_view.Items.Add(item_back_view_all);
                    //
                    MenuItem item_back_view_step = new MenuItem();
                    item_back_view_step.Header = "На задний план (1 шаг)";
                    item_back_view_step.Click += NewLayerBottomStepClick;
                    item_back_view.Items.Add(item_back_view_step);
                }
                result.Items.Add(item_back_view);
            }
            ////Меню исходный размер
            MenuItem itemeoriginalsize = new MenuItem();
            itemeoriginalsize.Header = "Исходный размер";
            itemeoriginalsize.Click += OriginalSizeControlClick;
            result.Items.Add(itemeoriginalsize);
            //
            if (result.Items.Count > 0)
                return result;
            else return null;
        }

        private bool YesRotate()
        {
            foreach (IGraficObejct el in _mainwindow.ActiveEl)
            {
                if (el is IFreeAngle)
                    return true;
            }
            //
            return false;
        }

        /// <summary>
        /// проверяем есть ли область вставки картинки
        /// </summary>
        /// <returns></returns>
        private bool IsAreaPicture()
        {
            foreach (IGraficObejct el in _mainwindow.ActiveEl)
            {
                if ((el is Area) && (el as Area).View == Common.Enums.ViewArea.area_picture)
                {
                    return true;
                }
            }
            //
            return false;
        }

        /// <summary>
        /// проверяем есть ли область браузера
        /// </summary>
        /// <returns></returns>
        private bool IsAreaBrowser()
        {
            foreach (IGraficObejct el in _mainwindow.ActiveEl)
            {
                if ((el is Area) && (el as Area).View == Common.Enums.ViewArea.webBrowser)
                {
                    return true;
                }
            }
            //
            return false;
        }

        private bool YesHelpLine()
        {
            int index = 0;
            //
            foreach (IGraficObejct el in _mainwindow.ActiveEl)
            {
                if (el is LineHelp)
                {
                    double buffer = Math.Round((el as LineHelp).StrokeThickness);
                    if (index == 0)
                        _weight = buffer;
                    else
                    {
                        if (_weight != buffer)
                        {
                            _weight = 1;
                            return true;
                        }
                    }
                    index++;
                }
            }
            //
            if (_weight < 1)
                _weight = 1;
            //
            if (index > 0)
                return true;
            //
            return false;
        }

        private bool YesScrollObejct()
        {
            foreach (IGraficObejct el in _mainwindow.ActiveEl)
            {
                if (el is IScrollElement)
                    return true;
            }
            //
            return false;
        }

        private bool YesResizeObejct()
        {
            int index = 0;
            bool answer = false;
            foreach (IGraficObejct el in _mainwindow.ActiveEl)
            {
                if (el is IResize)
                {
                    answer = true;
                    if (el is NumberTrain)
                        index++;
                }
            }
            if (index == _mainwindow.ActiveEl.Count)
                isVisiblityHeight = false;
            //
            return answer;
        }

        private bool YesMirrorElements()
        {
            if (_elementselect != null && (_elementselect is LineHelp))
            {
                LineHelp line_mirror = _elementselect as LineHelp;
                foreach (PathFigure geo in line_mirror.Figure.Figures)
                {
                    double point = 0;
                    int index = 0;
                    for (int i =0; i< geo.Segments.Count; i++)
                    {  
                        //сегмент линия
                        LineSegment lin = geo.Segments[i] as LineSegment;
                        if (lin != null)
                        {
                            if (lin.Point.X == geo.StartPoint.X || lin.Point.Y == geo.StartPoint.Y)
                            {
                                if (i == 0)
                                {
                                    if (lin.Point.X == geo.StartPoint.X)
                                    {
                                        point = geo.StartPoint.X;
                                        _mirror = AligmentMirror.horizontal;
                                    }
                                    else
                                    {
                                        point = geo.StartPoint.Y;
                                        _mirror = AligmentMirror.vertical;
                                    }
                                }
                                else
                                {
                                    if (point != geo.StartPoint.X && point != geo.StartPoint.Y)
                                        return false;
                                }
                                
                            }
                            else
                                return false;
                            index++;
                        }   
                    }
                    if (index == geo.Segments.Count)
                        return true;
                }
            }
            //
            return false;
        }

        private bool YesTextElement()
        {
            foreach (IGraficObejct el in _mainwindow.ActiveEl)
            {
                if (el is IFontSize)
                    return true;
            }
            //
            return false;
        }
      
        private void AllElementStationControlClick(object sender, RoutedEventArgs args)
        {
            List<int> _idcollection = new List<int>();
            foreach (KeyValuePair<int, IGraficObejct> el in _collectionElement)
            {
                if (el.Value != null && el.Value.StationNumber == _selectStationnumber && !(el.Value is ArrowMove) && !(el.Value is NumberTrain) && !(el.Value is LinePeregon))
                    _idcollection.Add(el.Value.Id);
            }
            //
            if (_idcollection.Count > 1 && SpisokId != null)
                SpisokId(_idcollection, null);
        }

        private void CreateProjectControlClick(object sender, RoutedEventArgs args)
        {
            List<int> _idcollection = new List<int>();
            StationProject selectstation = new StationProject();
            //
            foreach (KeyValuePair<int, IGraficObejct> el in _collectionElement)
            {
                if (el.Value != null && el.Value.StationNumber == _selectStationnumber && !(el.Value is ArrowMove) && !(el.Value is NumberTrain) && !(el.Value is LinePeregon) && !(el.Value is LineHelp) && !(el.Value is Move) && !(el.Value is KGU) && !(el.Value is KTCM))
                {
                    _idcollection.Add(el.Value.Id);
                    //
                    if(el.Value is ButtonStation)
                        selectstation.ButtonStation.Add(el.Value);
                    //
                    if (el.Value is Traintrack)
                        selectstation.Path.Add(el.Value);
                    //
                    if (el.Value is Signal)
                        selectstation.Signal.Add(el.Value);
                }
            }
            //
            if (_idcollection.Count > 1 && SpisokId != null)
                SpisokId(_idcollection, null);
            //создаем шаблон пректа станции
            System.Windows.Forms.SaveFileDialog saveFileDial = new System.Windows.Forms.SaveFileDialog();
            saveFileDial.Filter = "CSV files (*.csv)|*.csv";
            if (saveFileDial.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectstation.FileName = saveFileDial.FileName; 
                File.Create(saveFileDial.FileName).Close(); 
                ThreadPool.QueueUserWorkItem(new WaitCallback(CreateProjectStation), selectstation);
            }
           // 
        }

        private void CreateProjectStation(object sender)
        {
            StationProject project = sender as StationProject;
            if (project != null && !string.IsNullOrEmpty(project.FileName))
            {
                try
                {
                    List<string> info = new List<string>();
                    info.Add("#;ИМЯ;ПАРАМЕТР;КОНТРОЛЬ");
                    //станционный контроль
                    if (project.ButtonStation.Count >= 0)
                    {
                        info.Add("#СТАНЦИОННЫЙ КОНТРОЛЬ;;;");
                        foreach (ButtonStation buttonstation in project.ButtonStation)
                        {
                            info.Add(string.Format("1;{0};{1};{2}", buttonstation.NameObject, "Сезонное управление", "ВСУ"));
                            info.Add(string.Format("1;{0};{1};{2}", buttonstation.NameObject, "Передача на сезонное управление", "КСУМ"));
                            info.Add(string.Format("1;{0};{1};{2}", buttonstation.NameObject, "Резервное управление", "КВРУ"));
                            info.Add(string.Format("1;{0};{1};{2}", buttonstation.NameObject, "Диспетчерское управление", "!КСУ*!КВРУ"));
                            info.Add(string.Format("1;{0};{1};{2}", buttonstation.NameObject, "Нет связи со станцией", "~ТЕСТ"));
                            info.Add(string.Format("1;{0};{1};", buttonstation.NameObject, "Авария"));
                            info.Add(string.Format("1;{0};{1};{2}", buttonstation.NameObject, "Неисправность", "ПОН"));
                            info.Add(string.Format("1;{0};{1};{2}", buttonstation.NameObject, "Пожар", "ПОТ"));
                            info.Add("---------------------------------------------------------------------------------;;;");
                        }
                    }
                    //маршрут
                    if (project.ButtonStation.Count >= 0)
                    {
                        info.Add("#МАРШРУТ;;;");
                        foreach (Signal signal in project.Signal)
                        {
                            info.Add(string.Format("2;{0};{1};", signal.NameObject, "Проезд"));
                            info.Add(string.Format("2;{0};{1};{2}", signal.NameObject, "Сигнал", string.Format("{0}С", signal.NameObject)));
                            info.Add(string.Format("2;{0};{1};", signal.NameObject, "Пригласительный"));
                            info.Add(string.Format("2;{0};{1};", signal.NameObject, "Занятие"));
                            info.Add(string.Format("2;{0};{1};", signal.NameObject, "Замыкание"));
                            info.Add(string.Format("2;{0};{1};", signal.NameObject, "Установка"));
                            info.Add(string.Format("2;{0};{1};", signal.NameObject, "Неисправность"));
                            info.Add(string.Format("2;{0};{1};", signal.NameObject, "Маневровый"));
                            info.Add("---------------------------------------------------------------------------------;;;");
                        }
                    }
                    //путь
                    if (project.ButtonStation.Count >= 0)
                    {
                        info.Add("#ПУТИ;;;");
                        foreach (Traintrack path in project.Path)
                        {
                            info.Add(string.Format("3;{0};{1};{2}", path.NameObject, "Занятие", string.Format("{0}П",path.NameObject)));
                            info.Add(string.Format("3;{0};{1};", path.NameObject, "Авто действие"));
                            info.Add(string.Format("3;{0};{1};", path.NameObject, "Электрификация"));
                            info.Add(string.Format("3;{0};{1};", path.NameObject, "Ограждение"));
                            info.Add("---------------------------------------------------------------------------------;;;");
                        }
                    }
                    //записываем в файл
                    File.WriteAllLines(project.FileName, info.ToArray(), Encoding.GetEncoding(1251));

                }
                catch { }
            }
        }

        private double CommonScroll()
        {
            bool common = true;
            double scroll = -1;
            foreach (IGraficObejct graf in _mainwindow.ActiveEl)
            {
                if (graf is IScrollElement)
                {
                    if (scroll == -1)
                        scroll = (graf as IScrollElement).CurrencyScroll;
                    else
                    {
                        if (scroll != (graf as IScrollElement).CurrencyScroll)
                        {
                            common = false;
                            break;
                        }
                    }
                }
            }
            //
            if (common)
                return scroll;
            else
            {
                if (_mainwindow.ActiveEl.Count == 1)
                    return (_mainwindow.ActiveEl[_mainwindow.ActiveEl.Count - 1] as IScrollElement).CurrencyScroll;
                else return 1;
            }
        }

        private double CommonFontSize()
        {
            double fontsize = 0;
            int index = 0;
            foreach (IGraficObejct graf in _mainwindow.ActiveEl)
            {
                if (graf is IFontSize)
                {
                    if (index == 0)
                    {
                        fontsize = (graf as IFontSize).Text.FontSize;
                    }
                    else
                    {
                        if (fontsize != (graf as IFontSize).Text.FontSize)
                            return 0;
                    }
                    index++;
                }
            }
            //
            return Math.Round(fontsize, 1);
        }

        private void ReverseControlClick(object sender, RoutedEventArgs args)
        {
            if (Reverse != null)
                Reverse();
        }

        private void HatchLineClick(object sender, RoutedEventArgs args)
        {
            HatchLineForm hatchform = new HatchLineForm((_mainwindow.ActiveEl[_mainwindow.ActiveEl.Count - 1] as LineHelp).StrokeDashArray);
            if (hatchform.ShowDialog().Value)
            {
                if (HatchLine != null)
                    HatchLine(hatchform.ArrayHatch);
            }
        }

        private void ElementsdeleteControlClick(object sender, RoutedEventArgs args)
        {
            if (Delete != null)
                Delete();
        }

        private void OriginalSizeControlClick(object sender, RoutedEventArgs args)
        {
            if (OriginalSize != null)
                OriginalSize();
        }

        private void AddPicture(object sender, RoutedEventArgs args)
        {
            System.Windows.Forms.OpenFileDialog openDialog = new System.Windows.Forms.OpenFileDialog() { Multiselect = false, Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF" };
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (NewSource != null)
                    NewSource(openDialog.FileName, Common.Enums.ViewArea.area_picture);
            }
        }

        private void AddLink(object sender, RoutedEventArgs args)
        {
            System.Windows.Forms.OpenFileDialog openDialog = new System.Windows.Forms.OpenFileDialog() { Multiselect = false, Filter = "Image Files(*.HTML;*.PDF)|*.HTML;*.PDF" };
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (NewSource != null)
                    NewSource(openDialog.FileName, Common.Enums.ViewArea.webBrowser);
            }
        }

        private void NewColorControlClick(object sender, RoutedEventArgs args)
        {
            //System.Windows.Forms.ColorDialog dialogcolor = new System.Windows.Forms.ColorDialog();
            //if (dialogcolor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //if (NewColor != null)
            //    NewColor(dialogcolor.Color.R, dialogcolor.Color.G, dialogcolor.Color.B);
            //}
            NewColorForm colorform = new NewColorForm(GetOneColor());
            colorform.ShowDialog();
            if (colorform.NameColor.NameColor != string.Empty)
            {
                if (NewColor != null)
                    NewColor(colorform.NameColor);
            }
        }

        private void NewWeightrControlClick(object sender, RoutedEventArgs args)
        {
            NewSizeElementWpf dialogsize = new NewSizeElementWpf(_weight, ViewObject.weight);
            if ((bool)dialogsize.ShowDialog())
            {
                if (NewWeight != null)
                    NewWeight(dialogsize.Scroll);
            }
        }

        private void IsFillInsideControlClick(object sender, RoutedEventArgs args)
        {
            if (IsFillInside != null)
                IsFillInside();
        }

        private void NewScrollClick(object sender, RoutedEventArgs args)
        {
            NewSizeElementWpf dialogsize = new NewSizeElementWpf(CommonScroll(), ViewObject.objectgrafic);
            if ((bool)dialogsize.ShowDialog())
            {
                if (NewScroll != null)
                    NewScroll(dialogsize.Scroll);
            }
        }

        private void NewSizeClick(object sender, RoutedEventArgs args)
        {
            NewWidthHeightElement dialogsize = new NewWidthHeightElement(isVisiblityHeight, "Ширина:", " Высота:");
            if ((bool)dialogsize.ShowDialog())
            {
                if (NewSize != null)
                    NewSize(dialogsize.XElement, dialogsize.YElement);
            }
        }

        private void AligmentClick(object sender, RoutedEventArgs args)
        {
            NewWidthHeightElement dialogsize = new NewWidthHeightElement(isVisiblityHeight, "Шаг X:", "Шаг Y:");
            if ((bool)dialogsize.ShowDialog())
            {
                if (NewAligment != null)
                    NewAligment(dialogsize.XElement, dialogsize.YElement);
            }
        }

        private void MirrorClick(object sender, RoutedEventArgs args)
        {
            LineHelp line_mirror = _elementselect as LineHelp;
            foreach (IGraficObejct element in _mainwindow.ActiveEl)
            {
                element.Mirror(_mirror, line_mirror.Figure.Figures[0].StartPoint);
            }
            //
            if (_mainwindow != null)
                _mainwindow.EventAddHistoty();
        }

        private void NewSizeFontClick(object sender, RoutedEventArgs args)
        {
            NewSizeElementWpf dialogsize = null;
            //if (_mainwindow.ActiveEl.Count == 1)
            //    dialogsize = new NewSizeElementWpf((_mainwindow.ActiveEl[_mainwindow.ActiveEl.Count - 1] as IFontSize).Text.FontSize, ViewObject.text);
            //else dialogsize = new NewSizeElementWpf(CommonScroll(), ViewObject.objectgrafic);
            dialogsize = new NewSizeElementWpf(CommonFontSize(), ViewObject.text);
            if ((bool)dialogsize.ShowDialog())
            {
                if (NewFontSize != null)
                    NewFontSize(dialogsize.Scroll);
            }
        }

        private void NewLayerBottomStepClick(object sender, RoutedEventArgs args)
        {
            if (NewLayer != null)
                NewLayer(ViewLayer.bottom, 0);
        }

        private void NewLayerTopStepClick(object sender, RoutedEventArgs args)
        {
            if (NewLayer != null)
                NewLayer(ViewLayer.top, 0);
        }

        private void NewLayerBottomAllClick(object sender, RoutedEventArgs args)
        {
            int min = int.MaxValue;
            foreach (KeyValuePair<int, IGraficObejct> el in _collectionElement)
            {
                if (el.Value.ZIndex < min)
                    min = el.Value.ZIndex;
            }
            if (NewLayer != null)
                NewLayer(ViewLayer.setvalue, min -1);
        }

        private void NewLayerTopAllClick(object sender, RoutedEventArgs args)
        {
            int max = int.MinValue;
            foreach (KeyValuePair<int, IGraficObejct> el in _collectionElement)
            {
                if (el.Value.ZIndex > max)
                    max = el.Value.ZIndex;
            }
            if (NewLayer != null)
                NewLayer(ViewLayer.setvalue, max + 1);
        }

        private void NewFamilyFontTimeClick(object sender, RoutedEventArgs args)
        {
            System.Windows.Forms.FontDialog fontdialog = new System.Windows.Forms.FontDialog();
            if (fontdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (NewFamilyFont != null)
                    NewFamilyFont(fontdialog.Font.FontFamily.Name);
            }
        }

        private void RotateLeftClick(object sender, RoutedEventArgs args)
        {
            if (Rotate != null)
                Rotate(90);
        }

        private void RotateRightClick(object sender, RoutedEventArgs args)
        {
            if (Rotate != null)
                Rotate(-90);
        }

        private void FreeRotateClick(object sender, RoutedEventArgs args)
        {
            SetAngle dialogangle = new SetAngle(_mainwindow);
            dialogangle.ShowDialog();
            //if ((bool)dialogangle.ShowDialog())
            //{
                
            //}
        }

        /// <summary>
        /// Возвращаем общее название цвета
        /// </summary>
        /// <returns></returns>
        private NameColors GetOneColor()
        {
            NameColors name = new NameColors() { NameColor = string.Empty};
            foreach (IGraficObejct graf in _mainwindow.ActiveEl)
            {
                if (graf is LineHelp)
                {
                    LineHelp line = graf as LineHelp;
                    if (line.NameViewColor.NameColor != name.NameColor && name.NameColor != string.Empty)
                        return new NameColors() { NameColor = string.Empty };
                    //
                    name.NameColor = line.NameViewColor.NameColor;
                    name.R = line.NameViewColor.R;
                    name.G = line.NameViewColor.G;
                    name.B = line.NameViewColor.B;
                }
            }
            return name;
        }
    }
}
