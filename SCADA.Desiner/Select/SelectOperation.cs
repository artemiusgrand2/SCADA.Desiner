using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using SCADA.Desiner.Inteface;
using SCADA.Desiner.Enums;
using SCADA.Desiner.BaseElement;

namespace SCADA.Desiner.Select
{
   
    /// <summary>
    /// класс для выбора объектов
    /// </summary>
    public class SelectOperation
    {
        /// <summary>
        /// выбираем один элемент
        /// </summary>
        /// <param name="collection">коллекция нарисованных объектов</param>
        /// <param name="pointclick">точка нажатия на мышь</param>
        /// <param name="activeselement">коллекция выделенных объектов</param>
        public static void SelectObject(Dictionary<int,IGraficObejct> collection, Point pointclick, List<IGraficObejct> activeselement)
        {
            List<IGraficObejct> select = FindElement(collection, pointclick);
            if (select != null)
            {
                DeleteActiveStroke(activeselement);
                //изменяем выбранную коллекцию объектов
                UpdateSelectObejct(ref select);
                //
                if (select[select.Count - 1].Selectobject)
                    select[select.Count - 1].DefaultColor();
                else
                {
                    select[select.Count - 1].DefaultColor();
                    select[select.Count - 1].Selectobject = true;
                    select[select.Count - 1].SelectColor();
                }
                //
                if (select[select.Count - 1].Selectobject)
                    activeselement.Add(select[select.Count - 1]);
                else
                    activeselement.Remove(select[select.Count - 1]);
            }
        }

        public static List<IGraficObejct> FindElement(Dictionary<int, IGraficObejct> collection, Point pointclick)
        {
            List<IGraficObejct> select = new List<IGraficObejct>();
            foreach (KeyValuePair<int, IGraficObejct> el in collection)
            {
                if ((el.Value is LineHelp) || (el.Value is LinePeregon))
                {
                    el.Value.CreateCollectionLines();
                    if (HitStrokeOne(el.Value.Lines, pointclick, el.Value, el.Value.StrokeThickness))
                        select.Add(el.Value);
                }
                else
                {
                    el.Value.CreateCollectionLines();
                    if (HitObject(el.Value.Lines, el.Value.Points, pointclick))
                        select.Add(el.Value);
                }
            }
            //
            if (select.Count > 0)
                return select;
            return null;
        }

        /// <summary>
        /// изменяем конечную выборку выделенных объектов
        /// </summary>
        /// <param name="select">массив найденных объектов</param>
        /// <returns></returns>
        private static void UpdateSelectObejct(ref List<IGraficObejct> select)
        {
            List<IGraficObejct> answer = new List<IGraficObejct>();
            for (int i = 0; i < select.Count; i++)
            {

                RamkaStation ramka = select[i] as RamkaStation;

                if (ramka == null)
                    answer.Add(select[i]);
                else
                    answer.Insert(IndexRamkaInsert(answer), select[i]);
            }
            //
            if (answer.Count > 0)
                select = answer;
        }

        private static int IndexRamkaInsert(List<IGraficObejct> select)
        {
            int last_index = 0;
            for(int i = 0; i< select.Count; i++)
            {
                if (select[i] is RamkaStation)
                    last_index = i+1;
            }
            //
            return last_index;
        }
        /// <summary>
        /// выделяем границу элемента
        /// </summary>
        /// <param name="collection">коллекция отрисованных объектов</param>
        /// <param name="pointclick">точка касания мышью</param>
        /// <param name="activeselement">коллекция активных элементов</param>
        public static void SelectStroke(Dictionary<int,IGraficObejct> collection, Point pointclick, List<IGraficObejct> activeselement)
        {
            List<MinLenght> lenght = new List<MinLenght>();
            //
            foreach (KeyValuePair<int, IGraficObejct> el in collection)
            {
                el.Value.CreateCollectionLines();
                DeleteAllActive(activeselement);
                //
                if (el.Value is IShowSettings)
                {
                    if (HitPoint(pointclick, el.Value, lenght))
                    {
                        el.Value.SelectStroke = true;
                        el.Value.SelectColor();
                        activeselement.Add(el.Value);
                        lenght.Clear();
                        return;
                    }
                }
                else
                {
                    if (HitStroke(pointclick, el.Value))
                    {
                       // DeleteAllActive(activeselement);
                        el.Value.SelectStroke = true;
                        el.Value.SelectColor();
                        activeselement.Add(el.Value);
                        lenght.Clear();
                        return;
                    }
                }
                // }     
            }
            //
            IGraficObejct element = IsPoint(lenght);
            if (element != null)
            {
                element.SelectStroke = true;
                element.SelectColor();
                activeselement.Add(element);
                lenght.Clear();
            }
            else
                lenght.Clear();
        }

