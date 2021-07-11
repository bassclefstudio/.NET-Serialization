using BassClefStudio.NET.Core.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.Model
{
    /// <summary>
    /// A service that can write and read <see cref="Graph{TNode, TConnection}"/>s used by the <see cref="GraphSerializer"/> to text.
    /// </summary>
    public interface IGraphWriter
    {
        /// <summary>
        /// Creates a <see cref="string"/> to represent the provided <see cref="Graph{TNode, TConnection}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="Graph{TNode, TConnection}"/> of serialization information to store.</param>
        /// <returns>A <see cref="string"/> serialized representation of the graph.</returns>
        string WriteGraph(Graph<SerializeNode, SerializeDependency> graph);

        /// <summary>
        /// Creates the new <see cref="Graph{TNode, TConnection}"/> from its <see cref="string"/> representation.
        /// </summary>
        /// <param name="text">The serialized graph in text.</param>
        /// <returns>A new <see cref="Graph{TNode, TConnection}"/> containing more generic serialization information.</returns>
        Graph<SerializeNode, SerializeDependency> ReadGraph(string text);

    }
}
