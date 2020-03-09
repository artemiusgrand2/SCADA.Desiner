using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Constanst;
using SCADA.Desiner.Enums;
using SCADA.Desiner.Delegate;
using SCADA.Desiner.CommandElement;
using SCADA.Desiner.GeometryTransform;
using SCADA.Common.Enums;
using SCADA.Common.SaveElement;
using SCADA.Common.Constant;

namespace SCADA.Desiner.Tools
{
    /// <summary>
    /// Interaction logic for ToolsManagement.xaml
    /// </summary>
    public partial class ToolsManagement : Window
    {
        #region Переменные и свойства
        Dictionary<ViewPanel, int> _viewpanel_number = new Dictionary<ViewPanel, int>();
      //  Dictionary<ViewCommand, int> _viewpanel_number = new Dictionary<ViewCommand, int>();
        private bool _closing = true;
        public bool ClosingCommand
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
        private ViewElement _currentelemantdraw = ViewElement.none;
        public ViewElement CurrentDraw
        {
            get
            {
                return _currentelemantdraw;
            }
            set
            {
                _currentelemantdraw = value;
            }
        }
        /// <summary>
        /// событие на добавление нового елемента
        /// </summary>
        public event addelement EventAdd;
        /// <summary>
        /// событие вызываемое при изменениии свойств объекта
        /// </summary>
        public event updateelementCommand UpdateElement;
        /// <summary>
        /// выделенный лемент управления
        /// </summary>
        IGraficObejct _selectitem = null;

        readonly IDictionary<ViewCommand, string> dictionaryCommand = new Dictionary<ViewCommand, string>()
        {
            {ViewCommand.diagnostics, ViewNameCommand.diagnostics}, {ViewCommand.numbertrain, ViewNameCommand.numbertrain}, {ViewCommand.show_control, ViewNameCommand.show_control},
            {ViewCommand.sound, ViewNameCommand.sound}, {ViewCommand.show_table_train, ViewNameCommand.show_table_train}, {ViewCommand.run_auto_supervisory, ViewNameCommand.run_auto_supervisory},
            {ViewCommand.filter_train, ViewNameCommand.filter_train}, {ViewCommand.help, ViewNameCommand.help}, {ViewCommand.style, ViewNameCommand.style}, {ViewCommand.train_even, ViewNameCommand.train_even},
            {ViewCommand.train_odd, ViewNameCommand.train_odd}, {ViewCommand.train_unknow, ViewNameCommand.train_unknow}, {ViewCommand.viewtrain, ViewNameCommand.viewtrain}, {ViewCommand.content_help, ViewNameCommand.content_help},
            {ViewCommand.content_exchange, ViewNameCommand.content_exchange}, {ViewCommand.update_style, ViewNameCommand.update_style}, {ViewCommand.numbertrack, ViewNameCommand.numbertrack}, {ViewCommand.pass, ViewNameCommand.pass},
            {ViewCommand.electro, ViewNameCommand.electro},  {ViewCommand.exit, ViewNameCommand.exit}
        };
        #endregion

        public ToolsManagement(double heightToolsGraphics, Settings start_settings)
        {
            InitializeComponent();
            Start(heightToolsGraphics, start_settings);
        }

        private void Start(double heightToolsGraphics, Settings start_settings)
        {
            if (start_settings != null)
            {
                if (start_settings.StartCommand_Width > 0)
                    Width = start_settings.StartCommand_Width;
                if (start_settings.StartCommand_Height > 0)
                    Height = start_settings.StartCommand_Height;
                //
                Left = start_settings.StartCommand_X;
                Top = start_settings.StartCommand_Y;
            }
            else
            {
                Left = WindowWork.WorkGrafic.width - this.Width;
                Top = 24 + heightToolsGraphics;
            }
            //
            FullcomboxInfo();
            OperationsGrafic.SelectObject += SelectObject;
        }

        /// <summary>
        /// заполняем перечень используемых станций
        /// </summary>
        /// <param name="stcollection"></param>
        private void FullcomboxInfo()
        {
            //_viewcommand_number.Add(ViewCommand.none, -1);
            foreach (KeyValuePair<ViewCommand, string> el in dictionaryCommand)
                Combox_ViewCommand.Items.Add(el.Value);
            //------------------------------------
            _viewpanel_number.Add(ViewPanel.none, -1);
            Combox_ViewPanel.Items.Add(ViewNamePanel.drawtrain);
            _viewpanel_number.Add(ViewPanel.drawtrain, Combox_ViewPanel.Items.Count - 1);
            //
            Combox_ViewPanel.Items.Add(ViewNamePanel.tabletrain);
            _viewpanel_number.Add(ViewPanel.tabletrain, Combox_ViewPanel.Items.Count - 1);
        }

