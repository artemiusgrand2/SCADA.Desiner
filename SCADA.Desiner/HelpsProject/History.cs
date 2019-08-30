using System;
using System.Collections.Generic;
using System.Windows.Media;
using SCADA.Desiner.Inteface;

namespace SCADA.Desiner.HelpsProject
{
     class History: IDisposable
    {
         List<IGraficObejct> _elements = new List<IGraficObejct>();
         public List<IGraficObejct> Elements
         {
             get
             {
                 return _elements;
             }
             set
             {
                 _elements = value;
             }
         }
       //Point _center = new Point();
       //public Point Center
       //{
       //    get
       //    {
       //        return _center;
       //    }
       //    set
       //    {
       //        _center = value;
       //    }
       //}
       //public double _scroll = 1;
       //public double Scroll
       //{
       //    get
       //    {
       //        return _scroll;
       //    }
       //    set
       //    {
       //        _scroll = value;
       //    }
       //}

       private TransformGroup groupTransform = new TransformGroup();
       /// <summary>
       /// Коллекция текущих трансформаций
       /// </summary>
       public TransformGroup GroupTransform
       {
           get { return groupTransform; }
           set { groupTransform = value; }
       }
       public void Dispose() { }
    }
}
