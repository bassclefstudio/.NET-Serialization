using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Serialization.Graphs
{
    /// <summary>
    /// An <see cref="Exception"/> thrown when an <see cref="object"/> in a <see cref="Graph"/> is of an unexpected or untrusted type.
    /// </summary>
    [Serializable]
    public class GraphTypeException : GraphException
    {
        /// <inheritdoc/>
        public GraphTypeException() { }
        /// <inheritdoc/>
        public GraphTypeException(string message) : base(message) { }
        /// <inheritdoc/>
        public GraphTypeException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected GraphTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// An <see cref="Exception"/> thrown if a <see cref="Graph"/> encounters an error during <see cref="Graph.BuildNodes(object)"/> or <see cref="Graph.BuildObject"/>.
    /// </summary>
    [Serializable]
    public class GraphException : Exception
    {
        /// <inheritdoc/>
        public GraphException() { }
        /// <inheritdoc/>
        public GraphException(string message) : base(message) { }
        /// <inheritdoc/>
        public GraphException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected GraphException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
