using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.CustomTypes
{
    /// <summary>
    /// Native serializer for <see cref="string"/> values.
    /// </summary>
    public class StringSerializer : StringSerializer<string>
    {
        /// <inheritdoc/>
        public override string ParseValue(string value)
        {
            return value;
        }

        /// <inheritdoc/>
        public override string GetString(string value)
        {
            return value;
        }
    }

    /// <summary>
    /// Native serializer for <see cref="int"/> values.
    /// </summary>
    public class IntSerializer : StringSerializer<int>
    {
        /// <inheritdoc/>
        public override int ParseValue(string value)
        {
            return int.Parse(value);
        }

        /// <inheritdoc/>
        public override string GetString(int value)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// Native serializer for <see cref="long"/> values.
    /// </summary>
    public class LongSerializer : StringSerializer<long>
    {
        /// <inheritdoc/>
        public override long ParseValue(string value)
        {
            return long.Parse(value);
        }

        /// <inheritdoc/>
        public override string GetString(long value)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// Native serializer for <see cref="double"/> values.
    /// </summary>
    public class DoubleSerializer : StringSerializer<double>
    {
        /// <inheritdoc/>
        public override double ParseValue(string value)
        {
            return double.Parse(value);
        }

        /// <inheritdoc/>
        public override string GetString(double value)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// Native serializer for <see cref="float"/> values.
    /// </summary>
    public class FloatSerializer : StringSerializer<float>
    {
        /// <inheritdoc/>
        public override float ParseValue(string value)
        {
            return float.Parse(value);
        }

        /// <inheritdoc/>
        public override string GetString(float value)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// Native serializer for <see cref="decimal"/> values.
    /// </summary>
    public class DecimalSerializer : StringSerializer<decimal>
    {
        /// <inheritdoc/>
        public override decimal ParseValue(string value)
        {
            return decimal.Parse(value);
        }

        /// <inheritdoc/>
        public override string GetString(decimal value)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// Native serializer for <see cref="bool"/> values.
    /// </summary>
    public class BoolSerializer : StringSerializer<bool>
    {
        /// <inheritdoc/>
        public override bool ParseValue(string value)
        {
            return bool.Parse(value);
        }

        /// <inheritdoc/>
        public override string GetString(bool value)
        {
            return value.ToString();
        }
    }
}
