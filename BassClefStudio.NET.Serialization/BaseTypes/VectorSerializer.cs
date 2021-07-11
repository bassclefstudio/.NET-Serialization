using BassClefStudio.NET.Serialization.Services;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BassClefStudio.NET.Serialization.BaseTypes
{
    /// <summary>
    /// A <see cref="CustomSerializer{T}"/> for the <see cref="Vector2"/> primitive.
    /// </summary>
    public class Vector2Serializer : CustomSerializer<Vector2>
    {
        /// <inheritdoc/>
        public override GraphPriority Priority { get; } = GraphPriority.Primitive;

        /// <inheritdoc/>
        protected override IDictionary<string, object> GetPropertiesInternal(Vector2 value)
        {
            return new Dictionary<string, object>()
            {
                { "X", value.X },
                { "Y", value.Y }
            };
        }

        /// <inheritdoc/>
        protected override Vector2 ConstructInternal(IDictionary<string, object> subGraph)
        {
            return new Vector2(
                subGraph["X"].As<float>(),
                subGraph["Y"].As<float>());
        }
    }
}
