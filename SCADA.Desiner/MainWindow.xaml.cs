using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SCADA.Desiner.WindowWork;


namespace SCADA.Desiner
{

    public partial class MainWindow : Window
    {
        #region Переменные и свойства

        WorkGrafic settingsWindow;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            SettingsDefult();
        }

        private void SettingsDefult()
        {
            Background = SCADA.Desiner.Constanst.MainColors.Fon;
            Dictionary<string, MenuItem> menuItems = new Dictionary<string, MenuItem>()
            {
                {NewProject.Name, NewProject}, {OpenProject.Name, OpenProject}, {SaveProject.Name, SaveProject}, {SaveAsProject.Name, SaveAsProject},
                {ExitProject.Name, ExitProject}, {SettingsStation.Name, SettingsStation}, {SettingsCommand.Name, SettingsCommand}, 
                {SettingsFigure.Name, SettingsFigure}, {FullScreen.Name, FullScreen}
            };
            settingsWindow = new WorkGrafic(this, DrawCanvas, menumain, menuItems, statuskoordinate, pointinsert, elementsize, infokoordinate, mousekoordinate, nameelement, countselectelement, Step);
        }
    }
}
