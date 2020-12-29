using BassClefStudio.NET.Core.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace BassClefStudio.NET.Serialization.Graphs
{
    /// <summary>
    /// Represents a graph of <see cref="Node"/>s, broken down to preserve references and including all public properties of provided objects.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// A collection of all of the <see cref="Node"/>s in this data structure.
        /// </summary>
        public List<Node> Nodes { get; }

        /// <summary>
        /// A <see cref="TypeGroup"/> of all trusted <see cref="Type"/>s for serialization.
        /// </summary>
        public TypeGroup TrustedTypes { get; }

        /// <summary>
        /// A collection of <see cref="GraphBehaviourInfo"/> instances indicating any special behaviours this <see cref="Graph"/> might need for certain types of objects.
        /// </summary>
        public List<GraphBehaviourInfo> Behaviours { get; }

        /// <summary>
        /// A collection of <see cref="ICustomSerializer"/>s that will be used to serialize/deserialize given value types.
        /// </summary>
        public List<ICustomSerializer> CustomSerializers { get; }

        /// <summary>
        /// Gets the static array of <see cref="Type"/>s that the <see cref="Graph"/> trusts by default. This includes basic types for collections such as <see cref="List{T}"/>.
        /// </summary>
        public static Type[] DefaultTrustedTypes { get; } = new Type[]
        {
            typeof(List<>),
            typeof(ObservableCollection<>),
            typeof(Array),
            typeof(Vector2),
            typeof(Guid),
            typeof(DateTimeOffset),
            typeof(Color),
            typeof(DateTimeSpan)
        };

        /// <summary>
        /// Gets the static array of <see cref="Type"/>s that the <see cref="Graph"/> trusts by default. This includes basic types for collections such as <see cref="List{T}"/>.
        /// </summary>
        public static Assembly[] DefaultTrustedAssemblies { get; } = new Assembly[]
        {
        };

        private int Index = 0;
        private Graph()
        {
            Nodes = new List<Node>();
            Behaviours = new List<GraphBehaviourInfo>();
            CustomSerializers = new List<ICustomSerializer>();

            TrustedTypes = new TypeGroup();
            TrustedTypes.KnownTypes.AddRange(DefaultTrustedTypes);
            TrustedTypes.KnownAssemblies.AddRange(DefaultTrustedAssemblies);
        }

        /// <summary>
        /// Creates a new empty <see cref="Graph"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        public Graph(params Assembly[] knownAssemblies) : this()
        {
            TrustedTypes.KnownAssemblies.AddRange(knownAssemblies);
        }

        /// <summary>
        /// Creates a new empty <see cref="Graph"/>.
        /// </summary>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public Graph(params Type[] knownTypes) : this()
        {
            TrustedTypes.KnownTypes.AddRange(knownTypes);
        }

        /// <summary>
        /// Creates a new empty <see cref="Graph"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public Graph(IEnumerable<Assembly> knownAssemblies, IEnumerable<Type> knownTypes) : this()
        {
            TrustedTypes.KnownAssemblies.AddRange(knownAssemblies);
            TrustedTypes.KnownTypes.AddRange(knownTypes);
        }

        #region BuildNode

        /// <summary>
        /// Builds the entire <see cref="Graph"/> from the given <see cref="object"/> and its properties recursively.
        /// </summary>
        /// <param name="o">the <see cref="object"/> to build the <see cref="Graph"/> from.</param>
        public NodeLink BuildNodes(object o)
        {
            if (o is IEnumerable<object> collection)
            {
                //// Collection serialization
                var myNode = CreateCollectionNode(collection);
                myNode.TypeName = TrustedTypes.GetMember(o).AssemblyQualifiedName;
                myNode.Children.AddRange(collection.Select(i => BuildNodes(i)));
                return myNode.MyLink;
            }
            else
            {
                //// Object serialization
                Type nodeType = TrustedTypes.GetMember(o);

                if (CustomSerializers.ContainsType(nodeType))
                {
                    //// With custom serializer
                    var serializer = CustomSerializers.GetForType(nodeType);
                    var myNode = CreateCustomNode(o);
                    myNode.TypeName = nodeType.AssemblyQualifiedName;
                    myNode.ValueString = serializer.Serialize(o);
                    return myNode.MyLink;
                }
                else
                {
                    //// Default node with reference handling and polymorphism.
                    var myNode = CreateNode(o);
                    myNode.TypeName = nodeType.AssemblyQualifiedName;
                    var myProps = GetProperties(o);
                    IDictionary<string, object> propertyRefs = myNode.Properties;
                    foreach (var p in myProps)
                    {
                        if (IsRefType(p.Value))
                        {
                            var myRef = Nodes.FirstOrDefault(n => n.BasedOn == p.Value);
                            if (myRef != null)
                            {
                                propertyRefs.Add(p.Key, myRef.MyLink);
                            }
                            else
                            {
                                propertyRefs.Add(p.Key, BuildNodes(p.Value));
                            }
                        }
                        else
                        {
                            propertyRefs.Add(p.Key, p.Value);
                        }
                    }
                    return myNode.MyLink;
                }
            }
        }

        private Node CreateNode(object o)
        {
            Node myNode = new Node(Index, o);
            Nodes.Add(myNode);
            Index++;
            return myNode;
        }

        private CollectionNode CreateCollectionNode(IEnumerable<object> o)
        {
            CollectionNode myNode = new CollectionNode(Index, o);
            Nodes.Add(myNode);
            Index++;
            return myNode;
        }

        private CustomValueNode CreateCustomNode(object o)
        {
            CustomValueNode myNode = new CustomValueNode(Index, o);
            Nodes.Add(myNode);
            Index++;
            return myNode;
        }

        private IDictionary<string, object> GetProperties(object o)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            var type = TrustedTypes.GetMember(o);
            var bs = Behaviours.GetBehaviours(type);

            if(bs.HasFlag(GraphBehaviour.IncludeFields))
            {
                foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        properties.Add(f.Name, f.GetValue(o));
                    }
                    catch (Exception ex)
                    {
                        throw new GraphException($"Failed to get field value {type.FullName}.{f.Name}.", ex);
                    }
                }
            }

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Func<PropertyInfo, bool> shouldAdd;
                if (bs.HasFlag(GraphBehaviour.ReadOnly))
                {
                    shouldAdd = p => p.CanRead && p.GetIndexParameters().Length == 0;
                }
                else
                {
                    shouldAdd = p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0;
                }
                if (shouldAdd(prop))
                {
                    try
                    {
                        properties.Add(prop.Name, prop.GetValue(o));
                    }
                    catch(Exception ex)
                    {
                        throw new GraphException($"Failed to get property value {type.FullName}.{prop.Name}.", ex);
                    }
                }
            }
            return properties;
        }

        #endregion
        #region BuildObject

        /// <summary>
        /// Reads the information contained in this <see cref="Graph"/> and creates a graph of .NET <see cref="object"/>s that reflect that data.
        /// </summary>
        public object BuildObject()
        {
            Dictionary<NodeLink, Node> nodeBuilders = new Dictionary<NodeLink, Node>();
            Dictionary<NodeLink, IDictionary<string, object>> remainingProperties = new Dictionary<NodeLink, IDictionary<string, object>>();
            if (!Nodes.Any())
            {
                throw new ArgumentException("No Nodes exist to create an object model from.");
            }
            foreach (var node in Nodes)
            {
                var nodeType = TrustedTypes.GetMember(node.TypeName);
                if (node is CustomValueNode customNode)
                {
                    var serializer = CustomSerializers.GetForType(nodeType);
                    customNode.BasedOn = serializer.Deserialize(customNode.ValueString);
                }
                else
                {
                    try
                    {
                        var myConstructors = nodeType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        bool isConstructed = false;
                        if (!(node is CollectionNode) && Behaviours.GetBehaviours(nodeType).HasFlag(GraphBehaviour.ParameterConstructor))
                        {
                            var availableProps = node.Properties.Where(p => IsValueType(p.Value)).ToArray();
                            foreach (var c in myConstructors.OrderByDescending(c => c.GetParameters().Length))
                            {
                                var ps = c.GetParameters();
                                if (ps.All(p => availableProps.Any(prop =>
                                    p.ParameterType.IsAssignableFrom(prop.GetType())
                                    && p.Name.Equals(prop.Key, StringComparison.OrdinalIgnoreCase))))
                                {
                                    var parameters = ps.Select(p => availableProps.FirstOrDefault(prop =>
                                        p.ParameterType.IsAssignableFrom(prop.GetType())
                                        && p.Name.Equals(prop.Key, StringComparison.OrdinalIgnoreCase)));
                                    remainingProperties.Add(node.MyLink, node.Properties.Except(parameters).ToDictionary());
                                    node.BasedOn = c.Invoke(parameters.Select(prop => prop.Value).ToArray());
                                    isConstructed = true;
                                }
                            }
                        }
                        else if (node is CollectionNode collectionNode && nodeType.IsArray)
                        {
                            int length = collectionNode.Children.Count;
                            var arrayConst = nodeType.GetConstructor(new[] { typeof(int) });
                            if (arrayConst != null)
                            {
                                node.BasedOn = arrayConst.Invoke(new object[] { length });
                                isConstructed = true;
                            }
                            else
                            {
                                throw new GraphException($"Could not create new instance of array {collectionNode.MyLink} - no valid array constructor found.");
                            }
                        }
                        else
                        {
                            var emptyConstructor = myConstructors.FirstOrDefault(c => !c.GetParameters().Any());
                            if (emptyConstructor != null)
                            {
                                node.BasedOn = emptyConstructor.Invoke(new object[0]);
                                remainingProperties.Add(node.MyLink, node.Properties);
                                isConstructed = true;
                            }
                        }

                        if (!isConstructed)
                        {
                            node.BasedOn = FormatterServices.GetUninitializedObject(nodeType);
                            remainingProperties.Add(node.MyLink, node.Properties);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new GraphException($"Failed to create instance of object {nodeType.FullName} [{node.MyLink}].", ex);
                    }
                }

                if (node.BasedOn == null)
                {
                    throw new GraphException($"Attempted to construct object of type {nodeType.FullName} [{node.MyLink}], but constructor method returned null.");
                }
                else
                {
                    nodeBuilders.Add(node.MyLink, node);
                }
            }
            //// Now we have a Dictionary with Node objects (and their constructed .NET objects), we can set all properties on the objects.
            foreach (var node in nodeBuilders.Values)
            {
                var type = TrustedTypes.GetMember(node.BasedOn);
                if(node is CustomValueNode customNode)
                {
                    //// Custom values are already deserialized as they were handled by ICustomSerializers.
                }
                else if (node is CollectionNode collectionNode)
                {
                    //// Collection initialization
                    if (collectionNode.BasedOn is Array array)
                    {
                        //// Array initialization
                        try
                        {
                            for (int i = 0; i < collectionNode.Children.Count; i++)
                            {
                                array.SetValue(nodeBuilders[collectionNode.Children[i]].BasedOn, i);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new GraphException($"Failed to add items to array [{node.MyLink}].", ex);
                        }
                    }
                    else if (collectionNode.BasedOn is IList list)
                    {
                        //// List initialization
                        try
                        {
                            foreach (var item in collectionNode.Children)
                            {
                                list.Add(nodeBuilders[item].BasedOn);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new GraphException($"Failed to add items to list [{node.MyLink}].", ex);
                        }
                    } 
                    else
                    {
                        throw new GraphException($"Collection initialization currently does not support anything other than arrays or IList<T>. Type: {type.FullName}");
                    }
                }
                else
                {
                    //// Object initialization
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var stringProp in remainingProperties[node.MyLink])
                    {
                        var refProp = properties.FirstOrDefault(p => p.Name == stringProp.Key);
                        if (refProp != null && refProp.CanWrite)
                        {
                            if (stringProp.Value is JObject o)
                            {
                                if (o.ContainsKey("Id"))
                                {
                                    try
                                    {
                                        refProp.SetValue(node.BasedOn, GetObject(nodeBuilders[(int)o["Id"]].BasedOn, refProp.PropertyType));
                                    }
                                    catch(Exception ex)
                                    {
                                        throw new GraphException($"Failed to set (object) property value [{node.MyLink}].{stringProp.Key}.", ex);
                                    }
                                }
                                else
                                {
                                    throw new GraphTypeException($"Unknown JSON object, expected NodeLink: {o}");
                                }
                            }
                            else
                            {
                                try
                                { 
                                    refProp.SetValue(node.BasedOn, GetObject(stringProp.Value, refProp.PropertyType));
                                }
                                catch (Exception ex)
                                {
                                    throw new GraphException($"Failed to set (value) property value [{node.MyLink}].{stringProp.Key}.", ex);
                                }
                            }
                        }
                        else
                        {
                            if(Behaviours.GetBehaviours(type).HasFlag(GraphBehaviour.SetFields))
                            {
                                var refField = fields.FirstOrDefault(p => p.Name == stringProp.Key);
                                if(refField != null)
                                {
                                    if (stringProp.Value is JObject o)
                                    {
                                        if (o.ContainsKey("Id"))
                                        {
                                            try
                                            { 
                                                refField.SetValue(node.BasedOn, GetObject(nodeBuilders[(int)o["Id"]].BasedOn, refField.FieldType));
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new GraphException($"Failed to set (object) field value [{node.MyLink}].{stringProp.Key}.", ex);
                                            }
                                        }
                                        else
                                        {
                                            throw new GraphTypeException($"Unknown JSON object, expected NodeLink: {o}");
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            refField.SetValue(node.BasedOn, GetObject(stringProp.Value, refField.FieldType));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new GraphException($"Failed to set (value) field value [{node.MyLink}].{stringProp.Key}.", ex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return nodeBuilders.FirstOrDefault().Value.BasedOn;
        }

        #endregion
        #region Types

        private bool IsRefType(object o)
        {
            return o != null
                && (TrustedTypes.IsMember(o.GetType())
                || (!(o is string) && o.GetType().IsClass));
        }

        private bool IsValueType(object o)
        {
            return o == null
                || o is string
                || !(o.GetType().IsClass);
        }

        private object GetObject(object o, Type desiredType)
        {
            if(o == null)
            {
                return null;
            }
            else if (desiredType.IsAssignableFrom(o.GetType()))
            {
                return o;
            }
            else
            {
                return Convert.ChangeType(o, desiredType);
            }
        }

        #endregion
    }

    internal static class GraphExtensions
    {
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> elements)
        {
            return elements.ToDictionary(e => e.Key, e => e.Value);
        }
    }
}
