using System;

namespace FiguresExample.Api.Models.Figures
{
    internal sealed class Triangle : Figure
    {
        /// <remarks>
        ///     Сделал метод статическим, так как ему не требуется внутренне состояние объекта и
        ///     статические методы работают, хоть и незначительно, но быстрее
        /// </remarks>
        private static bool CheckTriangleInequality(float a, float b, float c) => a < b + c;

        public override bool Validate(out string errorMessage)
        {
            if (CheckTriangleInequality(SideA, SideB, SideC)
                && CheckTriangleInequality(SideB, SideA, SideC)
                && CheckTriangleInequality(SideC, SideB, SideA))
            {
                errorMessage = null;

                return true;
            }

            errorMessage = "Triangle restrictions not met";

            return false;
        }

        protected override double GetArea()
        {
            var p = (SideA + SideB + SideC) / 2;
            return Math.Sqrt(p * (p - SideA) * (p - SideB) * (p - SideC));
        }

        /// <remarks>Добавил в расчёт количество данных позиций</remarks>
        public override decimal GetTotalPrice() => (decimal) GetArea() * 1.2m * Count;
    }
}