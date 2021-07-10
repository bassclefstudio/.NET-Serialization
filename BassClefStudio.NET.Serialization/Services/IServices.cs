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
    public interface IPropertyConsumer : IGraphService
    {
        /// <summary>
        /// Populates an object's properties and other values from the given <see cref="IDictionary{TKey, TValue}"/> provided by the serializer.
        /// </summary>
        /// <param name="value">The value to resolve dependencies for.</param>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the defined <see cref="object"/> values under their <see cref="string"/> keys.</param>
        /// <returns>A deserialized object.</returns>
        void PopulateObject(object value, IDictionary<string, object> subGraph);
    }

    /// <summary>
    /// Represents a service that can construct <see cref="object"/>s for the <see cref="GraphSerializer"/> serializer.
    /// </summary>
    public interface IGraphConstructor : IGraphService
    {
        /// <summary>
        /// Attempts to construct/initialize a <paramref name="desiredType"/> <see cref="object"/>.
        /// </summary>
        /// <param name="desiredType">The <see cref="Type"/> of the object to construct.</param>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the currently defined <see cref="object"/> values under their <see cref="string"/> keys. This may be less complete due to dependencies than the graph provided to <see cref="IPropertyConsumer.PopulateObject(object, IDictionary{string, object})"/> or <see cref="ISerializable.PopulateObject(IDictionary{string, object})"/>.</param>
        /// <param name="built">If successful, the newly-constructed <see cref="object"/>.</param>
        /// <param name="usedKeys">An optionally output collection of <see cref="string"/> keys from the <paramref name="subGraph"/> that were applied during construction.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        bool TryConstruct(Type desiredType, IDictionary<string, object> subGraph, out object built, out IEnumerable<string> usedKeys);
    }

    /// <summary>
    /// Represents a full custom serializer within the <see cref="GraphSerializer"/> system, in charge of property management and construction on a given type of <see cref="object"/>s.
    /// </summary>
    public interface ICustomSerializer : IPropertyProvider, IGraphConstructor
    { }
}
