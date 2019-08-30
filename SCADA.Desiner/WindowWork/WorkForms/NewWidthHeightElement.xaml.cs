using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SCADA.Desiner.WorkForms
{
    public partial class NewWidthHeightElement : Window
    {
        public double XElement { get; set; }
        public double YElement { get; set; }
        private double _delta = 40;

        public NewWidthHeightElement(bool IsVisiblityHeight, string nameX, string nameY)
        {
            InitializeComponent();
            //
            label_x.Content = nameX;
            label_y.Content = nameY;
            if (IsVisiblityHeight)
            {
                Buffer.Visibility = System.Windows.Visibility.Visible;
                PanelHeight.Visibility = System.Windows.Visibility.Visible;
                Height += _delta;
            }
        }


        private void IsEnabledOk()
        {
            if (XElement == 0 && YElement == 0)
                button_OK.IsEnabled = false;
            else
                button_OK.IsEnabled = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }



        private void text_box_height_TextChanged(object sender, TextChangedEventArgs e)
        {
            double buffer = 0;
            if (double.TryParse(text_box_height.Text, out buffer))
            {
                double value = double.Parse(text_box_height.Text);
                if (value >= 1)
                {
                    YElement = value;

                }
                else
                {
                    text_box_height.Text = string.Empty;
                    YElement = 0;
                }
            }
            else
            {
                text_box_height.Text = string.Empty;
                YElement = 0;
            }
            //
            IsEnabledOk();
        }

        private void text_box_width_TextChanged(object sender, TextChangedEventArgs e)
        {
            double buffer = 0;
            if (double.TryParse(text_box_width.Text, out buffer))
            {
                double value = double.Parse(text_box_width.Text);
                if (value >= 1)
                    XElement = value;
                else
                {
                    text_box_width.Text = string.Empty;
                    XElement = 0;
                }
            }
            else
            {
                text_box_width.Text = string.Empty;
                XElement = 0;
            }
            //
            IsEnabledOk();
        }
    }
}
