using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiguresExample.Api.Models
{
    public class Cart
    {
        // TODO: Вынести в файл ресурсов сообщение об ошибке
        [Required(ErrorMessage = "Positions - обязательный параметр")]
        public List<Position> Positions { get; set; }
    }
}