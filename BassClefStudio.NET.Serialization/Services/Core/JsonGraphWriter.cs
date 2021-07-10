using BassClefStudio.NET.Core.Structures;
using BassClefStudio.NET.Serialization.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="IGraphWriter"/> that uses JSON to write the <see cref="Graph{TNode, TConnection}"/> to text.
    /// </summary>
    public class JsonGraphWriter : IGraphWriter
    {
        /// <inheritdoc/>
        public Graph<SerializeNode, SerializeDependency> ReadGraph(string text)
        {
            JObject json = JObject.Parse(text);
            string[] assemblyNames = json["assemblies"].ToObject<string[]>();
            List<SerializeNode> nodes = new List<SerializeNode>();
            foreach(JObject node in json["nodes"])
            {
                nodes.Add(ReadNode(node, assemblyNames));
            }

            List<SerializeDependency> dependencies = new List<SerializeDependency>();
            foreach (JObject connection in json["connections"])
            {
                dependencies.Add(ReadConnection(connection, nodes));
            }

            var graph = new Graph<SerializeNode, SerializeDependency>();
            foreach(var n in nodes)
            {
                graph.AddNode(n);
            }
            foreach(var d in dependencies)
            {
                graph.AddConnection(d);
            }
            return graph;
        }

        private SerializeNode ReadNode(JObject node, string[] assemblyNames)
        {
            Type type = null;
            if (node["type"].Type == JTokenType.Object)
            {
                type = ResolveType((JObject)node["type"], assemblyNames);
            }
            var nodeObj = new SerializeNode((string)node["id"], type);
            if(node.ContainsKey("value"))
            {
                nodeObj.Value = node["value"];
                nodeObj.IsNative = true;
            }
            else
            {
                nodeObj.Value = null;
                nodeObj.IsNative = false;
            }

            return nodeObj;
        }

        private Type ResolveType(JObject typeJson, string[] assemblyNames)
        {
            if (typeJson.Type == JTokenType.Null)
            {
                return null;
            }
            else
            {
                string assemblyName = assemblyNames[(int)typeJson["assembly"]];
                string typeName = (string)typeJson["name"];
                Type type = Type.GetType(Assembly.CreateQualifiedName(assemblyName, typeName), true);
                if(typeJson.ContainsKey("generic"))
                {
                    var genericTypes = typeJson["generic"].OfType<JObject>().Select(o => ResolveType(o, assemblyNames));
                    return type.MakeGenericType(genericTypes.ToArray());
                }
                else
                {
                    return type;
                }
            }
        }

        private SerializeDependency ReadConnection(JObject connection, IEnumerable<SerializeNode> nodes)
        {
            string id = (string)connection["id"];
            string startId = (string)connection["start"];
            string endId = (string)connection["end"];
            return new SerializeDependency(id, nodes.First(n => n.Id == startId), nodes.First(n => n.Id == endId));
        }

        #region Write

        /// <inheritdoc/>
        public string WriteGraph(Graph<SerializeNode, SerializeDependency> graph)
        {
            List<Assembly> assemblies = new List<Assembly>(graph.Nodes.Where(n => n.DesiredType != null).Select(n => n.DesiredType.Assembly).Distinct());
            var nodes = graph.Nodes.Select(n => WriteNode(n, assemblies));
            var connections = graph.Connections.Select(c => WriteConnection(c));

            JObject json = new JObject(
                new JProperty(
                    "assemblies",
                    assemblies.Select(a => a.FullName)),
                new JProperty(
                    "nodes",
                    nodes),
                new JProperty(
                    "connections",
                    connections));

            return json.ToString(Newtonsoft.Json.Formatting.None);
        }

        private JObject WriteNode(SerializeNode node, IList<Assembly> assemblies)
        {
            JObject typeJson = null;
            if (node.DesiredType != null)
            {
                typeJson = GetType(node.DesiredType, assemblies);
            }

            if (node.IsNative)
            {
                return new JObject(
                    new JProperty("id", node.Id),
                    new JProperty("type", typeJson),
                    new JProperty("value", node.Value));
            }
            else
            {
                return new JObject(
                    new JProperty("id", node.Id),
                    new JProperty("type", typeJson));
            }
        }

        private JObject GetType(Type type, IList<Assembly> assemblies)
        {
            int assemblyIndex = assemblies.IndexOf(type.Assembly);
            JObject typeJson;
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                typeJson = new JObject(
                    new JProperty("assembly", assemblyIndex),
                    new JProperty("name", genericType.FullName),
                    new JProperty("generic", new JArray(
                        type.GenericTypeArguments.Select(t => GetType(t, assemblies)))));
            }
            else
            {
                typeJson = new JObject(
                    new JProperty("assembly", assemblyIndex),
                    new JProperty("name", type.FullName));
            }

            return typeJson;
        }

        private JObject WriteConnection(SerializeDependency connection)
        {
            return new JObject(
                new JProperty("id", connection.DependencyId),
                new JProperty("start", connection.StartNode.Id),
                new JProperty("end", connection.EndNode.Id));
        }

        #endregion
    }
}