        /// <summary>
        /// Определяем находится ли курсор мыши над границей или объектом
        /// </summary>
        /// <param name="collection">коллекция отрисованных элементов</param>
        /// <param name="pointclick">координаты мыши</param>
        public static MouseSelect IsSelectElementMouse(Dictionary<int, IGraficObejct> collection, Point pointclick)
        {
            MouseSelect isselect = new MouseSelect();
            foreach (KeyValuePair<int, IGraficObejct> el in collection)
            {
                el.Value.CreateCollectionLines();
                //
                if (el.Value is IShowSettings)
                {
                    if (HitStrokeOne(el.Value.Lines, pointclick, el.Value, el.Value.StrokeThickness))
                    {
                        isselect.mousefill = true;
                        return isselect;
                    }
                }
                else
                {
                    if (HitObject(el.Value.Lines, el.Value.Points, pointclick))
                    {
                        isselect.mousefill = true;
                        return isselect;
                    }
                    else
                    {
                        //
                        AligmentStroke aligment = HitStrokeMouse(pointclick, el.Value);
                        if (aligment != AligmentStroke.none)
                        {
                            isselect.mousestroke = true;
                            isselect.aligment = aligment;
                            return isselect;
                        }
                    }
                }
            }
            //
            return isselect;
        }

        public static void DeleteActiveStroke(List<IGraficObejct> activeselement)
        {
            for (int i = 0; i < activeselement.Count; i++)
            {
                if (activeselement[i].SelectStroke)
                {
                    activeselement[i].DefaultColor();
                    activeselement.RemoveAt(i);
                    i--;
                }
            }
        }

        private static void DeleteAllActive(List<IGraficObejct> activeselement)
        {
            for (int i = 0; i < activeselement.Count; i++)
                activeselement[i].DefaultColor();
            activeselement.Clear();
        }

        /// <summary>
        /// выбор рамкой нескольких объектов
        /// </summary>
        /// <param name="collection">коллекция отрисованных объектов</param>
        /// <param name="ramka">рамка выделения</param>
        /// <param name="direction">направление выделения</param>
        public static void SelectRamka(Dictionary<int,IGraficObejct> collection, Rectangle ramka, double direction, List<IGraficObejct> activeselement)
        {
            //очищаем коллекцию активных элементов
            DeleteAllActive(activeselement);
            //
            List<IGraficObejct> select = new List<IGraficObejct>();
            foreach (KeyValuePair<int,IGraficObejct> el in collection)
            {
                //GraficObejct geometry = el as GraficObejct;
                //if (geometry != null)
                //{
                    el.Value.CreateCollectionLines();
                    if (direction >= 0)
                    {
                        if (IsObjectAllRec(el.Value.Points, ramka))
                        {
                            el.Value.Selectobject = true;
                            el.Value.SelectColor();
                            activeselement.Add(el.Value);
                        }
                    }
                    else
                    {
                        if (IsObjectIntersectionRec(el.Value.Points, el.Value.Lines, ramka))
                        {
                            el.Value.Selectobject = true;
                            el.Value.SelectColor();
                            activeselement.Add(el.Value);
                        }
                    }
                //}
            }
        }

        /// <summary>
        /// Определяем пересекаются ли линии
        /// </summary>
        /// <param name="pa1">первая точка первой линии</param>
        /// <param name="pa2">вторая точка первой линии</param>
        /// <param name="pb1">первая точка второй линии</param>
        /// <param name="pb2">второй точка второй линии</param>
        /// <returns></returns>
         static bool IsLinePartsIntersected(Point pa1, Point pa2, Point pb1, Point pb2)
        {
             double v1 = (pb2.X - pb1.X) * (pa1.Y - pb1.Y) - (pb2.Y - pb1.Y) * (pa1.X - pb1.X);
             double v2 = (pb2.X - pb1.X) * (pa2.Y - pb1.Y) - (pb2.Y - pb1.Y) * (pa2.X - pb1.X);
             double v3 = (pa2.X - pa1.X) * (pb1.Y - pa1.Y) - (pa2.Y - pa1.Y) * (pb1.X - pa1.X);
             double v4 = (pa2.X - pa1.X) * (pb2.Y - pa1.Y) - (pa2.Y - pa1.Y) * (pb2.X - pa1.X);

             if ((v1 * v2 < 0) && (v3 * v4 < 0))
                 return true;
             else return false;
        }
        /// <summary>
        /// определяем попадание на границу объекта принадлежит ли точка объекту
        /// </summary>
        /// <returns></returns>
        static bool IsIntersectedBorder(Point p1, Point p2, Point PointX, ref Point result, double strokethickness)
        {
            double a = DistancePoint(p1, PointX);
            double b = DistancePoint(p2, PointX);
            double c = DistancePoint(p1, p2);
            double angle1 = AngleTriangle(a, b, c);
            double angle2 =  AngleTriangle(b, a, c);
            //
            if (angle1 >= 0 && angle2 > 0)
            {
                //
                double sin = Sin(angle1) * b;
                if ((sin) <= (strokethickness))
                {
                    result = GetPoint(p1, p2, PointX);
                    return true;
                }
            }
            return false;
        }

