using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FiguresExample.Api.Abstract;
using FiguresExample.Api.Extensions;
using FiguresExample.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FiguresExample.Api
{
    [ApiController]
    [Route("[controller]")]
    public sealed class FiguresController : ControllerBase
    {
        private readonly IFiguresStorage _figuresStorage;
        private readonly ILogger<FiguresController> _logger;
        private readonly IOrderStorage _orderStorage;

        public FiguresController(ILogger<FiguresController> logger, IOrderStorage orderStorage, IFiguresStorage figuresStorage)
        {
            // Сделаем сервис логирования обязательным, раз уж у нас высоконагруженный контроллер/экшен
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Без сервиса orderStorage контроллер не сможет выполнять свои функции и, если его не инжектировали, то рушим всё и сразу
            _orderStorage = orderStorage ?? throw new ArgumentNullException(nameof(orderStorage));

            // Вместо ссылки на статический сервис, сделаем его инжектируемым, чтобы в будущем можно было легко протестировать и ничто нам не мешает сделать его как Singleton, но не здесь
            _figuresStorage = figuresStorage ?? throw new ArgumentNullException(nameof(orderStorage));
        }

        // хотим оформить заказ и получить в ответе его стоимость
        [HttpPost("order")] // Оставляем название экшена, но имя метода меняем, чтобы соответствовало naming convention
        public async Task<ActionResult> OrderAsync(Cart cart)
        {
            // Заказ с единственным полем Positions, который при этом равен null, нам точно не нужен, сразу возвращаем Bad Request (400)
            if (cart?.Positions == null)
            {
                // Перед тем как вернуть Bad Request (400), логируем Warning, желательно ещё и передать в лог сам запрос
                _logger.Log(LogLevel.Warning, $"Bad Request {nameof(cart.Positions)} is null");
                return new BadRequestResult();
            }

            // ПОМЕНЯЛ МЕСТАМИ проверку доступности позиций для заказа и преобразование заказа из API моделей в доменные модели + ВАЛИДАЦИЮ,
            // так как это всё происходит в коде, а значит быстро (должно быть быстрее, чем обращение к сервису FiguresStorage -> IRedisStorage).
            // Если что-то пойдёт не так или сработает валидация, то мы вернём отрицательный ответ пользователю, не обращаясь и не нагружая FiguresStorage -> IRedisStorage.
            // А в задании было сказано про несколько инстансов этого сервиса (API), а значит Redis (или что-то другое, что запроксировали как Redis) будут закидывать запросами сразу несколько инстансов (может быть их штук 100 запустят потом).
            // В любом случае, если есть валидация, то она должна сработать раньше чем обращения к другим сервисам

            // Вынесем логику по созданию и маппингу конкретных типов в отдельный экстеншен и избавимся от конструкции switch case (с оговорками)
            var order = cart.ToOrder(out var errorMessages);

            if (errorMessages?.Any() == true)
            {
                var errorMessage = $"Bad Request. Validation errors: {string.Join(Environment.NewLine, errorMessages)}";

                // Перед тем как вернуть Bad Request (400) response, логируем Warning 
                _logger.Log(LogLevel.Warning, errorMessage);
                // Пользователю тоже даём обратную связь
                return BadRequest(errorMessage);
            }

            // И только теперь мы начинаем проверять доступность позиции для заказа (после преобразования API моделей и валидации)
            // TODO: выбрать один из вариантов проверки, в зависимости от среднего количества позиций и скорости работы сервиса IFiguresStorage
            // ВАРИАНТ 1: Если проверка работает приемлемо быстро и позиций обычно не бывает много, то
            if (!AreAllPositionsAvailableSlow(cart, out var badPosition))
            // Иначе, если проверка выше занимает много времени (зависит от реализации IFiguresStorage), то можно попробовать делать такую проверку и параллельно:
            // ВАРИАНТ 2:
            //if (!AreAllPositionsAvailableFast(cart, out var badPosition))
            {
                // Перед тем как вернуть Bad Request (400) response, логируем Warning 
                _logger.Log(LogLevel.Warning, $"Bad Request for type '{badPosition?.Type}' with count '{badPosition?.Count}'");
                return new BadRequestResult();
            }

            foreach (var position in cart.Positions) _figuresStorage.Reserve(position.Type, position.Count);

            decimal result;

            try
            {
                // Так как, реализации IOrderStorage я не увидел, то могу лишь предполагать, что в качестве результата она возвращает стоимость, которая теперь будет вычисляться примерно так:
                // result = order.GetTotalPrice();
                // и, за счёт реализации паттерна Компоновщик, стоимость легко посчитается и не получится добавить фигуры без реализации подсчёта цены

                // Используя await мы получим конкретное исключение вместо AggregateException
                result = await _orderStorage.SaveAsync(order);
            }
            catch (Exception e)
            {
                // Перед тем как пробросить эксепшен - логируем его
                _logger.Log(LogLevel.Error, e, e.Message);

                // Именно так, чтобы сохранить полный stack trace
                throw;
            }

            return new OkObjectResult(result);

            // Используя result.Result мы получим AggregateException (с сообщением о том, что произошла одна или более ошибок).
            // И получить ошибку можно будет только из InnerException или InnerExceptions, а полный StackTrace придётся собирать из полей StackTrace AggregateException и InnerException
            //var result = _orderStorage.SaveAsync(order);

            //return new OkObjectResult(result.Result);
        }

        /// <remarks>
        ///     Конечно же, если выберется такая проверка, то её лучше вынести в отдельный сервис, потому-что контроллер не должен
        ///     быть таким умным и многофункциональным.
        ///     Тут баланс между скоростью работы сервиса IFiguresStorage/ его нагруженностью / средним колличеством позиций
        ///     Ну и если позиций мало, сервис работает быстро, то этот метод лучше не использовать, так как генерация исключения
        ///     операция не дешёвая
        /// </remarks>
        // TODO: вынести в отлельный сервис
        private bool AreAllPositionsAvailableFast(Cart cart, out Position badPosition)
        {
            var cts = new CancellationTokenSource();

            Position badPositionLocal = null;

            try
            {
                cart.Positions.AsParallel().WithCancellation(cts.Token).ForAll(position =>
                {
                    // Если нашли недоступную позицию - отменяем все проверки (неплохо бы добавить CancellationToken и в IFiguresStorage)
                    if (!_figuresStorage.CheckIfAvailable(position.Type, position.Count))
                    {
                        badPositionLocal = position;

                        cts.Cancel();
                    }
                });
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (AggregateException e)
            {
                // Что-то пошло не так рушим всё и сразу
                // Перед тем как пробросить эксепшен - логируем
                _logger.Log(LogLevel.Error, e, e.Message);

                // Именно так, чтобы сохранить полный stack trace
                throw;
            }
            finally
            {
                badPosition = badPositionLocal;

                cts.Dispose();
            }

            return true;
        }

        /// <remarks>
        ///     Если сервис IFiguresStorage работает быстро и среднеее количество позиций небольшое, то лучше использовать
        ///     этот метод
        /// </remarks>
        private bool AreAllPositionsAvailableSlow(Cart cart, out Position badPosition)
        {
            var badPositionLocal = cart.Positions.FirstOrDefault(position => !_figuresStorage.CheckIfAvailable(position.Type, position.Count));

            return (badPosition = badPositionLocal) == null;
        }
    }
}