using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Serialization.Json
{
    /// <summary>
    /// Represents a JSON-specific <see cref="IConverter{TFrom, TTo}"/> for converting objects to <see cref="JObject"/>s.
    /// </summary>
    /// <typeparam name="T">The type of object to convert from.</typeparam>
    public interface IToJsonConverter<T> : IConverter<T, JToken>
    { }

    /// <summary>
    /// Represents a JSON-specific <see cref="IConverter{TFrom, TTo}"/> for converting objects from <see cref="JToken"/>s.
    /// </summary>
    /// <typeparam name="T">The type of object to convert to.</typeparam>
    public interface IFromJsonConverter<T> : IConverter<JToken, T>
    { }

    public static class JsonConverterExtensions
    {
        /// <summary>
        /// JSON-specific. Converts an object from a <see cref="JToken"/> to an object of type <typeparamref name="T"/> using a <see cref="IFromJsonConverter{T}"/>, respecting <see cref="JTokenType.Null"/>.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="jsonConverter">The <see cref="IFromJsonConverter{T}"/> to use for conversion.</param>
        /// <param name="json">The input <see cref="JToken"/>.</param>
        public static T GetTo<T>(this IFromJsonConverter<T> jsonConverter, JToken json)
        {
            if (json.Type == JTokenType.Null || jsonConverter == null || !jsonConverter.CanConvert(json))
            {
                return default(T);
            }
            else
            {
                return jsonConverter.Convert(json);
            }
        }

        /// <summary>
        /// JSON-specific. Converts an object from a <see cref="JToken"/> to an object of type <typeparamref name="T"/> using a collection of <see cref="IFromJsonConverter{T}"/>, respecting <see cref="JTokenType.Null"/>.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="jsonConverters">The collection of <see cref="IFromJsonConverter{T}"/> objects to use for conversion. Chooses the first or default where <see cref="IConverter{TFrom, TTo}.CanConvert(TFrom)"/> returns true.</param>
        /// <param name="json">The input <see cref="JToken"/>.</param>
        public static T GetTo<T>(this IEnumerable<IFromJsonConverter<T>> jsonConverters, JToken json)
        {
            var converter = jsonConverters.FirstOrDefault(c => c.CanConvert(json));
            if (json.Type == JTokenType.Null || converter == null || !converter.CanConvert(json))
            {
                return default(T);
            }
            else
            {
                return converter.GetTo(json);
            }
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the given <see cref="JToken"/> is not <see cref="JTokenType.Null"/> not null, and has a "Type" property with a given <see cref="string"/> value.
        /// </summary>
        /// <param name="json">The <see cref="JToken"/> to check.</param>
        /// <param name="type">The requested value of the "Type" property.</param>
        public static bool IsJsonType(this JToken json, string type)
        {
            if (json.Type == JTokenType.Null)
            {
                return false;
            }
            else if (json["Type"]?.Type == JTokenType.String)
            {
                return (string)json["Type"] == type;
            }
            else
            {
                return false;
            }
        }
    }
}
