using System;
using System.Windows;
using System.Windows.Input;
using SCADA.Desiner.Enums;

namespace SCADA.Desiner.WorkForms
{
    public partial class CloseProgramm : Window
    {
        //public ViewCancel Cancel { get; set; }

        public CloseProgramm()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void button_NO_Click(object sender, RoutedEventArgs e)
        {
            DialogResult =false;
        }

        //private void button_CANCEL_Click(object sender, RoutedEventArgs e)
        //{
        //    Cancel = ViewCancel.cancel;
        //}
    }
}
