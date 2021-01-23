using BassClefStudio.NET.Core.Primitives;
using BassClefStudio.NET.Serialization.Graphs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.CustomTypes
{
    /// <summary>
    /// Represents static properties that manage the lists of natively trusted types, assemblies, and <see cref="ICustomSerializer"/>s.
    /// </summary>
    public static class NativeTypes
    {
        /// <summary>
        /// Gets a static collection of <see cref="ICustomSerializer"/>s that can be added by default to <see cref="Graph"/>s.
        /// </summary>
        public static ICustomSerializer[] DefaultCustomSerializers { get; } = new ICustomSerializer[]
        {
            new ColorSerializer(),
            new GuidSerializer(),
            new VectorSerializer(),
            new DateTimeOffsetSerializer(),
            new DateTimeZoneSerializer(),
            new DateTimeSpanSerializer()
        };

        /// <summary>
        /// Gets the static array of <see cref="Type"/>s that <see cref="Graph"/>s trust by default. This includes basic types for collections such as <see cref="List{T}"/>.
        /// </summary>
        public static Type[] DefaultTrustedTypes { get; } = new Type[]
        {
            typeof(List<>),
            typeof(ObservableCollection<>),
            typeof(Array),
            typeof(Vector2),
            typeof(Guid),
            typeof(DateTimeOffset),
            typeof(DateTimeZone),
            typeof(Color),
            typeof(DateTimeSpan)
        };

        /// <summary>
        /// Gets the static array of <see cref="Assembly"/>s that <see cref="Graph"/>s trust by default.
        /// </summary>
        public static Assembly[] DefaultTrustedAssemblies { get; } = new Assembly[]
        {
        };
    }
}