        /// <summary>
        /// выделяем нужные объекты
        /// </summary>
        /// <param name="selectcollection"></param>
        private void SelectObject(List<IGraficObejct> selectcollection)
        {
            Empty();
            try
            {
                if (selectcollection.Count == 1 && selectcollection[selectcollection.Count - 1] is ButtonCommand)
                {
                    _selectitem = selectcollection[selectcollection.Count - 1];
                    Name_textbox.Text = selectcollection[selectcollection.Count - 1].NameObject;
                    Help_textbox.Text = (selectcollection[selectcollection.Count - 1] as ButtonCommand).HelpText;
                    Parametrs.Text = (selectcollection[selectcollection.Count - 1] as ButtonCommand).Parameters;
                    Combox_ViewCommand.SelectedItem =  (dictionaryCommand.ContainsKey((selectcollection[selectcollection.Count - 1] as ButtonCommand).ViewCommand))? dictionaryCommand[(selectcollection[selectcollection.Count - 1] as ButtonCommand).ViewCommand]:null;
                    Combox_ViewPanel.SelectedIndex = _viewpanel_number[(selectcollection[selectcollection.Count - 1] as ButtonCommand).ViewPanel];
                }
                else
                    _selectitem = null;
            }
            catch { }
        }

        private void Empty()
        {
            if (Name_textbox.Text != string.Empty)
                Name_textbox.Text = string.Empty;
            Help_textbox.Text = string.Empty;
            Parametrs.Text = string.Empty;
            Combox_ViewCommand.SelectedIndex = -1;
            Combox_ViewPanel.SelectedIndex = -1;
        }

        private void button_management_Click(object sender, RoutedEventArgs e)
        {
            if (_currentelemantdraw == ViewElement.none)
            {
                _currentelemantdraw = ViewElement.buttoncommand;
                if (EventAdd != null)
                    EventAdd();
            }
        }

        private void Ok_Settings_Click(object sender, RoutedEventArgs e)
        {
            ButtonOK();
        }

        private void ButtonOK()
        {
            if (_selectitem != null)
            {
                if (UpdateElement != null)
                    UpdateElement(Name_textbox.Text, Help_textbox.Text, Parametrs.Text, SetViewCommand(), SetViewPanel(), _selectitem.Id);
            }
        }

        private ViewPanel SetViewPanel()
        {
            if (Combox_ViewPanel.SelectedItem != null)
            {
                if (Combox_ViewPanel.SelectedItem.ToString() == ViewNamePanel.drawtrain)
                    return ViewPanel.drawtrain;
                //
                if (Combox_ViewPanel.SelectedItem.ToString() == ViewNamePanel.tabletrain)
                    return ViewPanel.tabletrain;
            }
            //
            return ViewPanel.none;
        }

        private ViewCommand SetViewCommand()
        {
            if (Combox_ViewCommand.SelectedIndex != -1)
            {
                foreach (KeyValuePair<ViewCommand, string> el in dictionaryCommand)
                {
                    if (Combox_ViewCommand.SelectedItem.ToString() == el.Value)
                        return el.Key;
                }
                //
                return ViewCommand.none;
            }
            return ViewCommand.none;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = _closing;
        }

        private void button_table_number_Click(object sender, RoutedEventArgs e)
        {
            if (_currentelemantdraw == ViewElement.none)
                _currentelemantdraw = ViewElement.tablenumbertrain;
        }

        private void button_area_station_Click(object sender, RoutedEventArgs e)
        {
            if (_currentelemantdraw == ViewElement.none)
                _currentelemantdraw = ViewElement.area_station;
        }

        private void button_area_message_Click(object sender, RoutedEventArgs e)
        {
            if (_currentelemantdraw == ViewElement.none)
                _currentelemantdraw = ViewElement.area_message;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void button_table_autopilot_Click(object sender, RoutedEventArgs e)
        {
            if (_currentelemantdraw == ViewElement.none)
                _currentelemantdraw = ViewElement.tableautopilot;
        }

    }
}
