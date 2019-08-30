using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace SCADA.Desiner.WindowWork
{

    public partial class HatchLineForm : Window
    {

        #region Переменные и свойства

        IList<double> arrayhatch = new List<double>();
        /// <summary>
        /// коллекция штрихов
        /// </summary>
        public IList<double> ArrayHatch
        {
            get
            {
                return arrayhatch;
            }
            set
            {
                arrayhatch = value;
            }
        }

        #endregion

        public HatchLineForm(DoubleCollection collection)
        {
            InitializeComponent();
            ShowHatchCollection(collection);
        }

        private void ShowHatchCollection(DoubleCollection collection)
        {
            int index = 0;
            foreach (double step in collection)
            {
                if (index != collection.Count - 1)
                    textBox_format.Text += string.Format("{0},", step);
                else
                    textBox_format.Text += string.Format("{0}", step);
                index++;
            }
        }

        private void ParserString(string data)
        {
            string[] parser = data.Split(new char []{',', ';'},  StringSplitOptions.RemoveEmptyEntries);
            double buffer;
            foreach (string step in parser)
            {
                if (double.TryParse(step, out buffer))
                    arrayhatch.Add(buffer);
                else
                    MessageBox.Show(string.Format("Неверный формат данных - {0}", step));
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            ParserString(textBox_format.Text);
            DialogResult = true;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
