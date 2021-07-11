using BassClefStudio.NET.Core.Structures;
using BassClefStudio.NET.Serialization.Model;
using BassClefStudio.NET.Serialization.Services;
using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization
{
    /// <summary>
    /// Represents a service that uses <see cref="Graph{TNode, TConnection}"/>-based data structures to (de)serialize polymophic and connected data, and <see cref="IGraphService"/>s to extend its functionality.
    /// </summary>
    public interface ISerializationService
    {
        /// <summary>
        /// A collection of <see cref="IGraphService"/>s that provide object property management, property injection, and construction for the <see cref="ISerializationService"/>, in order of priority.
        /// </summary>
        IEnumerable<IGraphService> Services { get; }

        /// <summary>
        /// The <see cref="IGraphWriter"/> for serializing generated <see cref="Graph{TNode, TConnection}"/>s.
        /// </summary>
        IGraphWriter GraphWriter { get; }
        
        /// <summary>
        /// A collection of <see cref="ITypeConfiguration"/>s describing the types of objects this <see cref="ISerializationService"/> supports, and how to serialize them (native vs. managed).
        /// </summary>
        IEnumerable<ITypeConfiguration> TypeConfiguration { get; }

        /// <summary>
        /// Serializes a given <typeparamref name="T"/> object to text.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <returns>Some <see cref="string"/> serialized text representing the <paramref name="value"/>.</returns>
        string Serialize<T>(T value);

        /// <summary>
        /// Deserializes some text to a <typeparamref name="T"/> object.
        /// </summary>
        /// <typeparam name="T">The type of object the serialized <paramref name="text"/> represents.</typeparam>
        /// <param name="text">The serialized text.</param>
        /// <returns>A <typeparamref name="T"/> object.</returns>
        T Deserialize<T>(string text);
    }
}
