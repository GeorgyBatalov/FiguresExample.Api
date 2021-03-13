using System;

namespace FiguresExample.Api.Models.Figures
{
    internal sealed class Square : Figure
    {
        // Допуск может быть любым, возьмём минимальное положительное число float
        private static readonly float _tolerance = float.Epsilon;

        public override decimal GetTotalPrice() => decimal.Zero;

        public override bool Validate(out string errorMessage)
        {
            errorMessage = null;

            // Формирование исключения - очень дорогая операция. Поменяем логику
            //if (SideA < 0)
            //    throw new InvalidOperationException("Square restrictions not met");

            //if (SideA != SideB)
            //    throw new InvalidOperationException("Square restrictions not met");

            if (SideA < 0)
                errorMessage = "Square restrictions not met";

            // Не совсем корректно так сравнивать float числа
            // if (SideA != SideB)

            // Сравним числа с указанным допуском
            if (Math.Abs(SideA - SideB) > _tolerance)
                errorMessage = "Square restrictions not met";

            return errorMessage == null;
        }

        protected override double GetArea() => SideA * SideA;
    }
}