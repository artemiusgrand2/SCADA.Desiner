using SCADA.Desiner.BaseElement;
using SCADA.Desiner.CommandElement;

namespace SCADA.Desiner.HelpsProject
{
    class ScrollStrokeThickess
    {
        /// <summary>
        /// анализируем масштабный коэффициент
        /// </summary>
        public static void SetScrollStrokeThickess(double scroll_old, double scroll_now)
        {
            //сигнал
            Signal.strokethickness *= (scroll_now / scroll_old);
            ArrowMove.strokethickness *= (scroll_now / scroll_old);
            //светофор
            LightShunting.strokethickness *= (scroll_now / scroll_old);
            LightTrain.strokethickness *= (scroll_now / scroll_old);

            ButtonStation.strokethickness *= (scroll_now / scroll_old);
            Area.strokethickness *= (scroll_now / scroll_old);
            Traintrack.strokethickness *= (scroll_now / scroll_old);
            ButtonCommand._strokethickness *= (scroll_now / scroll_old);
            //КГУ
            KGU.strokethickness *= (scroll_now / scroll_old);
            KGU.heightdefult *= (scroll_now / scroll_old);
            KGU.widthdefult *= (scroll_now / scroll_old);
            //КТСМ
            KTCM.strokethickness *= (scroll_now / scroll_old);
            KTCM.heightdefult *= (scroll_now / scroll_old);
            KTCM.widthdefult *= (scroll_now / scroll_old);
            //переезд
            Move.strokethickness *= (scroll_now / scroll_old);
            Move.heightdefult *= (scroll_now / scroll_old);
            //номера поездов
            NumberTrain.strokethickness *= (scroll_now / scroll_old);
            NameStation.strokethickness *= (scroll_now / scroll_old);
            TextHelp.strokethickness *= (scroll_now / scroll_old);
            TimeForm.strokethickness *= (scroll_now / scroll_old);
        }
    }
}
