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
        public GraphPriority Priority { get; } = GraphPriority.Collection;

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
        public GraphPriority Priority { get; } = GraphPriority.Collection;

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph)
        {
            return !desiredType.IsArray 
                && subGraph.ContainsKey<int>("count")
                && Enumerable.Range(0, subGraph["count"].As<int>())
                    .All(i => subGraph.ContainsKey(i.ToString()));
        }

        /// <inheritdoc/>
        public void PopulateObject(object value, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            List<string> keys = new List<string>() { "count" };
            var list = (IList)value;
            foreach (var index in Enumerable.Range(0, subGraph["count"].As<int>()))
            {
                keys.Add(index.ToString());
                list.Add(subGraph[index.ToString()]);
            }
            usedKeys = keys.AsEnumerable();
        }
    }

    /// <summary>
    /// An <see cref="IGraphConstructor"/> for constructing correctly-sized <see cref="Array"/>s.
    /// </summary>
    public class ArrayConstructor : IGraphConstructor
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.OfType<Array>();

        /// <inheritdoc/>
        public GraphPriority Priority { get; } = GraphPriority.Collection;

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph)
        {
            return desiredType.IsArray
                && subGraph.ContainsKey("count");
        }

        /// <inheritdoc/>
        public object Construct(Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            usedKeys = Array.Empty<string>();
            return Array.CreateInstance(
                desiredType.GetElementType(),
                subGraph["count"].As<int>());
        }
    }

    /// <summary>
    /// An <see cref="IPropertyConsumer"/> that adds items to an array from a given property collection.
    /// </summary>
    public class ArrayPropertyConsumer : IPropertyConsumer
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.OfType<Array>();

        /// <inheritdoc/>
        public GraphPriority Priority { get; } = GraphPriority.Collection;

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph)
        {
            return desiredType.IsArray 
                && subGraph.ContainsKey<int>("count")
                && Enumerable.Range(0, subGraph["count"].As<int>())
                    .All(i => subGraph.ContainsKey(i.ToString()));
        }

        /// <inheritdoc/>
        public void PopulateObject(object value, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            List<string> keys = new List<string>() { "count" };
            var array = (Array)value;
            foreach (var index in Enumerable.Range(0, subGraph["count"].As<int>()))
            {
                keys.Add(index.ToString());
                array.SetValue(subGraph[index.ToString()], index);
            }
            usedKeys = keys.AsEnumerable();
        }
    }
}
