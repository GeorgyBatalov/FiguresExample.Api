namespace FiguresExample.Api.Abstract
{
    public interface IFiguresStorage
    {
        /// <remarks>Сделал бы асинхронным и добавил бы CancelerationToken, если бы конечно IRedisClient его поддерживал</remarks>
        bool CheckIfAvailable(string type, int count);

        /// <remarks>Сделал бы асинхронным и добавил бы CancelerationToken, если бы конечно IRedisClient его поддерживал</remarks>
        void Reserve(string type, int count);
    }
}