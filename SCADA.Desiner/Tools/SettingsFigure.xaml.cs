using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.HelpsProject;
using SCADA.Desiner.Delegate;
using SCADA.Desiner.GeometryTransform;
using SCADA.Common.SaveElement;

namespace SCADA.Desiner.Tools
{
    /// <summary>
    /// Interaction logic for SettingsFigure.xaml
    /// </summary>
    public partial class SettingsFigure : Window
    {
        #region Переменные и свойства

        public static int NSP = -1;
        private bool _closing = false;
        //флаг закрыттия панели
        public bool ClosingSettings
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
        /// передаем список выделенных элементов
        /// </summary>
        public event SpisokId SpisokId;
        /// <summary>
        /// колекция таблицы
        /// </summary>
        ObservableCollection<MySettingsFigure> collection = null;

        #endregion

        public SettingsFigure(Settings start_settings)
        {
            InitializeComponent();
            Start(start_settings);
        }

        private void Start(Settings start_settings)
        {
            if (start_settings != null)
            {
                if (start_settings.StartSettings_Width > 0)
                    Width = start_settings.StartSettings_Width;
                if (start_settings.StartSettings_Height > 0)
                    Height = start_settings.StartSettings_Height;
                //
                Left = start_settings.StartSettings_X;
                Top = start_settings.StartSettings_Y;
            }
            else
            {
                Left = (System.Windows.SystemParameters.PrimaryScreenWidth - Width) / 2;
                Top = (System.Windows.SystemParameters.PrimaryScreenHeight - Height) / 2;
            }
            //
            collection = new ObservableCollection<MySettingsFigure>();
            TableObject.ItemsSource = collection;
            OperationsGrafic.SelectObject += SelectObject;
            OperationsGrafic.UpdateGeometry += UpdateObject;
        }

        /// <summary>
        /// обновление списка объектов
        /// </summary>
        /// <param name="updatecollection">список объектов для редактирования</param>
        private void UpdateObject(List<IGraficObejct> updatecollection)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateGeometry), updatecollection);
            if (updatecollection != null && updatecollection.Count == 1 && collection.Count > 0)
            {
                if (collection[collection.Count - 1].Id == updatecollection[updatecollection.Count - 1].Id)
                {
                    collection.Clear();
                    RefreshTable(updatecollection[updatecollection.Count - 1]);
                }
            }
        }

        private void UpdateGeometry(object sender)
        {
            if (sender != null && sender is List<IGraficObejct>)
            {
                List<IGraficObejct> updatecollection = sender as List<IGraficObejct>;
                Dispatcher.Invoke(new Action(() =>
                {
                    if (updatecollection.Count == 1 && collection.Count > 0)
                    {
                        if (collection[collection.Count - 1].Id == updatecollection[updatecollection.Count - 1].Id)
                        {
                            collection.Clear();
                            RefreshTable(updatecollection[updatecollection.Count - 1]);
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// выделяем нужные объекты
        /// </summary>
        /// <param name="selectcollection"></param>
        private void SelectObject(List<IGraficObejct> selectcollection)
        {
            try
            {
                collection.Clear();
                //
                if (selectcollection.Count == 1)
                    RefreshTable(selectcollection[selectcollection.Count - 1]);
            }
            catch { }
        }

        private void RefreshTable(IGraficObejct element)
        {
            if (element is IShowSettings)
            {
                int idfigure = 0;
                foreach (PathFigure geo in element.Figure.Figures)
                {
                    int idsegment = 0;
                    AddElemnet(element.Id, idfigure, NSP, SetPoint(geo.StartPoint.X, geo.StartPoint.Y), string.Empty, string.Empty, geo.StartPoint, element.Index);
                    foreach (PathSegment seg in geo.Segments)
                    {
                        //сегмент линия
                        LineSegment lin = seg as LineSegment;
                        if (lin != null)
                        {
                            AddElemnet(element.Id, idfigure, idsegment, SetPoint(lin.Point.X, lin.Point.Y), Angle(collection[collection.Count - 1].PointValue, lin.Point),
                                Delta(collection[collection.Count - 1].PointValue, lin.Point), lin.Point, element.Index);
                            idsegment++;
                        }
                    }
                    idfigure++;
                }
            }
            //
            TableObject.Items.Refresh();
        }

        private string Angle(Point start, Point finish)
        {
            try
            {
                return string.Format("{0:F2}", (Math.Atan((finish.X - start.X) / (finish.Y - start.Y)) * (180 / Math.PI)));
            }
            catch
            {
                return "error";
            }
        }

        private string Delta(Point start, Point finish)
        {
            try
            {
                return string.Format("({0:F2}, {1:F2})", (finish.X - start.X)/ System.Windows.SystemParameters.CaretWidth, (finish.Y - start.Y)/ System.Windows.SystemParameters.CaretWidth);
            }
            catch
            {
                return "error";
            }
        }

        private string SetPoint(double X, double Y)
        {
          //  return string.Format("({0:F2}; {1:F2}", (X - OperationsGrafic.Po.X) / OperationsGrafic.CurrentScroll, (Y - OperationsGrafic.Po.Y) / OperationsGrafic.CurrentScroll);
            return string.Format("({0:F2}; {1:F2}", X / System.Windows.SystemParameters.CaretWidth, Y / System.Windows.SystemParameters.CaretWidth);
        }

        private void AddElemnet(int id, int idfigure, int idsegment, string point, string angle, string delta, Point pointvalue, Index index)
        {
            collection.Add(new MySettingsFigure()
            {
                Id = id,
                IdFigure = idfigure,
                IdSegment = idsegment,
                Point = point,
                Angle = angle,
                Delta = delta,
                Number = collection.Count + 1,
                PointValue = new Point(pointvalue.X, pointvalue.Y)
            });
            //
            if (index != null)
            {
                if (idfigure == index.IndexFigure)
                {
                    if (idsegment == index.IndexSegment)
                        collection[collection.Count - 1].IsSelect = true;
                }
            }
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = _closing;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Table_Mouse_Click(object sender, MouseEventArgs e)
        {
            SelectId();
        }

        private void TableObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectId();
        }

        private void SelectId()
        {
            try
            {
                List<int> _answer = new List<int>();
                _answer.Add((TableObject.SelectedItem as MySettingsFigure).Id);
                //
                if (SpisokId != null && _answer.Count > 0)
                    SpisokId(_answer, new PointFigure()
                    {
                        IdFigure = (TableObject.SelectedItem as MySettingsFigure).IdFigure,
                        IdSegment = (TableObject.SelectedItem as MySettingsFigure).IdSegment
                    });
            }
            catch { }
        }

        private void SelectionModeNew(MySettingsFigure row)
        {
            foreach (MySettingsFigure el in collection)
            {
                if (el.Id == row.Id && el.IdFigure == row.IdFigure && el.IdSegment == row.IdSegment)
                    el.IsSelect = true;
                else el.IsSelect = false;
            }
            //
            TableObject.Items.Refresh();
        }
    }
}
