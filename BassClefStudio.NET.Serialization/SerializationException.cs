using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization
{
    /// <summary>
    /// An <see cref="Exception"/> thrown when an <see cref="ISerializationService"/> fails in (de)serializing a given object.
    /// </summary>
    [Serializable]
    public class SerializationException : Exception
    {
        /// <inheritdoc/>
        public SerializationException() { }
        /// <inheritdoc/>
        public SerializationException(string message) : base(message) { }
        /// <inheritdoc/>
        public SerializationException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected SerializationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    /// <summary>
    /// An <see cref="Exception"/> thrown if required <see cref="ITypeMatch"/> expressions fail while (de)serializing a given object with an <see cref="ISerializationService"/>.
    /// </summary>
    [Serializable]
    public class TypeMatchException : SerializationException
    {
        /// <summary>
        /// The <see cref="ITypeMatch"/> expression that failed.
        /// </summary>
        public ITypeMatch TypeMatch { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of the bad object.
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Creates a new <see cref="TypeMatchException"/>.
        /// </summary>
        /// <param name="typeMatch">The <see cref="ITypeMatch"/> expression that failed.</param>
        /// <param name="objectType">The <see cref="Type"/> of the bad object.</param>
        public TypeMatchException(ITypeMatch typeMatch, Type objectType) : base($"A required type match failed to match the provided type \"{objectType.Name}\".")
        {
            TypeMatch = typeMatch;
            ObjectType = objectType;
        }
        /// <inheritdoc/>
        protected TypeMatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
