using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UssJuniorTest.DTO
{
    public class QueryParameters
    {
        [Required(ErrorMessage = "Начало интересующего промежутка обязательно")]
        [Description("Начало интересующего промежутка")]
        public DateTime Begin { get; set; }

        [Required(ErrorMessage = "Конец интересующего промежутка обязательно")]
        [Description("Конец интересующего промежутка")]
        public DateTime End { get; set; }

        [Description("Фильтрация по имени человека")]

        public string FilterByName { get; set; }

        [Description("Фильтрация по производителю автомобиля")]
        public string FilterByCarManufacturer { get; set; }

        [Description("Номер страницы")]
        public int Page { get; set; } = 1;

        [Description("Размер страницы")]
        public int PageSize { get; set; } = 10;

        [Description("Поле для сортировки (name, carmanufacturer)")]
        public string SortByNameOrManufacturer { get; set; }
    }
}
