using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="IGraphConstructor"/> that creates uninitialized <see cref="object"/>s for any type without an available constructor.
    /// </summary>
    public class GraphNoConstructor : IGraphConstructor
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.All();

        /// <inheritdoc/>
        public GraphPriority Priority { get; } = GraphPriority.Base;

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph) => true;

        /// <inheritdoc/>
        public object Construct(Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            usedKeys = Array.Empty<string>();
            return FormatterServices.GetUninitializedObject(desiredType);
        }
    }

    /// <summary>
    /// An <see cref="IGraphConstructor"/> that constructs <see cref="object"/>s for any type with a default constructor.
    /// </summary>
    public class GraphDefaultConstructor : IGraphConstructor
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; } = TypeMatch.All();

        /// <inheritdoc/>
        public GraphPriority Priority { get; } = GraphPriority.BaseReflection;

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph)
        {
            //// Attempt to get parameterless constructor.
            var constructor = desiredType.GetConstructor(Type.EmptyTypes);
            return constructor != null;
        }

        /// <inheritdoc/>
        public object Construct(Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            usedKeys = Array.Empty<string>();
            var constructor = desiredType.GetConstructor(Type.EmptyTypes);
            return constructor.Invoke(Array.Empty<object>());
        }
    }
}
