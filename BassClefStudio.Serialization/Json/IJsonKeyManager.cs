using BassClefStudio.NET.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Serialization.Json
{
    /// <summary>
    /// Represents a JSON-specific implementation of <see cref="IKeyManager{TItem, TInput, TOutput}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item stored in the <see cref="IKeyManager{TItem, TInput, TOutput}"/>.</typeparam>
    public interface IJsonKeyManager<T> : IKeyManager<T, JToken, JArray> where T : IIdentifiable<string>
    { }

    /// <summary>
    /// The basic implementation of <see cref="IJsonKeyManager{T}"/>, using a <see cref="Dictionary{TKey, TValue}"/> as a backing store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonKeyManager<T> : IJsonKeyManager<T> where T : IIdentifiable<string>
    {
        public IFromJsonConverter<T> FromJsonConverter { get; set; }
        public IToJsonConverter<T> ToJsonConverter { get; set; }

        /// <summary>
        /// The <see cref="Dictionary{TKey, TValue}"/> where all objects in the <see cref="JsonKeyManager{T}"/> are stored.
        /// </summary>
        public Dictionary<string, T> AvailableObjects { get; }

        /// <summary>
        /// Creates an empty <see cref="JsonKeyManager{T}"/>.
        /// </summary>
        public JsonKeyManager()
        {
            AvailableObjects = new Dictionary<string, T>();
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            if (!AvailableObjects.ContainsKey(item.Id))
            {
                AvailableObjects.Add(item.Id, item);
            }
        }

        /// <inheritdoc/>
        public T this[string id] => AvailableObjects[id];

        /// <inheritdoc/>
        public void Clear() => AvailableObjects.Clear();

        /// <inheritdoc/>
        public JArray SeralizeObjects()
        {
            return new JArray(AvailableObjects.Values.Select(t => ToJsonConverter.GetTo(t)));
        }

        /// <inheritdoc/>
        public void DeserializeObjects(JToken json)
        {
            var objects = json.Select(j => FromJsonConverter.GetTo(j));
            foreach (T o in objects)
            {
                Add(o);
            }
        }
    }
}
