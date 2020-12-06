using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Serialization.Graphs
{
    /// <summary>
    /// Represents an <see cref="object"/> in a data graph.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// The <see cref="NodeLink"/> that can be used for referring to this <see cref="Node"/> instance.
        /// </summary>
        public NodeLink MyLink { get; }

        /// <summary>
        /// The <see cref="object"/> that this <see cref="Node"/> represents.
        /// </summary>
        public object BasedOn { get; set; }

        /// <summary>
        /// The name of the type of this <see cref="Node"/>.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// A collection of properties with <see cref="string"/> keys - these are either basic (value) objects, or <see cref="NodeLink"/>(s) to other reference <see cref="Node"/>s.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Creates a new <see cref="Node"/>.
        /// </summary>
        /// <param name="myLink">The <see cref="NodeLink"/> that can be used for referring to this <see cref="Node"/> instance.</param>
        public Node(NodeLink myLink)
        {
            MyLink = myLink;
            Properties = new Dictionary<string, object>();
        }
    }
}