        static bool IsIntersectedBorder(Point p1, Point p2, Point PointX, double strokethickness)
        {
            double a = DistancePoint(p1, PointX);
            double b = DistancePoint(p2, PointX);
            double c = DistancePoint(p1, p2);
            double angle1 = AngleTriangle(a, b, c);
            double angle2 = AngleTriangle(b, a, c);
            //
            if (angle1 >= 0 && angle2 > 0)
            {
                //
                double sin = Sin(angle1) * b;
                if ((sin) <= (strokethickness))
                {
                    return true;
                }
            }
            return false;
        }

        private static Point GetPoint(Point p1, Point p2, Point n)
        {
            double A = p1.Y - p2.Y;
            double B = p2.X - p1.X;
            double C = p2.Y * p1.X - p2.X * p1.Y;
            double k = -(A / B);
            double b1 = -(C / B);

            if (k != 0)
            {
                if (double.IsInfinity(k))
                {
                    return new Point(p1.X, n.Y);
                }
                else
                {
                    double b2 = (n.Y + (n.X / k));
                    double x = (k * (b2 - b1)) / (k * k + 1);
                    double y = k * x + b1;
                    return new Point(x, y);
                }
            }
            else
            {
                return new Point(n.X, b1);
            }
        }

        static  private double AngleTriangle(double a, double b, double c)
        {
            return ((Math.Pow(b,2) + Math.Pow(c,2) - Math.Pow(a,2))/(2*b*c));
        }

        static private double Sin(double cos)
        {
            return (Math.Sqrt(1-cos*cos));
        }
        /// <summary>
        /// определяем расстояние между точками
        /// </summary>
        /// <param name="p1">точка номер один</param>
        /// <param name="p2">точка номер два</param>
        /// <returns></returns>
        static private double DistancePoint(Point p1, Point p2)
        {
            double X = Math.Pow(p2.X - p1.X, 2);
            double Y = Math.Pow(p2.Y - p1.Y, 2);
            double answer = Math.Sqrt(X+Y);
            return answer;
        }
        /// <summary>
        /// определяем попадает ли точка в области рамки
        /// </summary>
        /// <param name="points">координаты точка</param>
        /// <param name="ramka">рамка</param>
        /// <returns></returns>
        static bool IsPontRec(Point point, Rectangle ramka)
        {
            bool Top = IsLinePartsIntersected(point, new Point(point.X, point.Y - ramka.Height), new Point(ramka.Margin.Left, ramka.Margin.Top), new Point(ramka.Margin.Left + ramka.Width, ramka.Margin.Top));
            bool Bottom = IsLinePartsIntersected(point, new Point(point.X, point.Y + ramka.Height), new Point(ramka.Margin.Left, ramka.Margin.Top + ramka.Height), new Point(ramka.Margin.Left + ramka.Width, ramka.Margin.Top + ramka.Height));
            bool Left = IsLinePartsIntersected(point, new Point(point.X - ramka.Width, point.Y), new Point(ramka.Margin.Left, ramka.Margin.Top), new Point(ramka.Margin.Left, ramka.Margin.Top + ramka.Height));
            bool Right = IsLinePartsIntersected(point, new Point(point.X + ramka.Width, point.Y), new Point(ramka.Margin.Left + ramka.Width, ramka.Margin.Top), new Point(ramka.Margin.Left + ramka.Width, ramka.Margin.Top + ramka.Height));
            if (Top && Bottom && Left && Right)
                return true;
            else return false;
        }

