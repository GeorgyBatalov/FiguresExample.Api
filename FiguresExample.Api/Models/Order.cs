using System.Collections.Generic;
using System.Linq;
using FiguresExample.Api.Abstract;
using FiguresExample.Api.Models.Figures;

namespace FiguresExample.Api.Models
{
    public class Order : IOrderComposite
    {
        public List<Figure> Positions { get; set; }

        /// <remarks>
        ///     Изменил название, так как GetTotal имеет двусмысленное название (общее количество товаров или итоговая цена)
        ///     и может вводить в заблуждение
        /// </remarks>
        public decimal GetTotalPrice() => Positions?.Sum(x => x.GetTotalPrice()) ?? decimal.Zero;
    }
}