using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Serialization.Graphs
{
    /// <summary>
    /// Represents a graph of <see cref="Node"/>s, broken down to preserve references and including all public properties of provided objects.
    /// </summary>
    public class NodeGraph
    {
        /// <summary>
        /// A collection of all of the <see cref="Node"/>s in this data structure.
        /// </summary>
        public List<Node> Nodes { get; }

        private int Index = 0;
        /// <summary>
        /// Creates a new empty <see cref="NodeGraph"/>.
        /// </summary>
        public NodeGraph()
        {
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Builds the entire <see cref="NodeGraph"/> from the given <see cref="object"/> and its properties recursively.
        /// </summary>
        /// <param name="o">the <see cref="object"/> to build the <see cref="NodeGraph"/> from.</param>
        public NodeLink Build(object o)
        {
            Node myNode = new Node(Index);
            Index++;
            myNode.TypeName = o.GetType().FullName;
            var myProps = GetProperties(o);
            IDictionary<string, object> propertyRefs = myNode.Properties;
            foreach (var p in myProps)
            {
                if(IsRefType(p.Value))
                {
                    var myRef = Nodes.FirstOrDefault(n => n.BasedOn == p.Value);
                    if(myRef != null) 
                    {
                        propertyRefs.Add(p.Key, myRef.MyLink);
                    }
                    else
                    {
                        propertyRefs.Add(p.Key, Build(p.Value));
                    }
                }
                else
                {
                    propertyRefs.Add(p.Key, p.Value);
                }
            }
            return myNode.MyLink;
        }

        private IDictionary<string, object> GetProperties(object o)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            var type = o.GetType();
            foreach(var prop in type.GetProperties().Where(i => i.CanRead && i.CanWrite))
            {
                properties.Add(prop.Name, prop.GetValue(o));
            }
            return properties;
        }

        private bool IsRefType(object o)
        {
            return o is ISerializable;
        }
    }
}
