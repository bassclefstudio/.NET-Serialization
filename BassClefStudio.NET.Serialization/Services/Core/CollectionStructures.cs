using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="IPropertyProvider"/> that enumerates <see cref="object"/>s in a collection as key-value pairs.
    /// </summary>
    public class CollectionPropertyProvider : IPropertyProvider
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.OfType<IEnumerable>();

        /// <inheritdoc/>
        public GraphPriority Priority { get; } = GraphPriority.Structure;

        /// <inheritdoc/>
        public IDictionary<string, object> GetProperties(object value)
        {
            var enumerable = (IEnumerable)value;
            var dictionary = new Dictionary<string, object>();
            int index = 0;
            foreach (var item in enumerable)
            {
                dictionary.Add(index.ToString(), item);
                index++;
            }
            dictionary.Add("count", index);
            return dictionary;
        }
    }

    /// <summary>
    /// An <see cref="IGraphConstructor"/> that creates a derived <see cref="IEnumerable"/> collection from a constructor that accepts a single <see cref="IEnumerable{T}"/>.
    /// </summary>
    public class CollectionConstructor : IGraphConstructor
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.OfType<IEnumerable>();

        /// <inheritdoc/>
        public GraphPriority Priority { get; } = GraphPriority.Structure;

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph)
        {
            return desiredType.IsGenericType
                && desiredType.GetGenericArguments().Length == 1
                && subGraph.ContainsKey<int>("count")
                && Enumerable.Range(0, subGraph["count"].As<int>())
                    .All(i => subGraph.ContainsKey(i.ToString()))
                && FindConstructor(desiredType, desiredType.GetGenericArguments().First()) != null;
        }

        /// <inheritdoc/>
        public object Construct(Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            var itemType = desiredType.GetGenericArguments().First();
            var constructor = FindConstructor(desiredType, itemType);
            var indexes = Enumerable.Range(0, subGraph["count"].As<int>());
            var array = Array.CreateInstance(
                itemType,
                subGraph["count"].As<int>());
            foreach (var index in indexes)
            {
                array.SetValue(subGraph[index.ToString()], index);
            }
            usedKeys = indexes.Select(i => i.ToString()).Concat(new string[] { "count" });
            return constructor.Invoke(new object[] { array });
        }

        private ConstructorInfo FindConstructor(Type desiredType, Type itemType)
        {
            var allConstructors = desiredType.GetConstructors();
            return allConstructors.FirstOrDefault(c =>
                c.GetParameters().Length == 1
                && (c.GetParameters()[0].ParameterType == typeof(IEnumerable<>).MakeGenericType(itemType)
                    || c.GetParameters()[0].ParameterType == itemType.MakeArrayType()));
        }
    }
}
