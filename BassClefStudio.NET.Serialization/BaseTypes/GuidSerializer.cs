using BassClefStudio.NET.Serialization.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.BaseTypes
{
    /// <summary>
    /// A <see cref="StringSerializer{T}"/> for the <see cref="Guid"/> primitive.
    /// </summary>
    public class GuidSerializer : StringSerializer<Guid>
    {
        /// <inheritdoc/>
        public override GraphPriority Priority { get; } = GraphPriority.Primitive;

        /// <inheritdoc/>
        protected override string GetValue(Guid value) => value.ToString("N");

        /// <inheritdoc/>
        protected override Guid Parse(string value) => Guid.Parse(value);
    }
}