        static bool IsLineRec(Line line, Rectangle ramka)
        {
            Point point1 = new Point(line.X1, line.Y1);
            Point point2 = new Point(line.X2, line.Y2);
            //проверяем пересечение с верхней линией
            if (IsLinePartsIntersected(point1, point2, new Point(ramka.Margin.Left, ramka.Margin.Top), new Point(ramka.Margin.Left + ramka.Width, ramka.Margin.Top)))
                return true;
            //проверяем пересечение с нижней линией
            if (IsLinePartsIntersected(point1, point2, new Point(ramka.Margin.Left, ramka.Margin.Top + ramka.Height), new Point(ramka.Margin.Left + ramka.Width, ramka.Margin.Top + ramka.Height)))
                return true;
            //проверяем пересечение с левой линией
            if (IsLinePartsIntersected(point1, point2, new Point(ramka.Margin.Left, ramka.Margin.Top), new Point(ramka.Margin.Left, ramka.Margin.Top + ramka.Height)))
                return true;
            //проверяем пересечение с правой линией
            if (IsLinePartsIntersected(point1, point2, new Point(ramka.Margin.Left + ramka.Width, ramka.Margin.Top), new Point(ramka.Margin.Left + ramka.Width, ramka.Margin.Top + ramka.Height)))
                return true;
            return false;
        }
        /// <summary>
        /// проверяем попадание объекта в рамку полностью
        /// </summary>
        /// <param name="_point">кочки объекта</param>
        /// <param name="ramka">рамка куда необходимо попасть</param>
        /// <returns></returns>
        static bool IsObjectAllRec(PointCollection _point, Rectangle ramka)
        {
            foreach (Point point in _point)
            {
                if (!IsPontRec(point, ramka))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// проверяем попадание объекта в рамку частично
        /// </summary>
        /// <param name="_point">кочки объекта</param>
        /// <param name="ramka">рамка куда необходимо попасть</param>
        /// <returns></returns>
        static bool IsObjectIntersectionRec(PointCollection _point, List<Line> _lines, Rectangle ramka)
        {
            //проверяем вхождение вершин рамки
            foreach (Point point in _point)
            {
                if (IsPontRec(point, ramka))
                    return true;
            }
            //проверяем пересечение с границами 
            foreach (Line lin in _lines)
            {
                if (IsLineRec(lin, ramka))
                    return true;
            }
            //
            return false;
        }

        public static Point MaxHeightWidth(PointCollection points)
        {
            Point max = new Point(double.MinValue, double.MinValue);
            Point min = new Point(double.MaxValue, double.MaxValue);
            foreach (Point point in points)
            {
                if (point.X > max.X)
                    max.X = point.X;
                //
                if (point.Y > max.Y)
                    max.Y = point.Y;
                //
                if (point.X < min.X)
                    min.X = point.X;
                //
                if (point.Y < min.Y)
                    min.Y = point.Y;
            }
            //
            return new Point(max.X - min.X, max.Y - min.Y);
        }

        /// <summary>
        /// определяем попадание в объект
        /// </summary>
        /// <param name="_lines">коллекция линий</param>
        /// <param name="_points">коллекция точек</param>
        /// <param name="pointclick">точка касания</param>
        /// <returns></returns>
        static bool HitObject(List<Line> _lines, PointCollection _points, Point pointclick)
        {
            Point max = MaxHeightWidth(_points);
            //направления
            bool Top = false;
            bool Bottom = false;
            bool Left = false;
            bool Right = false;
            //
            foreach (Line lin in _lines)
            {
                if (IsLinePartsIntersected(pointclick, new Point(pointclick.X, pointclick.Y - max.Y), new Point(lin.X1, lin.Y1), new Point(lin.X2, lin.Y2)))
                    Top = true;
                //
                if (IsLinePartsIntersected(pointclick, new Point(pointclick.X, pointclick.Y + max.Y), new Point(lin.X1, lin.Y1), new Point(lin.X2, lin.Y2)))
                    Bottom = true;
                //
                if (IsLinePartsIntersected(pointclick, new Point(pointclick.X - max.X, pointclick.Y), new Point(lin.X1, lin.Y1), new Point(lin.X2, lin.Y2)))
                    Left = true;
                //
                if (IsLinePartsIntersected(pointclick, new Point(pointclick.X + max.X, pointclick.Y), new Point(lin.X1, lin.Y1), new Point(lin.X2, lin.Y2)))
                    Right = true;
            }
            //
            if (Top && Bottom && Left && Right)
                return true;
            else
                return false;
        }
        /// <summary>
        /// определяем попадание награницу объект
        /// </summary>
        /// <param name="pointclick">точка касания</param>
        /// <returns></returns>
        static bool HitStroke(Point pointclick, IGraficObejct geometry)
        {
            //
            foreach (Line line in geometry.Lines)
            {
                if (IsIntersectedBorder(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2), pointclick, geometry.StrokeThickness))
                {
                    geometry.Index = geometry.FindIndexLine(new Line() { X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2 });
                    if (geometry.Index != null)
                        return true;
                }
            }
          //  geometry.Index = null;
            return false;
        }

        static AligmentStroke HitStrokeMouse(Point pointclick, IGraficObejct geometry)
        {
            //
            foreach (Line line in geometry.Lines)
            {
                if (IsIntersectedBorder(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2), pointclick, geometry.StrokeThickness))
                {
                    AligmentStroke answer = geometry.FindLineAligment(new Line() { X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2 });
                    if (answer != AligmentStroke.none)
                        return answer;
                }
            }
            return AligmentStroke.none;
        }

