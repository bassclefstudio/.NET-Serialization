using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="Attribute"/> that specifies to <see cref="IGraphService"/>s that this value should not be included in serialization dependencies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreSerializeAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="IgnoreSerializeAttribute"/>.
        /// </summary>
        public IgnoreSerializeAttribute()
        { }
    }
}
