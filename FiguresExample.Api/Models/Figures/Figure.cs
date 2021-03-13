using FiguresExample.Api.Abstract;

namespace FiguresExample.Api.Models.Figures
{
    /// <remarks>Теперь фигуры реализуют интерфейс IOrderComposite</remarks>
    public abstract class Figure : IOrderComposite
    {
        public float SideA { get; set; }
        public float SideB { get; set; }
        public float SideC { get; set; }

        /// <remarks>
        ///     Заметил, что это поле отсутствовало в Figure
        ///     Очевидно, что количество позиций должно влиять на суммарную стоимость позиции и заказа в целом
        /// </remarks>
        public int Count { get; set; }

        /// <remarks>
        ///     Каждая фигура теперь может сама посчитать свою стоимость.
        /// </remarks>
        public abstract decimal GetTotalPrice();

        /// <remarks>
        ///     На мой взгляд, эксепшен - это не очень ожидаемый и очевидный результат работы метода валидации.
        ///     К тому же, это очень дорогая операция.
        ///     Я бы исправил логику валидации с выбрасывания эксепшена на возврат Boolean значения,
        ///     так как выбрасывать эксепшен каждый раз когда мы хотим убедиться в валидности - значительно дороже, чем вернуть
        ///     Boolean и дальше это как-то обработать (например: получив отрицательный результат - сначала залогировать, а потом
        ///     выбросить эксепшен)
        /// </remarks>
        public abstract bool Validate(out string errorMessage);

        /// <remarks>Не увидел в коде использования этого метода сделал его protected. Ничего не мешает мне его потом сделать publuc</remarks>
        protected abstract double GetArea();
    }
}