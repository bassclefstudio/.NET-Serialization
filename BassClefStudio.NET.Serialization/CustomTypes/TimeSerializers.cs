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
    public class DateTimeOffsetSerializer : ICustomSerializer
    {
        /// <inheritdoc/>
        public TypeGroup ApplicableTypes { get; } = new TypeGroup(typeof(DateTimeOffset));

        /// <inheritdoc/>
        public object Deserialize(string value)
        {
            return DateTimeOffset.Parse(value);
        }

        /// <inheritdoc/>
        public string Serialize(object o)
        {
            if (o is DateTimeOffset offset)
            {
                return offset.ToString();
            }
            else
            {
                throw new ArgumentException($"DateTimeOffsetSerializer expects objects of type DateTimeOffset - object type {o?.GetType().Name}");
            }
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
