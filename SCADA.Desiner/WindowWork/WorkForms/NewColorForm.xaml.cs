using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Desiner.WindowWork;
using SCADA.Common.SaveElement;

namespace SCADA.Desiner.WorkForms
{
  
    public partial class NewColorForm : Window
    {

        #region Переменные

        NameColors _namecolor = new NameColors() { NameColor = string.Empty };
        public NameColors NameColor
        {
            get
            {
                return _namecolor;
            }
        }
        /// <summary>
        /// цвет оттенок красного
        /// </summary>
        public byte R { get; set; }
        /// <summary>
        /// цвет оттенок зеленого
        /// </summary>
        public byte G { get; set; }
        /// <summary>
        /// цвет оттенок синего
        /// </summary>
        public byte B { get; set; }

        #endregion

        public NewColorForm(NameColors namecolor)
        {
            InitializeComponent();
            FullCollection();
            AnalisName(namecolor);
        }

        private void FullCollection()
        {
            foreach (NameColors color in WorkGrafic.NameColors)
            {
                comboBox_namecolor.Items.Add(color.NameColor);
            }
        }

        private void AnalisName(NameColors namecolor)
        {
            foreach (object ob in comboBox_namecolor.Items)
            {
                if (ob.ToString() == namecolor.NameColor)
                {
                    comboBox_namecolor.SelectedItem = ob;
                    return;
                }
            }
            //
            comboBox_namecolor.Items.Add(namecolor.NameColor);
            WorkGrafic.NameColors.Add(namecolor);
            comboBox_namecolor.SelectedItem = comboBox_namecolor.Items[comboBox_namecolor.Items.Count - 1];
        }

        public static bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            //
            foreach (NameColors color in WorkGrafic.NameColors)
            {
                if (name == color.NameColor)
                {
                    return false;
                }
            }
            //
            return true;
        }

        private bool Contains(string name, ref int index)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            //
            foreach (NameColors color in WorkGrafic.NameColors)
            {
                if (name == color.NameColor)
                {
                    return false;
                }
                index++;
            }
            //
            return true;
        }

        private void GetColorPanel()
        {
            panel_color.Background = new SolidColorBrush(Color.FromRgb(WorkGrafic.NameColors[comboBox_namecolor.SelectedIndex].R, WorkGrafic.NameColors[comboBox_namecolor.SelectedIndex].G,
                 WorkGrafic.NameColors[comboBox_namecolor.SelectedIndex].B));
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_namecolor.Items.Count > 0)
            {
                if (comboBox_namecolor.SelectedIndex == -1)
                    MessageBox.Show("Выберите из перечьня название цвета !!!");
                else
                {
                    _namecolor.NameColor = WorkGrafic.NameColors[comboBox_namecolor.SelectedIndex].NameColor;
                    _namecolor.R = WorkGrafic.NameColors[comboBox_namecolor.SelectedIndex].R;
                    _namecolor.G = WorkGrafic.NameColors[comboBox_namecolor.SelectedIndex].G;
                    _namecolor.B = WorkGrafic.NameColors[comboBox_namecolor.SelectedIndex].B;
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Введите название цвета название цвета !!!");
            }
            //
         //   DialogResult = true;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void button_removeAtcolor_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_namecolor.Items.Count > 0)
            {
                if (comboBox_namecolor.SelectedIndex != -1)
                {
                    int N = comboBox_namecolor.SelectedIndex;
                    comboBox_namecolor.Items.RemoveAt(N);
                    comboBox_namecolor.Text = string.Empty;
                    WorkGrafic.NameColors.RemoveAt(N);
                }
                else
                {
                    comboBox_namecolor.Items.RemoveAt(comboBox_namecolor.Items.Count - 1);
                    WorkGrafic.NameColors.RemoveAt(WorkGrafic.NameColors.Count - 1);
                }
            }
        }

        private void button_addcolor_Click(object sender, RoutedEventArgs e)
        {
            EnterNewNameColor namecolor;
            if (comboBox_namecolor.SelectedIndex != -1)
                namecolor = new EnterNewNameColor(WorkGrafic.NameColors[comboBox_namecolor.SelectedIndex]);
            else namecolor = new EnterNewNameColor(null);
            //
            namecolor.ShowDialog();
            if (namecolor.NameColor.NameColor != string.Empty)
            {
                int index = 0;
                if (Contains(namecolor.NameColor.NameColor, ref index))
                {
                    comboBox_namecolor.Items.Add(namecolor.NameColor.NameColor);
                    WorkGrafic.NameColors.Add(namecolor.NameColor);
                    comboBox_namecolor.SelectedItem = comboBox_namecolor.Items[comboBox_namecolor.Items.Count - 1];
                }
                else
                {
                    WorkGrafic.NameColors[index].R = namecolor.NameColor.R;
                    WorkGrafic.NameColors[index].G = namecolor.NameColor.G;
                    WorkGrafic.NameColors[index].B = namecolor.NameColor.B;
                    comboBox_namecolor.SelectedItem = comboBox_namecolor.Items[index];
                    GetColorPanel();
                }
            }
        }

        private void comboBox_namecolor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_namecolor.SelectedIndex != -1)
            {
                GetColorPanel();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
