using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services
{
    /// <summary>
    /// Represents an object that is serializable by an <see cref="ISerializationService"/>.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Creates or retrieves an <see cref="IDictionary{TKey, TValue}"/> of <see cref="object"/> values that should be serialized as the given <see cref="string"/> keys.
        /// </summary>
        /// <returns>The <see cref="IDictionary{TKey, TValue}"/> serialization information for this object.</returns>
        IDictionary<string, object> GetProperties();

        /// <summary>
        /// Populates this <see cref="ISerializable"/>'s properties and other values from the given <see cref="IDictionary{TKey, TValue}"/> provided by the serializer.
        /// </summary>
        /// <param name="subGraph">The <see cref="IDictionary{TKey, TValue}"/> containing the currently defined <see cref="object"/> values under their <see cref="string"/> keys.</param>
        void PopulateObject(IDictionary<string, object> subGraph);
    }
}
