using BassClefStudio.NET.Serialization.Graphs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.CustomTypes
{
    /// <summary>
    /// An <see cref="ICustomSerializer"/> for dealing with <see cref="Guid"/>s
    /// </summary>
    public class GuidSerializer : ICustomSerializer
    {
        /// <inheritdoc/>
        public TypeGroup ApplicableTypes { get; } = new TypeGroup(typeof(Guid));

        /// <inheritdoc/>
        public object Deserialize(string value)
        {
            return Guid.Parse(value);
        }

        /// <inheritdoc/>
        public string Serialize(object o)
        {
            return ((Guid)o).ToString("N");
        }
    }
}
