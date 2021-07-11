using BassClefStudio.NET.Core.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.Model
{
    /// <summary>
    /// Represents a dependency one serialized <see cref="SerializeNode"/> has on another.
    /// </summary>
    public class SerializeDependency : IConnection<SerializeNode>
    {
        /// <summary>
        /// The unique <see cref="string"/> key which is used to identify this dependency to the parent object.
        /// </summary>
        public string DependencyId { get; }

        /// <summary>
        /// The parent <see cref="SerializeNode"/> node.
        /// </summary>
        public SerializeNode StartNode { get; }

        /// <summary>
        /// The property <see cref="SerializeNode"/> that <see cref="StartNode"/> depends on.
        /// </summary>
        public SerializeNode EndNode { get; }

        /// <summary>
        /// Creates a new <see cref="SerializeDependency"/>.
        /// </summary>
        /// <param name="id">The unique <see cref="string"/> key which is used to identify this dependency to the parent object.</param>
        /// <param name="parent">The parent <see cref="SerializeNode"/> node.</param>
        /// <param name="child">The property <see cref="SerializeNode"/> that <see cref="StartNode"/> depends on.</param>
        public SerializeDependency(string id, SerializeNode parent, SerializeNode child)
        {
            DependencyId = id;
            StartNode = parent;
            EndNode = child;
        }

        /// <inheritdoc/>
        public ConnectionMode Mode => ConnectionMode.Forwards;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{{StartNode}}} => {{{EndNode}}}";
        }
    }
}
