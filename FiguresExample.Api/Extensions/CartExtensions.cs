using System.Collections.Generic;
using System.Linq;
using FiguresExample.Api.Models;

namespace FiguresExample.Api.Extensions
{
    internal static class CartExtensions
    {
        internal static Order ToOrder(this Cart cart, out ICollection<string> errorMessages)
        {
            errorMessages = null;

            // В заказе одно единственное поле Positions и оно равно null - думаю нам такое не подходит
            // Вообще, в контроллере уже есть такая проверка, но мы не знаем кто и как в будущем
            // может использовать этот метод
            if (cart?.Positions == null)
                return null;

            var errorMessagesLocal = new List<string>(cart.Positions.Count);

            var order = new Order
            {
                Positions = cart.Positions.Select(p =>
                {
                    var figure = p.ToConcreteFigure(out var errorMessage);

                    if (!string.IsNullOrWhiteSpace(errorMessage))
                        errorMessagesLocal.Add(errorMessage);

                    return figure;
                }).ToList()
            };

            errorMessages = errorMessagesLocal;

            // Если есть ошибки, то, думаю, что не стоит возвращать половину результата,
            // чтобы не вводить клиента этого метода в заблуждение, что всё хорошо.
            // Пусть получчает null и изучает ошибки
            return errorMessages.Any() ? null : order;
        }
    }
}