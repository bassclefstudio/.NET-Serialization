using BassClefStudio.Serialization.Graphs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.Serialization
{
    /// <summary>
    /// A wrapper around <see cref="Graphs.Graph"/> that provides methods for serializing and deserializing <see cref="Graphs.Graph"/> data.
    /// </summary>
    public class SerializationService
    {
        /// <summary>
        /// The <see cref="Graphs.Graph"/> that is built to handle the object references when serializing or deserializing objects.
        /// </summary>
        public Graph Graph { get; }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        public SerializationService(params Assembly[] knownAssemblies)
        {
            Graph = new Graph(knownAssemblies);
        }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public SerializationService(params Type[] knownTypes)
        {
            Graph = new Graph(knownTypes);
        }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public SerializationService(Assembly[] knownAssemblies, Type[] knownTypes)
        {
            Graph = new Graph(knownAssemblies, knownTypes);
        }

        /// <summary>
        /// Builds a <see cref="Graphs.Graph"/> for the desired <see cref="object"/> (and its property graph) and serializes it to a JSON string.
        /// </summary>
        /// <param name="objectGraph">The parent <see cref="object"/> to serialize.</param>
        public string Serialize(object objectGraph)
        {
            Graph.Nodes.Clear();
            Graph.BuildNodes(objectGraph);
            return JsonConvert.SerializeObject(Graph.Nodes);
        }

        /// <summary>
        /// Deserializes a collection of <see cref="Node"/>s from JSON and builds the resulting object model.
        /// </summary>
        /// <param name="json">The <see cref="string"/> JSON content.</param>
        /// <returns>The parent <see cref="object"/>, as type <typeparamref name="T"/>.</returns>
        public T Deserialize<T>(string json)
        {
            var nodes = JsonConvert.DeserializeObject<Node[]>(json);
            Graph.Nodes.Clear();
            Graph.Nodes.AddRange(nodes);
            var o = Graph.BuildObject();
            return (T)o;
        }
    }
}
