using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Serialization
{
    /// <summary>
    /// Represents a service which can convert an object of type <see cref="TFrom"/> to an object of type <see cref="TTo"/>.
    /// </summary>
    public interface IConverter<TFrom, TTo>
    {
        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the <see cref="IConverter{TFrom, TTo}"/> can convert the given <see cref="TFrom"/> object.
        /// </summary>
        /// <param name="item">The <see cref="TFrom"/> object to check.</param>
        bool CanConvert(TFrom item);

        /// <summary>
        /// Converts an object of type <see cref="TFrom"/> to an object of type <see cref="TTo"/>.
        /// /// </summary>
        /// <param name="item">The <see cref="TFrom"/> object to convert.</param>
        TTo Convert(TFrom item);
    }

    /// <summary>
    /// This exception is thrown when an error occurs while using a <see cref="IConverter{TFrom, TTo}"/> instance.
    /// </summary>
    [Serializable]
    public class ConvertException : Exception
    {
        public ConvertException() { }
        public ConvertException(string message) : base(message) { }
        public ConvertException(string message, Exception inner) : base(message, inner) { }
    }

    public static class ConverterExtensions
    {
        /// <summary>
        /// Attempts to convert an item of type <typeparamref name="TFrom"/> to an item of type <typeparamref name="TTo"/> using an <see cref="IConverter{TFrom, TTo}"/>. If the <paramref name="item"/> is null, the <paramref name="converter"/> is null, or <see cref="IConverter{TFrom, TTo}.CanConvert(TFrom)"/> returns false, the method returns a default value.
        /// </summary>
        /// <param name="converter">The <see cref="IConverter{TFrom, TTo}"/> to use for conversion.</param>
        /// <param name="item">The <typeparamref name="TFrom"/> to convert.</param>
        public static TTo GetTo<TFrom, TTo>(this IConverter<TFrom, TTo> converter, TFrom item)
        {
            if (item == null || converter == null || !converter.CanConvert(item))
            {
                return default(TTo);
            }
            else
            {
                return converter.Convert(item);
            }
        }

        /// <summary>
        /// Attempts to convert an item of type <typeparamref name="TFrom"/> to an item of type <typeparamref name="TTo"/> using a collection of <see cref="IConverter{TFrom, TTo}"/>. If the <paramref name="item"/> is null, or none of the <paramref name="converters"/> return true for <see cref="IConverter{TFrom, TTo}.CanConvert(TFrom)"/>, the method returns a default value.
        /// </summary>
        /// <param name="converters">The collection of <see cref="IConverter{TFrom, TTo}"/> to use for conversion.</param>
        /// <param name="item">The <typeparamref name="TFrom"/> to convert.</param>
        public static TTo GetTo<TFrom, TTo>(this IEnumerable<IConverter<TFrom, TTo>> converters, TFrom item)
        {
            var converter = converters.FirstOrDefault(c => c.CanConvert(item));
            if (item == null || converter == null || !converter.CanConvert(item))
            {
                return default(TTo);
            }
            else
            {
                return converter.GetTo(item);
            }
        }
    }
}
