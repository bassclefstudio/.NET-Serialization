using BassClefStudio.NET.Core;
using BassClefStudio.NET.Core.Primitives;
using BassClefStudio.NET.Core.Structures;
using BassClefStudio.NET.Serialization.BaseTypes;
using BassClefStudio.NET.Serialization.Model;
using BassClefStudio.NET.Serialization.Services;
using BassClefStudio.NET.Serialization.Services.Core;
using BassClefStudio.NET.Serialization.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BassClefStudio.NET.Serialization
{
    /// <summary>
    /// A <see cref="Graph{TNode, TConnection}"/>-based serializer that implements circular, polymorphic serialization with no attribute or constructor requirements.
    /// </summary>
    public class GraphSerializer : ISerializationService
    {
        #region Properties

        /// <inheritdoc/>
        public List<IGraphService> Services { get; }

        /// <summary>
        /// The collection of <see cref="IGraphConstructor"/> <see cref="Services"/>.
        /// </summary>
        public IEnumerable<IGraphConstructor> Constructors => Services.OfType<IGraphConstructor>();

        /// <summary>
        /// The collection of <see cref="IPropertyProvider"/> <see cref="Services"/>.
        /// </summary>
        public IEnumerable<IPropertyProvider> Providers => Services.OfType<IPropertyProvider>();

        /// <summary>
        /// The collection of <see cref="IPropertyConsumer"/> <see cref="Services"/>.
        /// </summary>
        public IEnumerable<IPropertyConsumer> Consumers => Services.OfType<IPropertyConsumer>();

        /// <inheritdoc/>
        public IGraphWriter GraphWriter { get; set; }

        /// <inheritdoc/>
        public ITypeMatch NativeType { get; set; }

        /// <inheritdoc/>
        public ITypeMatch TrustedTypes { get; set; }

        /// <summary>
        /// Gets an <see cref="ITypeMatch"/> for both <see cref="NativeType"/> and <see cref="TrustedTypes"/>.
        /// </summary>
        public ITypeMatch AllTrusted => TrustedTypes.Concat(NativeType);

        /// <summary>
        /// The <see cref="Graph{TNode, TConnection}"/> structure containing the information about currently serializing objects.
        /// </summary>
        public Graph<SerializeNode, SerializeDependency> CurrentGraph { get; private set; }

        #endregion
        #region Initialize

        /// <summary>
        /// Creates a new <see cref="GraphSerializer"/> with default settings and the given <see cref="ITypeMatch"/> for trusted types.
        /// </summary>
        /// <param name="trustedTypes">An <see cref="ITypeMatch"/> expression that matches against all trusted <see cref="Type"/>s.</param>
        public GraphSerializer(ITypeMatch trustedTypes)
        {
            Services = new List<IGraphService>()
            {
                new GraphNoConstructor(),
                new GraphDefaultConstructor(),
                new ReflectionGraphProperty(),
                new CollectionPropertyProvider(),
                new ListPropertyConsumer(),
                new ArrayPropertyConsumer(),
                new ArrayConstructor(),
                new DateTimeSerializer(),
                new DateTimeOffsetSerializer(),
                new DateTimeZoneSerializer(),
                new DateTimeSpanSerializer(),
                new ColorSerializer(),
                new GuidSerializer(),
                new Vector2Serializer()
            };
            TrustedTypes = trustedTypes.Concat(
                TypeMatch.Type(
                    typeof(List<>),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(DateTimeZone),
                    typeof(DateTimeSpan),
                    typeof(Color),
                    typeof(Guid),
                    typeof(Vector2)));
            GraphWriter = new JsonGraphWriter();
            NativeType = TypeMatch.Type(
                typeof(string),
                typeof(byte),
                typeof(uint),
                typeof(int),
                typeof(ulong),
                typeof(long),
                typeof(float),
                typeof(double),
                typeof(bool));
        }

        #endregion
        #region Interface

        /// <inheritdoc/>
        public string Serialize<T>(T value)
        {
            BuildGraph(value);
            return GraphWriter.WriteGraph(CurrentGraph);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string text)
        {
            CurrentGraph = GraphWriter.ReadGraph(text);
            //// The root object will always have ID of "0".
            var o = ReadGraph(CurrentGraph.Nodes.Get(0.ToString()));
            return (T)o;
        }

        #endregion
        #region BuildGraph

        private int currentId = 0;

        /// <summary>
        /// Creates and builds a new <see cref="CurrentGraph"/> for the given <see cref="object"/>.
        /// </summary>
        /// <param name="root">The root object in the graph, which is currently being serialized.</param>
        /// <returns>The <see cref="SerializeNode"/> of the <paramref name="root"/> object.</returns>
        private SerializeNode BuildGraph(object root)
        {
            CurrentGraph = new Graph<SerializeNode, SerializeDependency>();
            currentId = 0;
            return GetNode(root);
        }

        /// <summary>
        /// Adds an <see cref="object"/> and its dependencies to the <see cref="CurrentGraph"/>.
        /// </summary>
        /// <param name="value">The <see cref="object"/> to add.</param>
        /// <returns>The created <see cref="SerializeNode"/> representing <paramref name="value"/>.</returns>
        private SerializeNode GetNode(object value)
        {
            if(value != null && !AllTrusted.Match(value?.GetType()))
            {
                throw new TypeMatchException(AllTrusted, value?.GetType());
            }

            var existing = CurrentGraph.Nodes.FirstOrDefault(n => object.ReferenceEquals(n.Value, value));
            if (existing != null)
            {
                //// Existing nodes should be reused.
                return existing;
            }
            else
            {
                var node = new SerializeNode(currentId.ToString(), value);
                currentId++;

                //// Adds the newly-created node to the graph.
                CurrentGraph.AddNode(node);

                //// Check if dependencies need to be resolved or if the value is native/null.
                if (value != null && !NativeType.Match(value.GetType()))
                {
                    //// Get all of the dependencies as connections (unique for each individual node, so can construct them here).
                    var dependencies = Providers.GetDependencies(value)
                        .Select(p => new SerializeDependency(p.Key, node, GetNode(p.Value)));

                    foreach (var dep in dependencies)
                    {
                        //// Add new dependencies to the graph.
                        CurrentGraph.AddConnection(dep);
                    }
                }
                else
                {
                    node.IsNative = true;
                }

                return node;
            }
        }

        #endregion
        #region ReadGraph

        private Dictionary<SerializeNode, List<SerializeDependency>> dependencyMap;
        private HashSet<SerializeNode> constructed;
        private HashSet<SerializeNode> populating;

        /// <summary>
        /// Constructs and populates new <see cref="object"/>s in <see cref="CurrentGraph"/> to match its structure and dependencies.
        /// </summary>
        /// <param name="root">The root object in the graph, which is currently being deserialized.</param>
        /// <returns>The final <see cref="object"/> value stored in the <paramref name="root"/> node.</returns>
        private object ReadGraph(SerializeNode root)
        {
            dependencyMap = new Dictionary<SerializeNode, List<SerializeDependency>>();
            constructed = new HashSet<SerializeNode>();
            populating = new HashSet<SerializeNode>();
            Construct(root);
            Populate(root);
            return root.Value;
        }

        /// <summary>
        /// Constructs new instances of the object tree starting at the given <see cref="SerializeNode"/>, passing constructor parameters greedily where possible.
        /// </summary>
        /// <param name="startNode">The <see cref="SerializeNode"/> to start construction at.</param>
        private void Construct(SerializeNode startNode)
        {
            if (!constructed.Contains(startNode))
            {
                if (startNode.IsNative)
                {
                    //// Test only against native trusted types.
                    if (startNode.DesiredType != null
                        && !NativeType.Match(startNode.DesiredType))
                    {
                        throw new TypeMatchException(NativeType, startNode.DesiredType?.GetType());
                    }

                    if(startNode.Value is JToken token)
                    {
                        if (token.Type == JTokenType.Null)
                        {
                            startNode.Value = null;
                        }
                        else
                        {
                            startNode.Value = token.ToObject(startNode.DesiredType);
                        }
                        constructed.Add(startNode);
                    }
                    else
                    {
                        throw new SerializationException("Native object not encapsulated in expected JToken value.");
                    }
                }
                else
                {
                    //// Test only against non-native trusted types.
                    if (!TrustedTypes.Match(startNode.DesiredType))
                    {
                        throw new TypeMatchException(TrustedTypes, startNode.DesiredType?.GetType());
                    }

                    //// Get all dependencies.
                    var dependencies = CurrentGraph.GetConnections(startNode);
                    dependencyMap.Add(startNode, new List<SerializeDependency>(dependencies));

                    //// Construct an object with non-circular dependencies.
                    object newValue = Constructors.Construct(
                        startNode.DesiredType,
                        dependencies
                            .Where(d => !IsCircular(d))
                            .ToDictionary(
                                d => d.DependencyId,
                                d =>
                                {
                                    Construct(d.EndNode);
                                    Populate(d.EndNode);
                                    return d.EndNode.Value;
                                }),
                        out var usedKeys);
                    dependencyMap[startNode].RemoveRange(
                        dependencyMap[startNode].ToArray()
                        .Where(d => usedKeys.Contains(d.DependencyId)));
                    startNode.Value = newValue;
                    constructed.Add(startNode);

                    foreach (var dep in dependencyMap[startNode])
                    {
                        Construct(dep.EndNode);
                    }
                }
                
            }
        }

        /// <summary>
        /// Populates already <see cref="Construct(SerializeNode)"/>-ed <see cref="SerializeNode"/>s using the available <see cref="Helpers"/>, from the bottom of the graph upwards.
        /// </summary>
        /// <param name="startNode"></param>
        private void Populate(SerializeNode startNode)
        {
            if (!populating.Contains(startNode))
            {
                populating.Add(startNode);
                if (dependencyMap.ContainsKey(startNode)
                && dependencyMap[startNode].Any())
                {
                    foreach (var dep in dependencyMap[startNode])
                    {
                        Populate(dep.EndNode);
                    }

                    //// Populate all remaining dependencies.
                    Consumers.PopulateObject(
                        startNode.Value,
                        dependencyMap[startNode].ToDictionary(d => d.DependencyId, d => d.EndNode.Value));

                    dependencyMap[startNode].Clear();
                }
            }
        }

        /// <summary>
        /// Evaluates whether this <see cref="SerializeDependency"/> has child properties in <see cref="CurrentGraph"/> that are dependent on the <see cref="SerializeDependency.StartNode"/> parent.
        /// </summary>
        /// <param name="dependency">The <see cref="SerializeDependency"/> dependency to query.</param>
        /// <returns>The <see cref="bool"/> result of the query.</returns>
        private bool IsCircular(SerializeDependency dependency)
        {
            var attemptPath = CurrentGraph.FindPath(dependency.EndNode, dependency.StartNode);
            return attemptPath != null;
        }

        #endregion
    }
}
