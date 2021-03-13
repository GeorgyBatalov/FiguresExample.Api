using System.Threading.Tasks;
using FiguresExample.Api.Models;

namespace FiguresExample.Api.Abstract
{
    public interface IOrderStorage
    {
        /// <summary>Сохраняет оформленный заказ и возвращает сумму</summary>
        /// <remarks>Добавил суффикс, в соответствии с naming conventions</remarks>
        Task<decimal> SaveAsync(Order order);
    }
}