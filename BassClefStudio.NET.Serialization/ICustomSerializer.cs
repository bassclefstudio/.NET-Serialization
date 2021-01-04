using BassClefStudio.NET.Serialization.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Serialization
{
    /// <summary>
    /// Represents a service that can provide custom serialization for values in a <see cref="Graph"/>. Note that, since native <see cref="Graph"/> features don't support custom serialization, this is mainly for serializing value types.
    /// </summary>
    public interface ICustomSerializer
    {
        /// <summary>
        /// A <see cref="TypeGroup"/> describing the <see cref="Type"/>s that this <see cref="ICustomSerializer"/> handles.
        /// </summary>
        TypeGroup ApplicableTypes { get; }

        /// <summary>
        /// Serializes an <see cref="object"/> (of a type described in <see cref="ApplicableTypes"/>) to a <see cref="string"/> value.
        /// </summary>
        /// <param name="o">The given <see cref="object"/> to serialize.</param>
        /// <returns>A <see cref="string"/> suitable for serialization in <see cref="CustomValueNode.ValueString"/>.</returns>
        string Serialize(object o);

        /// <summary>
        /// Deserializes a <see cref="string"/> representation (for one of the specified types in <see cref="ApplicableTypes"/>) into the <see cref="object"/> it represents.
        /// </summary>
        /// <param name="value">A previously-serialized <see cref="string"/> value.</param>
        /// <returns>A <see cref="string"/> suitable for serialization in <see cref="CustomValueNode.ValueString"/>.</returns>
        object Deserialize(string value);
    }

    /// <summary>
    /// A base implementation of <see cref="ICustomSerializer"/> that provides basic <see cref="string"/> serialization of properties for a single type.
    /// </summary>
    /// <typeparam name="T">The type of object this <see cref="CustomSerializer{T}"/> serializes.</typeparam>
    public abstract class CustomSerializer<T> : ICustomSerializer
    {
        /// <inheritdoc/>
        public TypeGroup ApplicableTypes { get; } = new TypeGroup(typeof(T));

        /// <summary>
        /// An array of <see cref="Func{T, TResult}"/> for serializing the individual properties of a recieved <typeparamref name="T"/> object.
        /// </summary>
        public abstract Func<T, object>[] GetProperties { get; }

        /// <summary>
        /// Deserialize a <typeparamref name="T"/> object from the retrieved array of <see cref="string"/> property values.
        /// </summary>
        /// <param name="values">The collection of <see cref="string"/> values previously created from <see cref="GetProperties"/>.</param>
        public abstract T DeserializeObject(string[] values);

        /// <summary>
        /// A <see cref="string"/> value to use to separate property values in the serialized <see cref="string"/>.
        /// </summary>
        public virtual string Delimiter { get; } = ";";

        /// <inheritdoc/>
        public object Deserialize(string value)
        {
            var vals = value.Split(new string[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            if(vals.Length != GetProperties.Length)
            {
                throw new GraphException($"Serialized object contains the incorrect number of properties. Recieved {vals.Length}, expected {GetProperties.Length}");
            }

            return DeserializeObject(vals);
        }

        /// <inheritdoc/>
        public string Serialize(object o)
        {
            if(o is T t)
            {
                return string.Join(Delimiter, GetProperties.Select(p => p(t)).ToArray());
            }
            else
            {
                throw new ArgumentException($"Serializer expected type {typeof(T).Name} - recieved {o?.GetType().Name}.");
            }
        }
    }

    /// <summary>
    /// A base implementation of <see cref="ICustomSerializer"/> that provides basic <see cref="object.ToString"/> and object parsing serialization for a single type.
    /// </summary>
    /// <typeparam name="T">The type of object this <see cref="StringSerializer{T}"/> serializes.</typeparam>
    public abstract class StringSerializer<T> : ICustomSerializer
    {
        /// <inheritdoc/>
        public TypeGroup ApplicableTypes { get; } = new TypeGroup(typeof(T));

        /// <summary>
        /// A function that, given a <see cref="string"/> representation, can parse a <typeparamref name="T"/> object value.
        /// </summary>
        public abstract T ParseValue(string value);

        /// <summary>
        /// A function that, given a <typeparamref name="T"/> object, produces a <see cref="string"/> representation.
        /// </summary>
        public virtual string GetString(T value)
        {
            return value.ToString();
        }

        /// <inheritdoc/>
        public object Deserialize(string value)
        {
            return ParseValue(value);
        }

        /// <inheritdoc/>
        public string Serialize(object o)
        {
            if (o is T t)
            {
                return GetString(t);
            }
            else
            {
                throw new ArgumentException($"Serializer expected type {typeof(T).Name} - recieved {o?.GetType().Name}.");
            }
        }
    }

    /// <summary>
    /// Contains extension methods for the <see cref="ICustomSerializer"/> interface.
    /// </summary>
    internal static class SerializerExtensions
    {
        /// <summary>
        /// Checks whether any of the <see cref="ICustomSerializer"/>s in a collection is setup to handle the given <see cref="Type"/>.
        /// </summary>
        /// <param name="serializers">The collection of available <see cref="ICustomSerializer"/>s to handle <see cref="object"/>s.</param>
        /// <param name="type">The desired <see cref="Type"/>.</param>
        /// <returns>A <see cref="bool"/> indicating whether a <see cref="ICustomSerializer"/> in the collection handles this <paramref name="type"/>.</returns>
        public static bool ContainsType(this IEnumerable<ICustomSerializer> serializers, Type type)
        {
            return serializers.Select(s => s.ApplicableTypes).Any(t => t.IsMember(type));
        }

        /// <summary>
        /// Gets the <see cref="ICustomSerializer"/> in a collection that is setup to handle the given <see cref="Type"/>.
        /// </summary>
        /// <param name="serializers">The collection of available <see cref="ICustomSerializer"/>s to handle <see cref="object"/>s.</param>
        /// <param name="type">The desired <see cref="Type"/>.</param>
        /// <returns>The <see cref="ICustomSerializer"/> in the collection that handles this <paramref name="type"/>.</returns>
        public static ICustomSerializer GetForType(this IEnumerable<ICustomSerializer> serializers, Type type)
        {
            return serializers.FirstOrDefault(s => s.ApplicableTypes.IsMember(type));
        }
    }
}
