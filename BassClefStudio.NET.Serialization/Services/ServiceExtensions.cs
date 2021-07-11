using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services
{
    /// <summary>
    /// Provides extension methods for the <see cref="IPropertyProvider"/>, <see cref="IPropertyConsumer"/>, <see cref="IGraphConstructor"/>, and <see cref="ISerializable"/> interfaces.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Gets the <see cref="IDictionary{TKey, TValue}"/> of all keyed dependencies of the given <see cref="object"/>.
        /// </summary>
        /// <param name="providers">An ordered collection of available <see cref="IPropertyProvider"/>s.</param>
        /// <param name="item">The <see cref="object"/> to serialize.</param>
        /// <returns>The <see cref="IDictionary{TKey, TValue}"/> serialization information for this object.</returns>
        public static IDictionary<string, object> GetDependencies(this IEnumerable<IPropertyProvider> providers, object item)
        {
            if (item is ISerializable serializable)
            {
                return serializable.GetProperties();
            }
            else
            {
                Type itemType = item?.GetType();
                var group = providers.GroupBy(s => s.Priority).OrderByDescending(g => g.Key)
                    .FirstOrDefault(g => g.Any(h => h.SupportedTypes.Match(itemType)));
                if (group != null)
                {
                    return group.Where(h => h.SupportedTypes.Match(itemType))
                        .SelectMany(h => h.GetProperties(item))
                        .ToDictionary(p => p.Key, p => p.Value);
                }
                else
                {
                    throw new SerializationException($"The given object of type {item?.GetType().Name} had no available serialization helpers.");
                }
            }
        }

        /// <summary>
        /// Populates an object's properties and other values from the given <see cref="IDictionary{TKey, TValue}"/> provided by the serializer.
        /// </summary>
        /// <param name="consumers">An ordered collection of available <see cref="IPropertyConsumer"/>s.</param>
        /// <param name="item">The <see cref="object"/> to deserialize.</param>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the currently defined <see cref="object"/> values under their <see cref="string"/> keys.</param>
        public static void PopulateObject(this IEnumerable<IPropertyConsumer> consumers, object item, IDictionary<string, object> subGraph)
        {
            if (item is ISerializable serializable)
            {
                serializable.PopulateObject(subGraph);
            }
            else
            {
                Type itemType = item.GetType();
                var currentValues = new Dictionary<string, object>(subGraph);
                foreach (var consumer in consumers.Where(c => c.SupportedTypes.Match(itemType)).OrderByDescending(s => s.Priority))
                {
                    if (consumer.CanHandle(itemType, subGraph))
                    {
                        consumer.PopulateObject(item, currentValues, out var keys);
                        if(keys != null)
                        {
                            foreach (var k in keys)
                            {
                                currentValues.Remove(k);
                            }

                            if(!currentValues.Any())
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Constructs a new <paramref name="desiredType"/> <see cref="object"/> using the given <see cref="IGraphConstructor"/>s.
        /// </summary>
        /// <param name="constructors">An ordered collection of available <see cref="IGraphConstructor"/>s.</param>
        /// <param name="desiredType">The <see cref="Type"/> of the object to construct.</param>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the currently defined <see cref="object"/> values under their <see cref="string"/> keys.</param>
        /// <param name="usedKeys">An optionally output collection of <see cref="string"/> keys from the <paramref name="subGraph"/> that were applied during construction.</param>
        /// <returns>The newly-constructed <see cref="object"/>.</returns>
        public static object Construct(this IEnumerable<IGraphConstructor> constructors, Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            foreach (var constructor in constructors.Where(c => c.SupportedTypes.Match(desiredType)).OrderByDescending(s => s.Priority))
            {
                if (constructor.CanHandle(desiredType, subGraph))
                {
                    var built = constructor.Construct(desiredType, subGraph, out usedKeys);
                    return built;
                }
            }

            throw new SerializationException($"No available IGraphConstructor could construct an object of type {desiredType.Name}");
        }

        /// <summary>
        /// Attempts to cast or convert the given <see cref="object"/> to a <typeparamref name="T"/> instance.
        /// </summary>
        /// <typeparam name="T">A (usually concrete) type to cast the <see cref="object"/> as.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <typeparamref name="T"/> value.</returns>
        public static T As<T>(this object value)
        {
            if (value is T t)
            {
                return t;
            }
            else
            {
                var convert = Convert.ChangeType(value, typeof(T));
                return (T)convert;
            }
        }

        /// <summary>
        /// Checks if a given key is present in an <see cref="IDictionary{TKey, TValue}"/> with a value of the specified type. 
        /// </summary>
        /// <typeparam name="T">A (usually concrete) type to cast the <see cref="object"/> as.</typeparam>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> to query.</param>
        /// <param name="key">The <see cref="string"/> key to check for.</param>
        /// <returns>A <see cref="bool"/> indicating whether a <typeparamref name="T"/> value is present at the given <paramref name="key"/>.</returns>
        public static bool ContainsKey<T>(this IDictionary<string, object> dictionary, string key)
        {
            if(dictionary.ContainsKey(key))
            {
                return dictionary[key] is T;
            }
            else
            {
                return false;
            }
        }
    }
}
