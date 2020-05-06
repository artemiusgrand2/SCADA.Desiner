using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.BaseElement;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.Enums;
using SCADA.Desiner.Delegate;
using SCADA.Desiner.GeometryTransform;
using SCADA.Common.SaveElement;
using SCADA.Common.Enums;

namespace SCADA.Desiner.Tools
{
    /// <summary>
    /// Interaction logic for ToolsGraphics.xaml
    /// </summary>
    public partial class ToolsGraphics : Window
    {
        #region Переменные и свойства

        /// <summary>
        /// рисовать ли линии ортоганальными
        /// </summary>
        public bool Orta = false;
        ObservableCollection<MyTable> collection = null;
        /// <summary>
        /// событие на добавление нового елемента
        /// </summary>
        public event addelement EventAdd;
        /// <summary>
        /// передаем список выделенных элементов
        /// </summary>
        public event SpisokId SpisokId;
        /// <summary>
        /// событие на добавление или удаления сетки
        /// </summary>
        public event visiblesetka EventSetka;
        /// <summary>
        /// событие вызываемое при изменениии свойств объекта
        /// </summary>
        public event updateelement UpdateElement;
        /// <summary>
        /// событие на привидение картинки в исходный режи
        /// </summary>
        public event addelement EventOriginalSize;
        /// <summary>
        /// текущий выбранный номер станции
        /// </summary>
        public int CurrentNumberStation { get; set; }
        /// <summary>
        /// текущий выбранная станция (ее название)
        /// </summary>
        public string CurrentNameStation { get; set; }
        private bool _closing = false;
        /// <summary>
        /// группа выделенных элементов
        /// </summary>
        List<IGraficObejct> _selectelement = new List<IGraficObejct>();
        //флаг закрыттия панели
        public bool ClosingTools
        {
            get
            {
                return _closing;
            }
            set
            {
                _closing = value;
            }
        }
        /// <summary>
        /// текущий элемент для рисования
        /// </summary>
        private ViewElement m_currentelemantdraw = ViewElement.none;
        public ViewElement CurrentDraw
        {
            get
            {
                return m_currentelemantdraw;
            }
            set
            {
                m_currentelemantdraw = value;
            }
        }
        /// <summary>
        /// коллекция проектируемых станций
        /// </summary>
        List<Station> _stations = new List<Station>();

        #endregion

        public ToolsGraphics(Settings start_settings)
        {
            InitializeComponent();
            Start(start_settings);
        }

        private void Start(Settings start_settings)
        {
            if (start_settings != null)
            {
                if (start_settings.StartCommand_Width > 0)
                    Width = start_settings.StartGrafic_Widht;
                if (start_settings.StartGrafic_Height > 0)
                    Height = start_settings.StartGrafic_Height;
                //
                Left = start_settings.StartGrafic_X;
                Top = start_settings.StartGrafic_Y;
            }
            else
            {
                Left = WindowWork.WorkGrafic.width - this.Width;
                Top = 24;
            }
            _closing = true;
            //
            foreach (string row in WindowWork.WorkGrafic.TypeDisconnectorsDic.Keys)
                Combox_TypeObject.Items.Add(row);
            CurrentNumberStation = SCADA.Common.Constant.CommonConstant.defultStationNumber;
            _stations = GetStationNumberName.GetCollectionStation();
            FullcomboxNumberStation(_stations);
            collection = new ObservableCollection<MyTable>();
            TableObject.ItemsSource = collection;
            Numberstationleft_textbox.TextChanged += Numberstationleft_textbox_TextChanged;
            Numberrightstation_textbox.TextChanged += Numberstationright_textbox_TextChanged;
            OperationsGrafic.AddObject += AddObject;
            OperationsGrafic.RemoveObject += RemoveObject;
            OperationsGrafic.UpdateObject += UpdateObject;
            OperationsGrafic.SelectObject += SelectObject;
        }

        private void UpdateTime(int countTime)
        {
            if (countTime > 0)
                button_time.IsEnabled = false;
            else button_time.IsEnabled = true;

        }

