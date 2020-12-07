using JsonKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.Graphs
{
    /// <summary>
    /// Represents a <see cref="Node"/> that contains a collection of other <see cref="Node"/>s.
    /// </summary>
    [JsonKnownThisType("collection")]
    public class CollectionNode : Node
    {
        /// <summary>
        /// The collection of <see cref="Node"/>s that this collection contains.
        /// </summary>
        public List<NodeLink> Children { get; }

        /// <summary>
        /// Creates a new <see cref="Node"/>.
        /// </summary>
        /// <param name="myLink">The <see cref="NodeLink"/> that can be used for referring to this <see cref="Node"/> instance.</param>
        /// <param name="basedOn">The <see cref="object"/> that this <see cref="Node"/> represents.</param>
        public CollectionNode(NodeLink myLink, object basedOn) : base(myLink, basedOn)
        {
            Children = new List<NodeLink>();
        }
    }
}
