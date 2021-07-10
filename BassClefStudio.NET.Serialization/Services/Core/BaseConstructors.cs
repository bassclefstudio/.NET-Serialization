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
        public ITypeMatch SupportedTypes { get; }

        /// <summary>
        /// Creates a new <see cref="GraphNoConstructor"/>.
        /// </summary>
        public GraphNoConstructor()
        {
            SupportedTypes = new AllTypeMatch();
        }

        /// <inheritdoc/>
        public bool TryConstruct(Type desiredType, IDictionary<string, object> subGraph, out object built, out IEnumerable<string> usedKeys)
        {
            usedKeys = Array.Empty<string>();
            built = FormatterServices.GetUninitializedObject(desiredType);
            return true;
        }
    }

    /// <summary>
    /// An <see cref="IGraphConstructor"/> that constructs <see cref="object"/>s for any type with a default constructor.
    /// </summary>
    public class GraphDefaultConstructor : IGraphConstructor
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; }

        /// <summary>
        /// Creates a new <see cref="GraphDefaultConstructor"/>.
        /// </summary>
        public GraphDefaultConstructor()
        {
            SupportedTypes = new AllTypeMatch();
        }

        /// <inheritdoc/>
        public bool TryConstruct(Type desiredType, IDictionary<string, object> subGraph, out object built, out IEnumerable<string> usedKeys)
        {
            var constructor = desiredType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                built = null;
                usedKeys = null;
                return false;
            }
            else
            {
                usedKeys = Array.Empty<string>();
                built = constructor.Invoke(Array.Empty<object>());
                return true;
            }
        }
    }
}
