using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SCADA.Common.SaveElement;

namespace SCADA.Desiner.WorkForms
{
  
    public partial class EnterNewNameColor : Window
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

        #endregion

        public EnterNewNameColor(NameColors namecolor)
        {
            InitializeComponent();
            //
            if (namecolor != null)
            {
                textBox_name.Text = namecolor.NameColor;
                _namecolor.R = namecolor.R;
                _namecolor.G = namecolor.G;
                _namecolor.B = namecolor.B;
                panel_color.Background = new SolidColorBrush(Color.FromRgb(namecolor.R, namecolor.G, namecolor.B));
            }
            else
                panel_color.Background = new SolidColorBrush(Color.FromRgb(_namecolor.R, _namecolor.G, _namecolor.B));
        }

        private void panel_color_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.ColorDialog colordialog = new System.Windows.Forms.ColorDialog();
            if (colordialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                panel_color.Background = new SolidColorBrush(Color.FromRgb(colordialog.Color.R, colordialog.Color.G, colordialog.Color.B));
                _namecolor.R = colordialog.Color.R;
                _namecolor.G = colordialog.Color.G;
                _namecolor.B = colordialog.Color.B;
            }
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox_name.Text))
            {
                _namecolor.NameColor = textBox_name.Text;
               // Close();
            }
            else MessageBox.Show("Нельзя вводить пустую строку !!!");
             DialogResult = true;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DragMove();
        }

    }
}
