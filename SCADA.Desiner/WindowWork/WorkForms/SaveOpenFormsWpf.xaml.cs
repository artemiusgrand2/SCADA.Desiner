using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using SCADA.Desiner.WindowWork;

namespace SCADA.Desiner.WorkForms
{
    public partial class SaveOpenFormsWpf : Window
    {
        #region Variable

        Timer tim;

        #endregion

        public SaveOpenFormsWpf(string name)
        {
            InitializeComponent();
            //
            label_message.Content = name;
            tim = new Timer(100);
            tim.Elapsed += tim_Tick;
            tim.Start();
        }

        private void tim_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (WorkGrafic.Close)
                {
                    tim.Stop();
                    Close();
                }
            }));
        }

    }
}
