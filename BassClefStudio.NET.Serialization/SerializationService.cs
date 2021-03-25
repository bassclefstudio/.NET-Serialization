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
    public class SerializationService : ISerializationService
    {
        /// <inheritdoc/>
        public Graph Graph { get; }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        public SerializationService(params Assembly[] knownAssemblies)
        {
            Graph = new Graph(knownAssemblies);
            AddCustomSerializers(NativeTypes.DefaultCustomSerializers);
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
            AddCustomSerializers(NativeTypes.DefaultCustomSerializers);
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
            AddCustomSerializers(NativeTypes.DefaultCustomSerializers);
            Graph.Behaviours.Add(new GraphBehaviourInfo(Graph.TrustedTypes, defaultBehaviours));
        }

        /// <summary>
        /// Creates a new <see cref="SerializationService"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public SerializationService(IEnumerable<Assembly> knownAssemblies, IEnumerable<Type> knownTypes)
        {
            Graph = new Graph(knownAssemblies, knownTypes);
            AddCustomSerializers(NativeTypes.DefaultCustomSerializers);
            Graph.Behaviours.Add(new GraphBehaviourInfo(Graph.TrustedTypes, GraphBehaviour.None));
        }

        /// <inheritdoc/>
        public void AddBehaviour(GraphBehaviourInfo behaviourInfo)
        {
            Graph.Behaviours.Add(behaviourInfo);
        }
        
        /// <inheritdoc/>
        public void AddCustomSerializer(ICustomSerializer customSerializer)
        {
            Graph.CustomSerializers.Add(customSerializer);
        }

        /// <inheritdoc/>
        public void AddCustomSerializers(IEnumerable<ICustomSerializer> customSerializers)
        {
            Graph.CustomSerializers.AddRange(customSerializers);
        }

        /// <inheritdoc/>
        public string Serialize(object objectGraph, Formatting formatting = Formatting.None)
        {
            if (objectGraph == null)
            {
                return string.Empty;
            }
            else
            {
                Graph.Nodes.Clear();
                Graph.BuildNodes(objectGraph);
                return JsonConvert.SerializeObject(Graph.Nodes, formatting);
            }
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            else
            {
                var nodes = JsonConvert.DeserializeObject<Node[]>(json);
                Graph.Nodes.Clear();
                Graph.Nodes.AddRange(nodes);
                var o = Graph.BuildObject();
                return (T)o;
            }
        }

        /// <inheritdoc/>
        public bool IsSerializable(Type type)
        {
            return Graph.TrustedTypes.IsMember(type);
        }
    }
}
