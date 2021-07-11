using BassClefStudio.NET.Core.Primitives;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BassClefStudio.NET.Serialization.Types
{
    /// <summary>
    /// A basic default implementation of <see cref="ITypeConfiguration"/> suitable for most serialization.
    /// </summary>
    public class DefaultTypeConfiguration : ITypeConfiguration
    {
        /// <inheritdoc/>
        public ITypeMatch NativeTypes { get; }
            = TypeMatch.Type(
                typeof(string),
                typeof(byte),
                typeof(uint),
                typeof(int),
                typeof(ulong),
                typeof(long),
                typeof(float),
                typeof(double),
                typeof(bool));

        /// <inheritdoc/>
        public ITypeMatch TrustedTypes { get; }
            = TypeMatch.Type(
                typeof(List<>),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(DateTimeZone),
                typeof(DateTimeSpan),
                typeof(Color),
                typeof(Guid),
                typeof(Vector2));
    }

    /// <summary>
    /// A basic implementation of <see cref="ITypeConfiguration"/> allowing for full customization of parameters.
    /// </summary>
    public class CustomTypeConfiguration : ITypeConfiguration
    {
        /// <inheritdoc/>
        public ITypeMatch NativeTypes { get; set; }

        /// <inheritdoc/>
        public ITypeMatch TrustedTypes { get; set; }

        /// <summary>
        /// Creates a new <see cref="CustomTypeConfiguration"/>.
        /// </summary>
        public CustomTypeConfiguration()
        {
            NativeTypes = TrustedTypes = new NoTypeMatch();
        }
    }
}
