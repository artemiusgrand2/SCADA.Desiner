
namespace SCADA.Desiner.HelpsProject
{
    public class MyTable
    {
        /// <summary>
        /// идентификатор объекта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// название объекта
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// пояснения по объекту
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// номер станции слева
        /// </summary>
        public string Numberstationleft { get; set; }
        /// <summary>
        /// номер станции справа
        /// </summary>
        public string Numberstationright { get; set; }
        /// <summary>
        /// является ли светофор входным
        /// </summary>
        public bool IsInput { get; set; }
        /// <summary>
        /// Тип разъединителя
        /// </summary>
        public string TypeDis { get; set; }
        /// <summary>
        /// является ли объект видимым
        /// </summary>
        public bool IsVisible { get; set; }
        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// выделени ли элемент
        /// </summary>
        public bool IsSelect { get; set; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FileClick { get; set; }
    }
}
