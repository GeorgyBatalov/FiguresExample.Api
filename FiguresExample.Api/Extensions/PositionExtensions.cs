using System;
using System.Collections.Generic;
using System.Linq;
using FiguresExample.Api.Models;
using FiguresExample.Api.Models.Figures;

namespace FiguresExample.Api.Extensions
{
    internal static class PositionExtensions
    {
        private static readonly IDictionary<string, Type> NestedTypesDictionary;

        static PositionExtensions()
        {
            // Один единственный раз запустим построение словаря
            NestedTypesDictionary = BuildNestedTypesDictionary();
        }

        private static Dictionary<string, Type> BuildNestedTypesDictionary()
        {
            var baseType = typeof(Figure);

            var nestedTypes = baseType.Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(baseType))
                .ToList();

            // Заставляем разработчиков следить за наличием конструктора по умолчанию для всех типов наследуемых от типа Figure,
            // ну или вернёмся к switch case конструкции 
            if (nestedTypes.Any(x => !x.HasDefaultConstructor()))
                throw new Exception($"All nested types of type {nameof(Figure)} should have default constructor");

            return nestedTypes.ToDictionary(x => x.Name, x => x);
        }

        /// <returns>Проверяем есть ли конструктор по умолчанию</returns>
        private static bool HasDefaultConstructor(this Type t)
        {
            return // t.IsValueType || 
                t.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <remarks>Создадим экстеншен который будет создавать из объектов Position конкретные объекты Figure</remarks>
        internal static Figure ToConcreteFigure(this Position position, out string errorMessage)
        {
            errorMessage = null;

            if (position == null)
                return null;

            // Мы не готовы к такому варианту, поэтому сдаёмся сразу
            if (string.IsNullOrWhiteSpace(position.Type))
                throw new ArgumentNullException(nameof(position.Type));

            // Мы не готовы обслуживать запросы с неподдерживаемыми типами фигур, поэтому взрываем всё и сразу
            if (!NestedTypesDictionary.TryGetValue(position.Type, out var concreteType) || concreteType == null)
                throw new ArgumentOutOfRangeException(nameof(position.Type));

            var figure = (Figure) Activator.CreateInstance(concreteType);

            // У нас есть проверка на наличие конструктора по умолчанию, но на всякий случай ещё раз проверим, вдруг что-то пошло не так
            if (figure == null)
                throw new ArgumentOutOfRangeException(nameof(position.Type));

            figure.SideA = position.SideA;
            figure.SideB = position.SideB;
            figure.SideC = position.SideC;

            // Добавил поле с количеством позиций
            figure.Count = position.Count;

            // Если есть ошибки валидации, то, думаю, что не стоит возвращать невалидный результат,
            // чтобы не вводить клиента этого метода в заблуждение, что всё хорошо.
            // Пусть получает null и изучает ошибку
            return figure.Validate(out errorMessage) ? figure : null;
        }
    }
}