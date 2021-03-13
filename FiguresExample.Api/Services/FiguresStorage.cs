using System;
using FiguresExample.Api.Abstract;

namespace FiguresExample.Api.Services
{
    /// <remarks>
    ///     Сделаем нестатическим и создадим (и реализуем) интерфейс, чтобы можно было использовать в тестах.
    ///     Даже если он должен быть в одном экземпляре, то ничто не мешает нам использовать Singleton.
    ///     Зато его теперь тоже можно тестировать и делать заглушки (Mock)
    /// </remarks>
    internal sealed class FiguresStorage : IFiguresStorage
    {
        // Лучше инжектировать IRedisClient. Если он должен быть в одном экземпляре, то ничто не мешает нам использовать Singleton
        private readonly IRedisClient _redisClient;

        public FiguresStorage(IRedisClient redisClient)
        {
            // Без redisClient наш сервис не будет работать, поэтому, если его нам не передали, то рушим всё и сразу
            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
        }

        /// <remarks>Я бы добавил CancellationTokenSource, если бы конечно IRedisClient его поддерживал</remarks>
        public bool CheckIfAvailable(string type, int count) => _redisClient.Get(type) >= count;

        /// <remarks>Я бы добавил CancellationTokenSource, если бы конечно IRedisClient его поддерживал</remarks>
        public void Reserve(string type, int count)
        {
            var current = _redisClient.Get(type);

            _redisClient.Set(type, current - count);
        }
    }
}