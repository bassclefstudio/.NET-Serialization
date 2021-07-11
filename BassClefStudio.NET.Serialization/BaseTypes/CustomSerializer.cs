using BassClefStudio.NET.Serialization.Services;
using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Serialization.BaseTypes
{
    /// <summary>
    /// An abstract, strongly-typed implementation of <see cref="ICustomSerializer"/>.
    /// </summary>
    /// <typeparam name="T">The type of values this <see cref="ICustomSerializer"/> serializes.</typeparam>
    public abstract class CustomSerializer<T> : ICustomSerializer
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; }
        
        /// <inheritdoc/>
        public abstract GraphPriority Priority { get; }

        /// <summary>
        /// Creates a new <see cref="CustomSerializer{T}"/>.
        /// </summary>
        public CustomSerializer()
        {
            SupportedTypes = TypeMatch.Type<T>();
        }

        /// <inheritdoc/>
        public IDictionary<string, object> GetProperties(object value)
        {
            if(value is T t)
            {
                return GetPropertiesInternal(t);
            }
            else
            {
                throw new ArgumentException($"Value expected of type {typeof(T).Name}; recieved type {value?.GetType().Name}.");
            }
        }

        /// <summary>
        /// Creates or retrieves an <see cref="IDictionary{TKey, TValue}"/> of <see cref="object"/> values that should be serialized as the given <see cref="string"/> keys.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> value to collect dependencies for.</param>
        /// <returns>The <see cref="IDictionary{TKey, TValue}"/> serialization information for this object.</returns>
        protected abstract IDictionary<string, object> GetPropertiesInternal(T value);

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph)
        {
            if (desiredType == typeof(T))
            {   
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public object Construct(Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            var built = ConstructInternal(subGraph);
            usedKeys = subGraph.Keys.ToArray();
            return built;
        }

        /// <summary>
        /// Attempts to construct/initialize a <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the currently defined <see cref="object"/> values under their <see cref="string"/> keys.</param>
        /// <returns>The newly-constructed <typeparamref name="T"/>.</returns>
        protected abstract T ConstructInternal(IDictionary<string, object> subGraph);
    }

    /// <summary>
    /// An abstract, strongly-typed implementation of <see cref="ICustomSerializer"/> optimized for ToString()/Parse() serialization of value types.
    /// </summary>
    /// <typeparam name="T">The type of values this <see cref="ICustomSerializer"/> serializes.</typeparam>
    public abstract class StringSerializer<T> : ICustomSerializer
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; }

        /// <inheritdoc/>
        public abstract GraphPriority Priority { get; }

        /// <summary>
        /// Creates a new <see cref="CustomSerializer{T}"/>.
        /// </summary>
        public StringSerializer()
        {
            SupportedTypes = TypeMatch.Type<T>();
        }

        /// <inheritdoc/>
        public IDictionary<string, object> GetProperties(object value)
        {
            if (value is T t)
            {
                return new Dictionary<string, object>()
                {
                    { "Value", GetValue(t) }
                };
            }
            else
            {
                throw new ArgumentException($"Value expected of type {typeof(T).Name}; recieved type {value?.GetType().Name}.");
            }
        }

        /// <summary>
        /// Gets the <see cref="string"/> serialized representation of the given value.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> object to serialize.</param>
        protected virtual string GetValue(T value) => value.ToString();

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph)
        {
            if (desiredType == typeof(T))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public object Construct(Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            var built = Parse(subGraph["Value"].As<string>());
            usedKeys = new string[] { "Value" };
            return built;
        }

        /// <summary>
        /// Attempts to construct/initialize a <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="value">The <see cref="string"/> parsed representation of the object (from <see cref="GetValue(T)"/>).</param>
        /// <returns>The newly-constructed <typeparamref name="T"/>.</returns>
        protected abstract T Parse(string value);
    }
}
