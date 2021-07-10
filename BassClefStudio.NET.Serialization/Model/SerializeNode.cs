using BassClefStudio.NET.Core.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.Model
{
    /// <summary>
    /// An <see cref="INode"/> in the graph serialization model, containing a single serialized <see cref="object"/> value.
    /// </summary>
    public class SerializeNode : INode
    {
        /// <inheritdoc/>
        public string Id { get; }

        /// <summary>
        /// The <see cref="object"/> value this <see cref="SerializeNode"/> encapsulates.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// A <see cref="bool"/>  indicating whether <see cref="Value"/> can be serialized directly as a native type (e.g. string or numeric types).
        /// </summary>
        public bool IsNative { get; set; } = false;

        /// <summary>
        /// The desired <see cref="Type"/> of <see cref="Value"/>.
        /// </summary>
        public Type DesiredType { get; }

        /// <summary>
        /// Creates a new <see cref="SerializeNode"/>.
        /// </summary>
        /// <param name="id">The <see cref="SerializeNode"/>'s unique <see cref="string"/> ID.</param>
        /// <param name="value">The <see cref="object"/> value this <see cref="SerializeNode"/> encapsulates.</param>
        public SerializeNode(string id, object value)
        {
            Id = id;
            Value = value;
            DesiredType = Value?.GetType();
        }

        /// <summary>
        /// Creates a new <see cref="SerializeNode"/>.
        /// </summary>
        /// <param name="id">The <see cref="SerializeNode"/>'s unique <see cref="string"/> ID.</param>
        /// <param name="desiredType">The desired <see cref="Type"/> of <see cref="Value"/>.</param>
        public SerializeNode(string id, Type desiredType)
        {
            Id = id;
            DesiredType = desiredType;
        }

        /// <inheritdoc/>
        public bool Equals(INode other) => other != null && this.Id == other.Id;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Id} ({(Value != null ? Value : DesiredType.Name)})";
        }
    }
}
