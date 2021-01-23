using BassClefStudio.NET.Core.Primitives;
using BassClefStudio.NET.Serialization.Graphs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.CustomTypes
{
    /// <summary>
    /// An <see cref="ICustomSerializer"/> for dealing with <see cref="DateTimeOffset"/>s.
    /// </summary>
    public class DateTimeOffsetSerializer : StringSerializer<DateTimeOffset>
    {
        /// <inheritdoc/>
        public override DateTimeOffset ParseValue(string value)
        {
            return DateTimeOffset.Parse(value);
        }
    }

    /// <summary>
    /// An <see cref="ICustomSerializer"/> for dealing with <see cref="DateTimeZone"/>s.
    /// </summary>
    public class DateTimeZoneSerializer : CustomSerializer<DateTimeZone>
    {
        /// <inheritdoc/>
        public override Func<DateTimeZone, object>[] GetProperties { get; } = new Func<DateTimeZone, object>[]
        {
            d => d.DateTime,
            d => d.TimeZone.Id
        };

        /// <inheritdoc/>
        public override DateTimeZone DeserializeObject(string[] values)
        {
            return new DateTimeZone(DateTime.Parse(values[0]), TimeZoneInfo.FindSystemTimeZoneById(values[1]));
        }
    }

    /// <summary>
    /// An <see cref="ICustomSerializer"/> for dealing with <see cref="DateTimeSpan"/>s.
    /// </summary>
    public class DateTimeSpanSerializer : CustomSerializer<DateTimeSpan>
    {
        /// <inheritdoc/>
        public override Func<DateTimeSpan, object>[] GetProperties { get; } = new Func<DateTimeSpan, object>[]
        {
            v => v.StartDate,
            v => v.EndDate
        };

        /// <inheritdoc/>
        public override DateTimeSpan DeserializeObject(string[] values)
        {
            return new DateTimeSpan(DateTimeOffset.Parse(values[0]), DateTimeOffset.Parse(values[1]));
        }
    }
}