        /// <summary>
        /// реагируем на добавление нового объекта
        /// </summary>
        /// <param name="addcollection">список объектов для добавления</param>
        private void AddObject(List<IGraficObejct> addcollection)
        {
            if (addcollection != null)
            {
                foreach (IGraficObejct row in addcollection)
                    collection.Add(new MyTable()
                    {
                        Numberstationleft = string.Format("{0:D6}", (int)row.StationNumber),
                        Name = row.NameObject,
                        Id = row.Id,
                        Notes = row.Notes,
                        Numberstationright = string.Format("{0:D6}", (int)row.StationNumberRight)
                    });
            }
        }
        /// <summary>
        /// реагируем на удаление объекта
        /// </summary>
        /// <param name="removecollection">список объектов для удаления</param>
        private void RemoveObject(List<IGraficObejct> removecollection)
        {
            try
            {
                foreach (IGraficObejct remobject in removecollection)
                {
                    MyTable answer = FindObject(remobject.Id);
                    if (answer != null)
                        collection.Remove(answer);
                }
            }
            catch { }
        }
        /// <summary>
        /// обновление списка объектов
        /// </summary>
        /// <param name="updatecollection">список объектов для редактирования</param>
        private void UpdateObject(List<IGraficObejct> updatecollection)
        {
            collection.Clear();
            AddObject(updatecollection);
        }
        /// <summary>
        /// выделяем нужные объекты
        /// </summary>
        /// <param name="selectcollection"></param>
        private void SelectObject(List<IGraficObejct> selectcollection)
        {
            _selectelement.Clear();
            Empty();
            try
            {
                ViewElements(selectcollection);
                if (selectcollection.Count > 0)
                {
                    foreach (IGraficObejct el in selectcollection)
                    {
                        _selectelement.Add(el);
                        MyTable row = FindObject(el.Id);
                        if (row != null)
                            row.IsSelect = true;
                    }
                    //проверяем наличие совпадений в группе элементов
                    bool text = true, notes = true, left = true, right = true, input = true, type = true, isvisible = true, fileClick = true;
                    EqualObject(ref text, ref notes, ref left, ref right, ref fileClick, _selectelement, ref input, ref type, ref isvisible);
                    if (text)
                        Name_textbox.Text = _selectelement[_selectelement.Count - 1].NameObject;
                    if (fileClick)
                        NameFileClick.Content = _selectelement[_selectelement.Count - 1].FileClick;
                    if (notes)
                        Notes_textbox.Text = _selectelement[_selectelement.Count - 1].Notes;
                    if (isvisible)
                        IsVisibleObject.IsChecked = _selectelement[_selectelement.Count - 1].IsVisible;
                    if (left)
                        Numberstationleft_textbox.Text = string.Format("{0:D6}",_selectelement[_selectelement.Count - 1].StationNumber);
                    if (right)
                        Numberrightstation_textbox.Text = string.Format("{0:D6}", _selectelement[_selectelement.Count - 1].StationNumberRight);
                    if (input && _selectelement[_selectelement.Count - 1] is LightTrain)
                        checkBox_inputlight.IsChecked = (_selectelement[_selectelement.Count - 1] as LightTrain).IsInput;
                    if (type && _selectelement[_selectelement.Count - 1] is Disconnectors)
                    {
                        Combox_TypeObject.Text = GetNameType((_selectelement[_selectelement.Count - 1] as Disconnectors).Type);
                        TypeDis.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                TableObject.Items.Refresh();
            }
            catch { }
        }

        /// <summary>
        /// получаем тип разъеденителей
        /// </summary>
        /// <param name="type">тип</param>
        /// <returns></returns>
        private string GetNameType(TypeDisconnectors type)
        {
            foreach (KeyValuePair<string, TypeDisconnectors> row in WindowWork.WorkGrafic.TypeDisconnectorsDic)
            {
                if(row.Value == type)
                    return row.Key;
            }
            //
            return string.Empty;
        }

        public void SelectNotTable()
        {
            TableObject.SelectedIndex = -1;
        }

        private void ViewElements(List<IGraficObejct> selectcollection)
        {
            if (selectcollection.Count == 0)
            {
                //LeftStation.Visibility = System.Windows.Visibility.Collapsed;
                //RightStation.Visibility = System.Windows.Visibility.Collapsed;
                checkBox_inputlight.Visibility = System.Windows.Visibility.Collapsed;
                TypeDis.Visibility = System.Windows.Visibility.Collapsed;
                SettingsAnalogCells.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                var checkbox_input = false;
                var selectAnalogCells = false;
                var right = false;
                var dis = false;
                //LeftStation.Visibility = System.Windows.Visibility.Visible;
                //RightStation.Visibility = System.Windows.Visibility.Visible;
                //
                foreach (IGraficObejct el in selectcollection)
                {
                    //if (!(el is NumberTrain) &&
                    //!(el is LinePeregon) &&
                    //!(el is LineHelp) &&
                    //!(el is Disconnectors) &&
                    //!(el is ArrowMove))
                    //{
                       
                    //    right = true;
                    //}
                    //
                    if (!(el is LightTrain))
                    {
                        checkBox_inputlight.Visibility = System.Windows.Visibility.Collapsed;
                        checkbox_input = true;
                    }
                    //
                    if (!(el is Disconnectors))
                    {
                        TypeDis.Visibility = System.Windows.Visibility.Collapsed;
                        dis = true;
                    }
                    //
                    if (!((el is Traintrack) && (el as Traintrack).View == ViewTrack.analogCell))
                    {
                        SettingsAnalogCells.Visibility = System.Windows.Visibility.Collapsed;
                        selectAnalogCells = true;
                    }
                }
                //
                //if (!right)
                //{
                //    RightStation.Visibility = System.Windows.Visibility.Visible;
                //    label_0.Content = "Ст. контроля";
                //}
                //
                if (!checkbox_input)
                    checkBox_inputlight.Visibility = System.Windows.Visibility.Visible;
                if (!selectAnalogCells)
                    SettingsAnalogCells.Visibility = System.Windows.Visibility.Visible;
                //
                if(!dis)
                    TypeDis.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// поиск объекта
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private MyTable FindObject(int Id)
        {
            foreach (MyTable row in collection)
            {
                if (row.Id == Id)
                    return row;
            }
            return null;
        }

        /// <summary>
        /// по номеру стнации находим ее описание
        /// </summary>
        /// <param name="number">номер станции</param>
        /// <returns></returns>
        private int FindStationNumber(string number)
        {
            for (int i = 0; i < _stations.Count; i++)
            {
                if (_stations[i].NumberStation == number)
                    return i;
            }
            return SCADA.Common.Constant.CommonConstant.defultStationNumber;
        }
        /// <summary>
        /// заполняем перечень используемых станций
        /// </summary>
        /// <param name="stcollection"></param>
        private void FullcomboxNumberStation(List<Station> stcollection)
        {
            foreach (Station el in stcollection)
            {
                comboBox_number.Items.Add(el.NameStation + "     -     " + el.NumberStation);
                Combox_NumberStationLeft.Items.Add(el.NameStation + "     -     " + el.NumberStation);
                Combox_NumberStationRight.Items.Add(el.NameStation + "     -     " + el.NumberStation);
            }
        }

        private void SelectCombox(string numberstation, ComboBox cox)
        {
            int indexStation = FindStationNumber(numberstation);
            if (indexStation != cox.SelectedIndex)
            {
                if (indexStation != -1)
                    cox.SelectedIndex = indexStation;
                else
                    cox.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// проверяем повторяющиеся элементы станции
        /// </summary>
        /// <returns></returns>
        private void EqualObject(ref bool name, ref bool notes, ref bool left, ref bool right, ref bool fileClick, List<IGraficObejct> collection, ref bool input, ref bool typeDisconnector, ref bool isvisible)
        {
            if (collection != null)
            {
                //
                if (collection.Count > 1)
                {
                    var text = collection[0].NameObject;
                    var notestext = collection[0].Notes;
                    var numberleft = collection[0].StationNumber;
                    var numberright = collection[0].StationNumberRight;
                    var visible = collection[0].IsVisible;
                    var IsInput =  (collection[0] is LightTrain) ? (collection[0] as LightTrain).IsInput : input = false;
                    if (!(collection[0] is LightTrain))
                        input = false;
                    var type = (collection[0] is Disconnectors) ? (collection[0] as Disconnectors).Type : TypeDisconnectors.notNormal;
                    if (!(collection[0] is Disconnectors))
                        typeDisconnector = false;
                    var fileClickCompare = collection[0].FileClick;
                    //
                    for (int i = 1; i < collection.Count; i++)
                    {
                        if (collection[i].StationNumber != numberleft)
                            left = false;
                        //
                        if (collection[i].StationNumberRight != numberright)
                            right = false;
                        //
                        if (collection[i].NameObject != text)
                            name = false;
                        //
                        if (collection[i].FileClick != fileClickCompare)
                            fileClick = false;
                        //
                        if (collection[i].Notes != notestext)
                            notes = false;
                        //
                        if (collection[i].IsVisible != visible)
                            isvisible = false;
                        //
                        if (collection[i] is LightTrain)
                        {
                            if ((collection[i] as LightTrain) != null)
                            {
                                if ((collection[i] as LightTrain).IsInput != IsInput)
                                    input = false;
                            }
                        }
                        else
                        {
                            input = false;
                        }
                        //
                        if (collection[i] is Disconnectors)
                        {
                            if ((collection[i] as Disconnectors) != null)
                            {
                                if ((collection[i] as Disconnectors).Type != type)
                                    typeDisconnector = false;
                            }
                        }
                        else
                        {
                            typeDisconnector = false;
                        }
                    }
                }
            }
        }

        private void UpdateEl(MyTable row)
        {
            if (!string.IsNullOrEmpty(Name_textbox.Text))
                row.Name = Name_textbox.Text;
            if (!string.IsNullOrEmpty(Notes_textbox.Text))
                row.Notes = Notes_textbox.Text;
            if (!string.IsNullOrEmpty(Numberstationleft_textbox.Text))
                row.Numberstationleft = Numberstationleft_textbox.Text;
            row.IsVisible = IsVisibleObject.IsChecked.Value;
            if (!string.IsNullOrEmpty(Numberrightstation_textbox.Text))
                row.Numberstationright = Numberrightstation_textbox.Text;
            //
            if (checkBox_inputlight.Visibility == System.Windows.Visibility.Visible)
                row.IsInput = checkBox_inputlight.IsChecked.Value;
            //
            if (TypeDis.Visibility == System.Windows.Visibility.Visible)
                row.TypeDis = Combox_TypeObject.Text;
            if (!string.IsNullOrEmpty(NameFileClick.Content.ToString()))
                row.FileClick = NameFileClick.Content.ToString();
            //
            TableObject.Items.Refresh();
        }

        private string GetNameDisconnector(TypeDisconnectors type)
        {
            foreach (var row in WindowWork.WorkGrafic.TypeDisconnectorsDic)
            {
                if (row.Value == type)
                    return row.Key;
            }
            //
            return string.Empty;
        }

        private void UpdateSettings(object sender)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (_selectelement.Count > 0)
                    {
                        List<MyTable> answerUpdate = new List<MyTable>();
                        foreach (IGraficObejct element in _selectelement)
                        {
                            MyTable row = FindObject(element.Id);
                            if (row != null)
                            {
                                UpdateEl(row);
                                //
                                answerUpdate.Add(row);
                                //answerUpdate.Add(new MyTable()
                                //{
                                //    Id = row.Id,
                                //    Name = row.Name,
                                //    Notes = row.Notes,
                                //    Numberstationleft = row.Numberstationleft,
                                //    Numberstationright = row.Numberstationright,
                                //    IsVisible = row.IsVisible,
                                //    IsInput = row.IsInput,
                                //    Type = row.TypeDis,//(_selectelement.Count == 1) ? Combox_TypeObject.Text: ((element is Disconnectors) ? GetNameDisconnector((element as Disconnectors).Type) : string.Empty)
                                //    FileClick = row.FileClick
                                //});
                            }
                        }
                        //
                        if (UpdateElement != null)
                            UpdateElement(answerUpdate);
                        //
                        if (TableObject.SelectedIndex >= 0 && TableObject.SelectedIndex < TableObject.Items.Count - 1)
                        {
                            TableObject.SelectedIndex++;
                            Name_textbox.Focus();
                        }
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }));
        }

        private void Name_textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdateSettings));
        }

