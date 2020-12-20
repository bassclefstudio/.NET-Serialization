using BassClefStudio.NET.Serialization.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BassClefStudio.NET.Serialization.CustomTypes
{
    /// <summary>
    /// An <see cref="ICustomSerializer"/> for dealing with <see cref="Guid"/>s
    /// </summary>
    public class VectorSerializer : CustomSerializer<Vector2>
    {
        /// <inheritdoc/>
        public override Func<Vector2, object>[] GetProperties { get; } = new Func<Vector2, object>[]
        {
            v => v.X,
            v => v.Y
        };

        /// <inheritdoc/>
        public override Vector2 DeserializeObject(string[] values)
        {
            return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        }
    }
}
