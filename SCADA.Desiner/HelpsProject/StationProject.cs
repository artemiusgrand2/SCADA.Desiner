using System;
using System.Collections.Generic;
using SCADA.Desiner.Inteface;

namespace SCADA.Desiner.HelpsProject
{
    class StationProject
    {
        /// <summary>
        /// путь к файлу проекта станции
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// список кнопок станции
        /// </summary>
        List<IGraficObejct> _buttonstation = new List<IGraficObejct>();
        public List<IGraficObejct> ButtonStation
        {
            get
            {
                return _buttonstation;
            }
            set
            {
                _buttonstation = value;
            }
        }
        /// <summary>
        /// список главных путей
        /// </summary>
        List<IGraficObejct> _path = new List<IGraficObejct>();
        public List<IGraficObejct> Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }
        /// <summary>
        /// список сигналов
        /// </summary>
        List<IGraficObejct> _signal = new List<IGraficObejct>();
        public List<IGraficObejct> Signal
        {
            get
            {
                return _signal;
            }
            set
            {
                _signal = value;
            }
        }
    }
}
