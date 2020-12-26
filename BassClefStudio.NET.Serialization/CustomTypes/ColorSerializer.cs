using BassClefStudio.NET.Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.CustomTypes
{
    /// <summary>
    /// An <see cref="ICustomSerializer"/> for dealing with <see cref="Color"/>s
    /// </summary>
    public class ColorSerializer : CustomSerializer<Color>
    {
        /// <inheritdoc/>
        public override Func<Color, object>[] GetProperties { get; } = new Func<Color, object>[]
        {
            c => c.A,
            c => c.R,
            c => c.G,
            c => c.B
        };

        /// <inheritdoc/>
        public override Color DeserializeObject(string[] values)
        {
            return new Color(byte.Parse(values[1]), byte.Parse(values[2]), byte.Parse(values[3]), byte.Parse(values[0]));
        }
    }
}
