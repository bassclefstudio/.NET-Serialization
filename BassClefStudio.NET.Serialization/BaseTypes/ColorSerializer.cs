using BassClefStudio.NET.Core.Primitives;
using BassClefStudio.NET.Serialization.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.BaseTypes
{
    /// <summary>
    /// A <see cref="CustomSerializer{T}"/> for the <see cref="Color"/> primitive.
    /// </summary>
    public class ColorSerializer : CustomSerializer<Color>
    {
        /// <inheritdoc/>
        protected override IDictionary<string, object> GetPropertiesInternal(Color value)
        {
            return new Dictionary<string, object>()
            {
                { "R", value.R },
                { "G", value.G },
                { "B", value.B },
                { "A", value.A }
            };
        }

        /// <inheritdoc/>
        protected override Color ConstructInternal(IDictionary<string, object> subGraph)
        {
            byte r = subGraph["R"].As<byte>();
            byte g = subGraph["G"].As<byte>();
            byte b = subGraph["B"].As<byte>();
            byte a = subGraph["A"].As<byte>();
            return new Color(r, g, b, a);
        }
    }
}
