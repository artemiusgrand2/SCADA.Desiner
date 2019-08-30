using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using SCADA.Desiner.Enums;

namespace SCADA.Desiner.WorkForms
{

    public partial class NewSizeElementWpf : Window
    {
        #region Переменные и свойства

        /// <summary>
        /// масштабный коэффициент
        /// </summary>
        public double Scroll
        {
            get
            {
                return slider_scroll.Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        ViewObject view;

        /// <summary>
        /// Вид
        /// </summary>
        public ViewObject View
        {
            get
            {
                return view;
            }
        }

        #endregion

        public NewSizeElementWpf(double scroll, ViewObject view)
        {
            InitializeComponent();
            this.view = view;
            //
            slider_scroll.Value = scroll;
            switch (view)
            {
                case ViewObject.text:
                    {
                        slider_scroll.Minimum = 1;
                        slider_scroll.Maximum = 100;
                        slider_scroll.TickFrequency = 1;
                    }
                    break;
                case ViewObject.weight:
                    {
                        slider_scroll.Minimum = 1;
                        slider_scroll.Maximum = 20;
                        slider_scroll.TickFrequency = 1;
                    }
                    break;
                case ViewObject.objectgrafic:
                    {
                        slider_scroll.Minimum = Math.Round(scroll / 4, 1);
                        slider_scroll.Maximum = Math.Round(scroll * 4, 1);
                        slider_scroll.TickFrequency = 0.1;
                    }
                    break;
            }
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
