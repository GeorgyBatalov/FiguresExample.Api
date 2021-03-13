using System;

namespace FiguresExample.Api.Models.Figures
{
    internal sealed class Circle : Figure
    {
        public override bool Validate(out string errorMessage)
        {
            // Формирование исключения - очень дорогая операция. Поменяем логику
            //if (SideA < 0)
            //    throw new InvalidOperationException("Circle restrictions not met");

            if (SideA < 0)
            {
                errorMessage = "Circle restrictions not met";

                return false;
            }

            errorMessage = null;

            return true;
        }

        protected override double GetArea() => Math.PI * SideA * SideA;

        /// <remarks>Добавил в расчёт количество данных позиций</remarks>
        public override decimal GetTotalPrice() => (decimal) GetArea() * 0.9m * Count;
    }
}