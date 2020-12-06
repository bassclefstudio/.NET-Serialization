using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace BassClefStudio.Serialization.Graphs
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
        /// A collection of known <see cref="Assembly"/> references (all types in the assembly have the potential to be serialized).
        /// </summary>
        public Assembly[] KnownAssemblies { get; }

        /// <summary>
        /// A collection of known <see cref="Type"/> references (all types have the potential to be serialized).
        /// </summary>
        public Type[] KnownTypes { get; }

        private int Index = 0;

        /// <summary>
        /// Creates a new empty <see cref="Graph"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        public Graph(params Assembly[] knownAssemblies)
        {
            Nodes = new List<Node>();
            KnownAssemblies = knownAssemblies;
            KnownTypes = new Type[0];
        }

        /// <summary>
        /// Creates a new empty <see cref="Graph"/>.
        /// </summary>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public Graph(params Type[] knownTypes)
        {
            Nodes = new List<Node>();
            KnownAssemblies = new Assembly[0];
            KnownTypes = knownTypes;
        }

        /// <summary>
        /// Creates a new empty <see cref="Graph"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public Graph(Assembly[] knownAssemblies, Type[] knownTypes)
        {
            Nodes = new List<Node>();
            KnownAssemblies = knownAssemblies;
            KnownTypes = knownTypes;
        }

        #region BuildNode

        /// <summary>
        /// Builds the entire <see cref="Graph"/> from the given <see cref="object"/> and its properties recursively.
        /// </summary>
        /// <param name="o">the <see cref="object"/> to build the <see cref="Graph"/> from.</param>
        public NodeLink BuildNodes(object o)
        {
            var myNode = CreateNode(o);
            myNode.TypeName = GetTrustedType(o).AssemblyQualifiedName;
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

        private Type GetTrustedType(object o)
        {
            var type = o.GetType();
            if (KnownAssemblies.Contains(type.Assembly))
            {
                return type;
            }
            else if (KnownTypes.Contains(type))
            {
                return type;
            }
            else
            {
                throw new GraphTypeException($"The type {type.FullName} of this object is not trusted.");
            }
        }

        private Node CreateNode(object o)
        {
            Node myNode = new Node(Index, o);
            Nodes.Add(myNode);
            Index++;
            return myNode;
        }

        private IDictionary<string, object> GetProperties(object o)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            var type = GetTrustedType(o);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                properties.Add(prop.Name, prop.GetValue(o));
            }
            return properties;
        }

        private bool IsRefType(object o)
        {
            return o is ISerializable;
            //return o.GetType().IsClass;
        }

        #endregion
        #region Read

        /// <summary>
        /// Reads the information contained in this <see cref="Graph"/> and creates a graph of .NET <see cref="object"/>s that reflect that data.
        /// </summary>
        public object BuildObject()
        {
            Dictionary<NodeLink, Node> nodeBuilders = new Dictionary<NodeLink, Node>();
            if(!Nodes.Any())
            {
                throw new ArgumentException("No Nodes exist to create an object model from.");
            }
            foreach (var node in Nodes)
            {
                var nodeType = GetTrustedType(node.TypeName);
                var nodeConsts = nodeType.GetConstructors();
                ConstructorInfo parameterless = nodeConsts.FirstOrDefault(c => !c.GetParameters().Any());
                if (parameterless != null)
                {
                    var myObject = parameterless.Invoke(new object[0]);
                    node.BasedOn = myObject;
                    nodeBuilders.Add(node.MyLink, node);
                }
                else
                {
                    throw new GraphException($"Currently cannot create an instance of an object without a parameterless constructor. Type: {nodeType.FullName}.");
                }
            }
            //// Now we have a Dictionary with Node objects (and their constructed .NET objects), we can set all properties on the objects.
            foreach (var node in nodeBuilders.Values)
            {
                var type = GetTrustedType(node.BasedOn);
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach(var stringProp in node.Properties)
                {
                    var refProp = properties.FirstOrDefault(p => p.Name == stringProp.Key);
                    if(refProp != null)
                    {
                        if(stringProp.Value is JObject o)
                        {
                            if (o.ContainsKey("Id"))
                            {
                                refProp.SetValue(node.BasedOn, nodeBuilders[(int)o["Id"]].BasedOn);
                            }
                            else
                            {
                                throw new GraphTypeException($"Unknown JSON object, expected NodeLink: {o}");
                            }
                        }
                        else
                        {
                            refProp.SetValue(node.BasedOn, stringProp.Value);
                        }
                    }
                    else
                    {
                        throw new GraphException($"Could not find property {stringProp.Value} on type {type.FullName}.");
                    }
                }
            }
            return nodeBuilders.FirstOrDefault().Value.BasedOn;
        }

        private Type GetTrustedType(string typeName)
        {
            IEnumerable<Type> trustedTypes = KnownAssemblies.SelectMany(a => a.GetTypes()).Concat(KnownTypes);
            var foundType = trustedTypes.FirstOrDefault(t => t.AssemblyQualifiedName == typeName);
            if (foundType != null)
            {
                return foundType;
            }
            else
            {
                throw new GraphTypeException($"The type {typeName} is not a trusted type.");
            }
        }

        #endregion
    }
}
