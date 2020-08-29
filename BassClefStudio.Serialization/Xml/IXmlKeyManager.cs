using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BassClefStudio.Serialization.Xml
{
    /// <summary>
    /// Represents a JSON-specific implementation of <see cref="IKeyManager{TItem, TInput, TOutput}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item stored in the <see cref="IKeyManager{TItem, TInput, TOutput}"/>.</typeparam>
    public interface IXmlKeyManager<T> : IKeyManager<T, XElement, IEnumerable<XElement>> where T : IIdentifiable<string>
    { }

    /// <summary>
    /// The basic implementation of <see cref="IXmlKeyManager{T}"/>, using a <see cref="Dictionary{TKey, TValue}"/> as a backing store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlKeyManager<T> : IXmlKeyManager<T> where T : IIdentifiable<string>
    {
        public IFromXmlConverter<T> FromXmlConverter { get; set; }
        public IToXmlConverter<T> ToXmlConverter { get; set; }

        /// <summary>
        /// The <see cref="Dictionary{TKey, TValue}"/> where all objects in the <see cref="XmlKeyManager{T}"/> are stored.
        /// </summary>
        public Dictionary<string, T> AvailableObjects { get; }

        /// <summary>
        /// Creates an empty <see cref="XmlKeyManager{T}"/>.
        /// </summary>
        public XmlKeyManager()
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
        public IEnumerable<XElement> SeralizeObjects()
        {
            return AvailableObjects.Values.Select(t => ToXmlConverter.GetTo(t));
        }

        /// <inheritdoc/>
        public void DeserializeObjects(XElement xml)
        {
            var objects = xml.Elements().Select(x => FromXmlConverter.GetTo(x));
            foreach (T o in objects)
            {
                Add(o);
            }
        }
    }
}
