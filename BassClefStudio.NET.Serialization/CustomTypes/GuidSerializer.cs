using BassClefStudio.NET.Serialization.Graphs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.CustomTypes
{
    /// <summary>
    /// An <see cref="ICustomSerializer"/> for dealing with <see cref="Guid"/>s
    /// </summary>
    public class GuidSerializer : StringSerializer<Guid>
    {
        /// <inheritdoc/>
        public override Guid ParseValue(string value)
        {
            return Guid.Parse(value);
        }

        /// <inheritdoc/>
        public override string GetString(Guid value)
        {
            return value.ToString("N");
        }
    }
}
