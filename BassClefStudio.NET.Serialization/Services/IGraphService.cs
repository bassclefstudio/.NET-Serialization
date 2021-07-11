using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services
{
    /// <summary>
    /// An interface for all plug-in services used by the <see cref="GraphSerializer"/>.
    /// </summary>
    public interface IGraphService
    {
        /// <summary>
        /// An <see cref="ITypeMatch"/> expression describing the types of objects this <see cref="IGraphService"/> supports.
        /// </summary>
        ITypeMatch SupportedTypes { get; }

        /// <summary>
        /// The <see cref="GraphPriority"/> execution priority of the given <see cref="IGraphService"/>.
        /// </summary>
        GraphPriority Priority { get; }
    }

    /// <summary>
    /// An <see cref="IGraphService"/> which is responsible for populating or constructing new objects.
    /// </summary>
    public interface IGraphHandler : IGraphService
    {
        /// <summary>
        /// Checks if a <see cref="Type"/> can be handled by this <see cref="IGraphHandler"/> with the given parameters.
        /// </summary>
        /// <param name="desiredType">The <see cref="Type"/> of the object to construct.</param>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the currently defined <see cref="object"/> values under their <see cref="string"/> keys.</param>
        /// <returns>A <see cref="bool"/> indicating whether this <see cref="IGraphHandler"/> can handle the given object.</returns>
        bool CanHandle(Type desiredType, IDictionary<string, object> subGraph);
    }

    /// <summary>
    /// An enum defining the priority order for a collection of <see cref="IGraphService"/>s when serializing or deserializing objects.
    /// </summary>
    public enum GraphPriority
    {
        /// <summary>
        /// This <see cref="IGraphService"/> serves all (or most) objects and should be run only if other services couldn't handle the request.
        /// </summary>
        Base = 0,
        /// <summary>
        /// A reflection-based <see cref="IGraphService"/> similar to <see cref="Reflection"/>, but applicable to most types regardless of parameter (like <see cref="Base"/>) and should be run only if parameter-based services were not available.
        /// </summary>
        BaseReflection = 1,
        /// <summary>
        /// Reflection-based <see cref="IGraphService"/>s, which run on remaining values after all custom services have been executed.
        /// </summary>
        Reflection = 2,
        /// <summary>
        /// Default or user-defined <see cref="IGraphService"/>s which are applied to broad categories of types (e.g. collections/<see cref="IEnumerable{T}"/>s).
        /// </summary>
        Structure = 3,
        /// <summary>
        /// Default or user-defined <see cref="IGraphService"/>s for primitive data types.
        /// </summary>
        Primitive = 4,
        /// <summary>
        /// Additional user-defined <see cref="IGraphService"/>s for dealing with specialized data.
        /// </summary>
        Custom = 5
    }

    /// <summary>
    /// Represents a service that provides serialization information for serialized <see cref="object"/>s.
    /// </summary>
    public interface IPropertyProvider : IGraphService
    {
        /// <summary>
        /// Creates or retrieves an <see cref="IDictionary{TKey, TValue}"/> of <see cref="object"/> values that should be serialized as the given <see cref="string"/> keys.
        /// </summary>
        /// <param name="value">The value to collect dependencies for.</param>
        /// <returns>The <see cref="IDictionary{TKey, TValue}"/> serialization information for this object.</returns>
        IDictionary<string, object> GetProperties(object value);
    }

    /// <summary>
    /// Represents a service that applies serialization information to deserialized <see cref="object"/>s.
    /// </summary>
    public interface IPropertyConsumer : IGraphHandler
    {
        /// <summary>
        /// Populates an object's properties and other values from the given <see cref="IDictionary{TKey, TValue}"/> provided by the serializer.
        /// </summary>
        /// <param name="value">The value to resolve dependencies for.</param>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the currently defined <see cref="object"/> values under their <see cref="string"/> keys.</param>
        /// <param name="usedKeys">An optionally output collection of <see cref="string"/> keys from the <paramref name="subGraph"/> that were applied during population.</param>
        /// <returns>A deserialized object.</returns>
        void PopulateObject(object value, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys);
    }

    /// <summary>
    /// Represents a service that can construct <see cref="object"/>s for the <see cref="GraphSerializer"/> serializer.
    /// </summary>
    public interface IGraphConstructor : IGraphHandler
    {
        /// <summary>
        /// Constructs and initializes a <paramref name="desiredType"/> <see cref="object"/>.
        /// </summary>
        /// <param name="desiredType">The <see cref="Type"/> of the object to construct.</param>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the currently defined <see cref="object"/> values under their <see cref="string"/> keys.</param>
        /// <param name="usedKeys">An optionally output collection of <see cref="string"/> keys from the <paramref name="subGraph"/> that were applied during construction.</param>
        /// <returns>The newly-constructed <see cref="object"/>.</returns>
        object Construct(Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys);
    }

    /// <summary>
    /// Represents a full custom serializer within the <see cref="GraphSerializer"/> system, in charge of property management and construction on a given type of <see cref="object"/>s.
    /// </summary>
    public interface ICustomSerializer : IPropertyProvider, IGraphConstructor
    { }
}
