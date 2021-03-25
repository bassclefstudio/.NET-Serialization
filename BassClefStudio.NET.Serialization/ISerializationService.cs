using BassClefStudio.NET.Serialization.Graphs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization
{
    /// <summary>
    /// An interface that abstracts the methods required to serialize and deserialize data from a <see cref="Graphs.Graph"/> or similar.
    /// </summary>
    public interface ISerializationService
    {
        /// <summary>
        /// The <see cref="Graphs.Graph"/> that is built to handle the object references when serializing or deserializing objects.
        /// </summary>
        Graph Graph { get; }

        /// <summary>
        /// Adds a given specific <see cref="GraphBehaviourInfo"/> to the underlying <see cref="Graph"/>.
        /// </summary>
        /// <param name="behaviourInfo">The <see cref="GraphBehaviourInfo"/> behaviour.</param>
        void AddBehaviour(GraphBehaviourInfo behaviourInfo);

        /// <summary>
        /// Adds a custom <see cref="ICustomSerializer"/> serializer to the <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="customSerializer">The <see cref="ICustomSerializer"/>s to add to the <see cref="SerializationService"/>'s <see cref="Graph"/>.</param>
        void AddCustomSerializer(ICustomSerializer customSerializer);

        /// <summary>
        /// Adds a collection of custom <see cref="ICustomSerializer"/> serializers to the <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="customSerializers">The collection of <see cref="ICustomSerializer"/>s to add to the <see cref="SerializationService"/>'s <see cref="Graph"/>.</param>
        void AddCustomSerializers(IEnumerable<ICustomSerializer> customSerializers);

        /// <summary>
        /// Builds a <see cref="Graphs.Graph"/> for the desired <see cref="object"/> (and its property graph) and serializes it to a JSON string.
        /// </summary>
        /// <param name="objectGraph">The parent <see cref="object"/> to serialize.</param>
        /// <param name="formatting">A <see cref="Formatting"/> enum indicating how the output should be styled.</param>
        string Serialize(object objectGraph, Formatting formatting = Formatting.None);

        /// <summary>
        /// Deserializes a collection of <see cref="Node"/>s from JSON and builds the resulting object model.
        /// </summary>
        /// <param name="json">The <see cref="string"/> JSON content.</param>
        /// <returns>The parent <see cref="object"/>, as type <typeparamref name="T"/>.</returns>
        T Deserialize<T>(string json);

        /// <summary>
        /// Checks whether this <see cref="ISerializationService"/> supports the serialization of the given type.
        /// </summary>
        /// <param name="type">The desired <see cref="Type"/> type of the object you wish to serialize.</param>
        /// <returns>A <see cref="bool"/> indicating whether this type can be serialized using this <see cref="ISerializationService"/>'s <see cref="Graph"/> and any other capabailities.</returns>
        bool IsSerializable(Type type);
    }

    /// <summary>
    /// Provides extension methods for the 
    /// </summary>
    public static class SerializationServiceExtensions
    {
        /// <summary>
        /// Checks whether this <see cref="ISerializationService"/> supports the serialization of the given type.
        /// </summary>
        /// <typeparam name="T">The desired type of the object you wish to serialize.</typeparam>
        /// <param name="service">The <see cref="ISerializationService"/> to query.</param>
        /// <returns>A <see cref="bool"/> indicating whether this type can be serialized using this <see cref="ISerializationService"/>'s <see cref="Graph"/> and any other capabailities.</returns>
        public static bool IsSerializable<T>(this ISerializationService service)
            => service.IsSerializable(typeof(T));
    }
}
