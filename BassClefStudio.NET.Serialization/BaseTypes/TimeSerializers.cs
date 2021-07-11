using BassClefStudio.NET.Core.Primitives;
using BassClefStudio.NET.Serialization.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.BaseTypes
{
    /// <summary>
    /// A <see cref="StringSerializer{T}"/> for the <see cref="DateTime"/> primitive.
    /// </summary>
    public class DateTimeSerializer : StringSerializer<DateTime>
    {
        /// <inheritdoc/>
        public override GraphPriority Priority { get; } = GraphPriority.Primitive;

        /// <inheritdoc/>
        protected override DateTime Parse(string value) => DateTime.Parse(value);
    }

    /// <summary>
    /// A <see cref="StringSerializer{T}"/> for the <see cref="DateTimeOffset"/> primitive.
    /// </summary>
    public class DateTimeOffsetSerializer : StringSerializer<DateTimeOffset>
    {
        /// <inheritdoc/>
        public override GraphPriority Priority { get; } = GraphPriority.Primitive;

        /// <inheritdoc/>
        protected override DateTimeOffset Parse(string value) => DateTimeOffset.Parse(value);
    }

    /// <summary>
    /// A <see cref="CustomSerializer{T}"/> for the <see cref="DateTimeZone"/> primitive.
    /// </summary>
    public class DateTimeZoneSerializer : CustomSerializer<DateTimeZone>
    {
        /// <inheritdoc/>
        public override GraphPriority Priority { get; } = GraphPriority.Primitive;

        /// <inheritdoc/>
        protected override IDictionary<string, object> GetPropertiesInternal(DateTimeZone value)
        {
            return new Dictionary<string, object>()
            {
                { "DateTime", value.DateTime },
                { "TimeZone", value.TimeZone.Id }
            };
        }

        /// <inheritdoc/>
        protected override DateTimeZone ConstructInternal(IDictionary<string, object> subGraph)
        {
            return new DateTimeZone(
                subGraph["DateTime"].As<DateTime>(),
                TimeZoneInfo.FindSystemTimeZoneById(subGraph["TimeZone"].As<string>()));      
        }
    }

    /// <summary>
    /// A <see cref="CustomSerializer{T}"/> for the <see cref="DateTimeSpan"/> primitive.
    /// </summary>
    public class DateTimeSpanSerializer : CustomSerializer<DateTimeSpan>
    {
        /// <inheritdoc/>
        public override GraphPriority Priority { get; } = GraphPriority.Primitive;

        /// <inheritdoc/>
        protected override IDictionary<string, object> GetPropertiesInternal(DateTimeSpan value)
        {
            return new Dictionary<string, object>()
            {
                { "Start", value.StartDate },
                { "End", value.EndDate }
            };
        }

        /// <inheritdoc/>
        protected override DateTimeSpan ConstructInternal(IDictionary<string, object> subGraph)
        {
            return new DateTimeSpan(
                subGraph["Start"].As<DateTimeOffset>(),
                subGraph["End"].As<DateTimeOffset>());
        }
    }
}
