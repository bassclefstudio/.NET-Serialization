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
    public class VectorSerializer : ICustomSerializer
    {
        /// <inheritdoc/>
        public TypeGroup ApplicableTypes { get; } = new TypeGroup(typeof(Vector2));

        /// <inheritdoc/>
        public object Deserialize(string value)
        {
            var array = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if(array.Length != 2)
            {
                throw new GraphException("Vector2 serialization expects two float parameters.");
            }
            var fs = array.Select(c => float.Parse(c)).ToArray();
            return new Vector2(fs[0], fs[1]);
        }

        /// <inheritdoc/>
        public string Serialize(object o)
        {
            var v = (Vector2)o;
            return $"{v.X};{v.Y}";
        }
    }
}
