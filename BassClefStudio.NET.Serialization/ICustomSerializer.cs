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
