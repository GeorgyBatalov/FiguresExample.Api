namespace FiguresExample.Api.Abstract
{
    /// <remarks>
    ///     Сделаем интерфейс публичным, чтобы можно было использовать в тестах и инжектировать или вообще проксировать другое
    ///     хранилище или не хранилище, а что угодно
    /// </remarks>
    public interface IRedisClient
    {
        /// <remarks>Сделал бы асинхронным и добавил бы CancelerationToken, если клиент такое позволяет</remarks>
        int Get(string type);

        /// <remarks>Сделал бы асинхронным и добавил бы CancelerationToken, если клиент такое позволяет</remarks>
        void Set(string type, int current);
    }
}