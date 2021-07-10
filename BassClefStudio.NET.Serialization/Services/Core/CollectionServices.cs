using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="IPropertyProvider"/> that enumerates <see cref="object"/>s in a collection as key-value pairs.
    /// </summary>
    public class CollectionPropertyProvider : IPropertyProvider
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.OfType<IEnumerable<object>>();

        /// <inheritdoc/>
        public IDictionary<string, object> GetProperties(object value)
        {
            var enumerable = (IEnumerable<object>)value;
            var dictionary = new Dictionary<string, object>();
            int index = 0;
            foreach (var item in enumerable)
            {
                dictionary.Add(index.ToString(), item);
                index++;
            }
            dictionary.Add("count", enumerable.Count());
            return dictionary;
        }
    }

    /// <summary>
    /// An <see cref="IPropertyConsumer"/> that adds items to a list from a given property collection.
    /// </summary>
    public class ListPropertyConsumer : IPropertyConsumer
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.OfType<IList>();

        /// <inheritdoc/>
        public void PopulateObject(object value, IDictionary<string, object> subGraph)
        {
            var list = (IList)value;
            if (subGraph.ContainsKey("count"))
            {
                foreach (var index in Enumerable.Range(0, subGraph["count"].As<int>()))
                {
                    list.Add(subGraph[index.ToString()]);
                }
            }
            else
            {
                throw new SerializationException($"Expected a \"count\" property on all collection graphs. {{{subGraph}}}");
            }
        }
    }
}
