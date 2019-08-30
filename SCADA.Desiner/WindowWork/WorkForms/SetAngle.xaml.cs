using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SCADA.Desiner.WindowWork;

namespace SCADA.Desiner.WorkForms
{
    public partial class SetAngle : Window
    {

        public double Angle { get; set; }
        private WorkGrafic operationgrafic;

        public SetAngle(WorkGrafic operationgrafic)
        {
            InitializeComponent();
            this.operationgrafic = operationgrafic;
        }

        private void IsEnabledOk()
        {
            if (Angle == 0)
                button_OK.IsEnabled = false;
            else
                button_OK.IsEnabled = true;
        }

        private void text_box_angle_TextChanged(object sender, TextChangedEventArgs e)
        {
            double buffer = 0;
            if (double.TryParse(text_box_angle.Text, out buffer))
            {
                double value = double.Parse(text_box_angle.Text);
                if (Math.Abs(value) >= 0 && Math.Abs(value) <= 360)
                    Angle = value;
                else
                    Angle = 0;
            }
            else
                Angle = 0;
            //
            IsEnabledOk();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (operationgrafic != null)
            {
                operationgrafic.EventRotateElement(Angle);
            }
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
