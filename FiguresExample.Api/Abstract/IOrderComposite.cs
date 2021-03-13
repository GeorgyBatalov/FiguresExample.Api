namespace FiguresExample.Api.Abstract
{
    /// <remarks>
    ///     Сделаем интефейс для реализации паттерна Компоновщик для заказа
    /// </remarks>
    internal interface IOrderComposite
    {
        decimal GetTotalPrice();
    }
}