        static bool HitPoint(Point pointclick, IGraficObejct geometry, List<MinLenght> lenght)
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftAlt))
            {
                foreach (Line line in geometry.Lines)
                {
                    Point result = new Point();
                    if (IsIntersectedBorder(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2), pointclick, ref result,geometry.StrokeThickness))
                    {
                        LineHelp linehp = geometry as LineHelp;
                        LinePeregon peregon = geometry as LinePeregon;
                        if (linehp != null || peregon != null)
                        {
                            if (peregon != null)
                            {
                                peregon.AddNewPointIndex(new Line() { X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2 }, result);
                                return true;
                            }
                            //
                            if (linehp != null)
                            {
                                linehp.AddNewPointIndex(new Line() { X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2 }, result);
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                IsIntersectedPoint(pointclick, geometry, geometry.Points, lenght);
                //if (IsIntersectedPoint(pointclick, geometry, geometry.Points))
                //    return true;
            }
            //
           // geometry.Index = null;
            return false;
        }

        static bool HitStrokeOne(List<Line> _lines, Point pointclick, IGraficObejct geometry, double scroll)
        {
            foreach (Line line in _lines)
            {
                if (IsIntersectedBorder(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2), pointclick,geometry.StrokeThickness))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// определяем попадание в точку
        /// </summary>
        /// <returns></returns>
        static bool IsIntersectedPoint( Point pointclick, IGraficObejct geometry, PointCollection points, List<MinLenght> lenght )
        {
            //
            foreach (Point poi in points)
            {
                if ((pointclick.X - (geometry.RamkaAllocation / 2) * geometry.StrokeThickness <= poi.X) && (pointclick.X + (geometry.RamkaAllocation / 2) * geometry.StrokeThickness >= poi.X) &&
                    (pointclick.Y - (geometry.RamkaAllocation / 2) * geometry.StrokeThickness <= poi.Y) && (pointclick.Y + (geometry.RamkaAllocation / 2) * geometry.StrokeThickness >= poi.Y))
                {
                    lenght.Add(new MinLenght() { Lenght = Math.Sqrt(Math.Pow(poi.X - pointclick.X, 2) + Math.Pow(poi.Y - pointclick.Y, 2)), P = new Point(poi.X, poi.Y), Element = geometry });
                    //geometry.Index = geometry.FindIndexLine(new Line() { X1 = poi.X, X2 = 0, Y1 = poi.Y, Y2 = 0 });
                    //if (geometry.Index != null)
                    //    return true;
                }
            }
            //
            //if (lenght.Count >= 1)
            //{
            //    lenght.Sort(
            //      (x, y) =>
            //    {
            //        if (x.Lenght > y.Lenght) return 1;
            //        if (x.Lenght < y.Lenght) return -1;
            //        return 0;
            //    }
            //  );
            //    //
            //    geometry.Index = geometry.FindIndexLine(new Line() { X1 = lenght[0].P.X, X2 = 0, Y1 = lenght[0].P.Y, Y2 = 0 });
            //    if (geometry.Index != null)
            //        return true;
            //}
            geometry.Index = null;
            return false;
        }

        static IGraficObejct IsPoint(List<MinLenght> lenght)
        {
            if (lenght.Count >= 1)
            {
                lenght.Sort(
                  (x, y) =>
                  {
                      if (x.Lenght > y.Lenght) return 1;
                      if (x.Lenght < y.Lenght) return -1;
                      return 0;
                  }
              );
                //
                lenght[0].Element.Index = lenght[0].Element.FindIndexLine(new Line() { X1 = lenght[0].P.X, X2 = 0, Y1 = lenght[0].P.Y, Y2 = 0 });
                if (lenght[0].Element.Index != null)
                    return lenght[0].Element;
            }
            //
            return null;
        }


    }
}
