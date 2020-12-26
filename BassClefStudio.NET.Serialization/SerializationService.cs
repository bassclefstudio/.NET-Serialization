using BassClefStudio.NET.Serialization.CustomTypes;
using BassClefStudio.NET.Serialization.Graphs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization
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

        private static ICustomSerializer[] DefaultCustomSerializers = new ICustomSerializer[]
        {
            new GuidSerializer(),
            new VectorSerializer()
        };

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        public SerializationService(params Assembly[] knownAssemblies)
        {
            Graph = new Graph(knownAssemblies);
            AddCustomSerializers(DefaultCustomSerializers);
        }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        /// <param name="defaultBehaviours">A set of <see cref="GraphBehaviour"/>s that will be applied to all trusted types in the <see cref="Graph"/>.</param>
        public SerializationService(GraphBehaviour defaultBehaviours, params Assembly[] knownAssemblies) : this(knownAssemblies)
        {
            Graph.Behaviours.Add(new GraphBehaviourInfo(Graph.TrustedTypes, defaultBehaviours));
        }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public SerializationService(params Type[] knownTypes)
        {
            Graph = new Graph(knownTypes);
            AddCustomSerializers(DefaultCustomSerializers);
        }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        /// <param name="defaultBehaviours">A set of <see cref="GraphBehaviour"/>s that will be applied to all trusted types in the <see cref="Graph"/>.</param>
        public SerializationService(GraphBehaviour defaultBehaviours, params Type[] knownTypes) : this(knownTypes)
        {
            Graph.Behaviours.Add(new GraphBehaviourInfo(Graph.TrustedTypes, defaultBehaviours));
        }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="defaultBehaviours">A set of <see cref="GraphBehaviour"/>s that will be applied to all trusted types in the <see cref="Graph"/>.</param>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public SerializationService(GraphBehaviour defaultBehaviours, IEnumerable<Assembly> knownAssemblies, IEnumerable<Type> knownTypes)
        {
            Graph = new Graph(knownAssemblies, knownTypes);
            AddCustomSerializers(DefaultCustomSerializers);
            Graph.Behaviours.Add(new GraphBehaviourInfo(Graph.TrustedTypes, defaultBehaviours));
        }

        /// <summary>
        /// Adds a given specific <see cref="GraphBehaviourInfo"/> to the underlying <see cref="Graph"/>.
        /// </summary>
        /// <param name="behaviourInfo">The <see cref="GraphBehaviourInfo"/> behaviour.</param>
        public void AddBehaviour(GraphBehaviourInfo behaviourInfo)
        {
            Graph.Behaviours.Add(behaviourInfo);
        }

        /// <summary>
        /// Adds a custom <see cref="ICustomSerializer"/> serializer to the <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="customSerializer">The <see cref="ICustomSerializer"/>s to add to the <see cref="SerializationService"/>'s <see cref="Graph"/>.</param>
        public void AddCustomSerializer(ICustomSerializer customSerializer)
        {
            Graph.CustomSerializers.Add(customSerializer);
        }

        /// <summary>
        /// Adds a collection of custom <see cref="ICustomSerializer"/> serializers to the <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="customSerializers">The collection of <see cref="ICustomSerializer"/>s to add to the <see cref="SerializationService"/>'s <see cref="Graph"/>.</param>
        public void AddCustomSerializers(IEnumerable<ICustomSerializer> customSerializers)
        {
            Graph.CustomSerializers.AddRange(customSerializers);
        }

        /// <summary>
        /// Builds a <see cref="Graphs.Graph"/> for the desired <see cref="object"/> (and its property graph) and serializes it to a JSON string.
        /// </summary>
        /// <param name="objectGraph">The parent <see cref="object"/> to serialize.</param>
        /// <param name="formatting">A <see cref="Formatting"/> enum indicating how the output should be styled.</param>
        public string Serialize(object objectGraph, Formatting formatting = Formatting.None)
        {
            Graph.Nodes.Clear();
            Graph.BuildNodes(objectGraph);
            return JsonConvert.SerializeObject(Graph.Nodes, formatting);
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
