using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="IPropertyConsumer"/> that adds items to a list from a given property collection.
    /// </summary>
    public class ListPropertyConsumer : IPropertyConsumer
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.OfType<IList>();

        /// <inheritdoc/>
        public GraphPriority Priority { get; } = GraphPriority.Primitive;

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
            var indexes = Enumerable.Range(0, subGraph["count"].As<int>());
            var list = (IList)value;
            foreach (var index in indexes)
            {
                list.Add(subGraph[index.ToString()]);
            }
            usedKeys = indexes.Select(i => i.ToString()).Concat(new string[] { "count" });
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
        public GraphPriority Priority { get; } = GraphPriority.Primitive;

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
        public GraphPriority Priority { get; } = GraphPriority.Primitive;

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
            var indexes = Enumerable.Range(0, subGraph["count"].As<int>());
            var array = (Array)value;
            foreach (var index in indexes)
            {
                array.SetValue(subGraph[index.ToString()], index);
            }
            usedKeys = indexes.Select(i => i.ToString()).Concat(new string[] { "count" });
        }
    }
}