        private void Empty()
        {
            if (Name_textbox.Text != string.Empty)
                Name_textbox.Text = string.Empty;
                NameFileClick.Content = string.Empty;
            if (Notes_textbox.Text != string.Empty)
                Notes_textbox.Text = string.Empty;
            IsVisibleObject.IsChecked = false;
            if (Numberstationleft_textbox.Text != string.Empty)
                Numberstationleft_textbox.Text = string.Empty;
            if (Numberrightstation_textbox.Text != string.Empty)
                Numberrightstation_textbox.Text = string.Empty;
            checkBox_inputlight.IsChecked = false;
            Combox_TypeObject.Text = string.Empty;
            //
            foreach (MyTable row in collection)
                row.IsSelect = false;
        }

        #region EventsToolBox

        private void Numberstationleft_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int buffer = 0;
                if (string.IsNullOrEmpty(Numberstationleft_textbox.Text) || !int.TryParse(Numberstationleft_textbox.Text, out buffer))
                {
                    Combox_NumberStationLeft.SelectedIndex = -1;
                    Numberstationleft_textbox.Text = string.Empty;
                }
                else
                    SelectCombox(Numberstationleft_textbox.Text, Combox_NumberStationLeft);
            }
            catch { }
        }

        private void Numberstationright_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int buffer = 0;
                if (string.IsNullOrEmpty(Numberrightstation_textbox.Text) || !int.TryParse(Numberrightstation_textbox.Text, out buffer))
                {
                    Combox_NumberStationRight.SelectedIndex = -1;
                    Numberrightstation_textbox.Text = string.Empty;
                }
                else
                    SelectCombox(Numberrightstation_textbox.Text, Combox_NumberStationRight);
            }
            catch { }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TableObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectelement.Clear();
                List<int> _answer = new List<int>();
                foreach (object selectrow in TableObject.SelectedItems)
                {
                    MyTable newrow = (MyTable)selectrow;
                    _answer.Add(newrow.Id);
                }
                //
                if (TableObject.SelectedItems.Count == 1)
                {
                    MyTable table = (MyTable)TableObject.SelectedItem;
                    Name_textbox.Text = table.Name;
                    Notes_textbox.Text = table.Notes;
                    IsVisibleObject.IsChecked = table.IsVisible;
                    Numberstationleft_textbox.Text = table.Numberstationleft;
                    Numberrightstation_textbox.Text = table.Numberstationright;
                    LineHelp line = new LineHelp() { Id = table.Id };
                    _selectelement.Add(line);
                }
                else
                    Empty();
                //
                if (SpisokId != null && _answer.Count > 0)
                    SpisokId(_answer, null);
            }
            catch { }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = _closing;
        }

        private void button_bigpath_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.chiefroad;
            if (EventAdd != null)
                EventAdd();
        }

        private void comboBox_number_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBox_number.SelectedIndex != -1)
                {
                    int buffer = 1;
                    string[] massiv = comboBox_number.SelectedItem.ToString().Split(new string[] { "     -     " }, StringSplitOptions.RemoveEmptyEntries);
                    if (massiv.Length == 2 && int.TryParse(massiv[1], out buffer))
                    {
                        CurrentNumberStation = int.Parse(massiv[1]);
                        CurrentNameStation = massiv[0];
                    }
                    else
                    {
                        CurrentNumberStation = -1;
                        CurrentNameStation = string.Empty;
                    }
                }
                else
                {
                    CurrentNumberStation = -1;
                    CurrentNameStation = string.Empty;
                }
            }
            catch { }
        }

        private void comboBox_StationLeft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Combox_NumberStationLeft.SelectedIndex != -1)
            {
                int buffer = 1;
                string[] massiv = Combox_NumberStationLeft.SelectedItem.ToString().Split(new string[] { "     -     " }, StringSplitOptions.RemoveEmptyEntries);
                if (massiv.Length == 2)
                {
                    if (int.TryParse(massiv[1], out buffer))
                        Numberstationleft_textbox.Text = massiv[1];
                }
            }
        }

        private void comboBox_StationRight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Combox_NumberStationRight.SelectedIndex != -1)
            {
                int buffer = 1;
                string[] massiv = Combox_NumberStationRight.SelectedItem.ToString().Split(new string[] { "     -     " }, StringSplitOptions.RemoveEmptyEntries);
                if (massiv.Length == 2)
                {
                    if (int.TryParse(massiv[1], out buffer))
                        Numberrightstation_textbox.Text = massiv[1];
                }
            }
        }

        private void checkBox_setka_Click(object sender, RoutedEventArgs e)
        {
            if (EventSetka != null)
                EventSetka((bool)checkBox_setka.IsChecked);
        }

        private void checkBox_orta_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox_orta.IsChecked)
                Orta = true;
            else Orta = false;
        }

        private void button_size_Click(object sender, RoutedEventArgs e)
        {
            //приводим картинку в оригинальный режим
            if (EventOriginalSize != null)
                EventOriginalSize();
        }

        private void button_station_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.buttonstation;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_route_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.signal;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_signal_shunting_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.lightShunting;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_signal_train_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.lightTrain;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_kgu_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.kgu;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_ktcm_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.ktcm;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_move_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.move;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_namestation_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.namestation;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_ramkastation_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.ramka;
            AllEnabled();
            if (EventAdd != null)
                EventAdd();
        }

        private void button_helpline_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.line;
            AllEnabled();
            if (EventAdd != null)
                EventAdd();
        }

        private void button_time_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.time;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_peregonpath_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.otrezok;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_numbertrain_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.numbertrain;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_peregonarrow_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.arrowmove;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_text_help_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.texthelp;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_help_element_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.help_element;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_disconnectors_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.disconnectors;
            if (EventAdd != null)
                EventAdd();
        }

        private void AllEnabled()
        {
            TableObject.IsEnabled = !TableObject.IsEnabled;
            Name_textbox.IsEnabled = !Name_textbox.IsEnabled;
            Notes_textbox.IsEnabled = !Notes_textbox.IsEnabled;
            IsVisibleObject.IsEnabled = !IsVisibleObject.IsEnabled;
            Numberstationleft_textbox.IsEnabled = !Numberstationleft_textbox.IsEnabled;
            Ok_Settings.IsEnabled = !Ok_Settings.IsEnabled;
            button_size.IsEnabled = !button_size.IsEnabled;
            comboBox_number.IsEnabled = !comboBox_number.IsEnabled;
            if (comboBox_number.SelectedIndex != -1)
            {
                button_bigpath.IsEnabled = !button_bigpath.IsEnabled;
                button_station.IsEnabled = !button_station.IsEnabled;
                button_route.IsEnabled = !button_route.IsEnabled;
                button_signal_shunting.IsEnabled = !button_signal_shunting.IsEnabled;
                button_signal_train.IsEnabled = !button_signal_train.IsEnabled;
                button_move.IsEnabled = !button_move.IsEnabled;
                button_kgu.IsEnabled = !button_kgu.IsEnabled;
                button_ktcm.IsEnabled = !button_ktcm.IsEnabled;
                button_namestation.IsEnabled = !button_namestation.IsEnabled;
                button_peregonarrow.IsEnabled = !button_peregonarrow.IsEnabled;
                button_peregonpath.IsEnabled = !button_peregonpath.IsEnabled;
                button_numbertrain.IsEnabled = !button_numbertrain.IsEnabled;
                button_help_element.IsEnabled = !button_help_element.IsEnabled;
            }
            button_ramkastation.IsEnabled = !button_ramkastation.IsEnabled;
            button_helpline.IsEnabled = !button_helpline.IsEnabled;
        }

        public void EnabledForm()
        {
            AllEnabled();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdateSettings));
        }

        private void button_area_picture_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.area_picture;
        }

        #endregion  

        private void button_analog_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.analogCell;
            if (EventAdd != null)
                EventAdd();
        }


        private void textFormatBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //double buffer;
            //if(!double.TryParse(SCADA.Common.HelpCommon.HelpFuctions.GetFormatString(textFormatBox.Text,out buffer))
        }

        private void button_diagnostic_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.diagnostikCell;
            if (EventAdd != null)
                EventAdd();
        }

        private void button_area_webBrowser_Click(object sender, RoutedEventArgs e)
        {
            m_currentelemantdraw = ViewElement.webBrowser;
        }

        private void AddFileClick_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new System.Windows.Forms.OpenFileDialog() { Multiselect = false, Filter = "All types(*.*)|*.*" };
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                NameFileClick.Content = openDialog.FileName;
        }
    }
}
