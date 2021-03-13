using System.ComponentModel.DataAnnotations;

namespace FiguresExample.Api.Models
{
    /// <remarks>Я бы навешал атрибутов для валидации моделей API</remarks>
    public class Position
    {
        // TODO: Вынести в файл ресурсов сообщение об ошибке
        [Required(ErrorMessage = "Укажите тип фигуры")]
        public string Type { get; set; }

        public float SideA { get; set; }
        public float SideB { get; set; }
        public float SideC { get; set; }

        // TODO: Вынести в файл ресурсов сообщение об ошибке
        [Required(ErrorMessage = "Количество - обязательный параметр")]
        public int Count { get; set; }
    }
}