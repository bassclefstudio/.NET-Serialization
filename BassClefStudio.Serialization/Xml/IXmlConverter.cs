using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BassClefStudio.Serialization.Xml
{
    /// <summary>
    /// Represents a JSON-specific <see cref="IConverter{TFrom, TTo}"/> for converting objects to <see cref="JObject"/>s.
    /// </summary>
    /// <typeparam name="T">The type of object to convert from.</typeparam>
    public interface IToXmlConverter<T> : IConverter<T, XElement>
    { }

    /// <summary>
    /// Represents a JSON-specific <see cref="IConverter{TFrom, TTo}"/> for converting objects from <see cref="JToken"/>s.
    /// </summary>
    /// <typeparam name="T">The type of object to convert to.</typeparam>
    public interface IFromXmlConverter<T> : IConverter<XElement, T>
    { }

    public static class XmlConverterExtensions
    {
        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the given <see cref="XElement"/> is not null and has a <see cref="XElement.Name"/> equal to the given <see cref="string"/> value.
        /// </summary>
        /// <param name="xml">The <see cref="XElement"/> to check.</param>
        /// <param name="type">The requested value of the "Type" property.</param>
        public static bool IsXmlType(this XElement xml, string type)
        {
            if (xml != null && xml.Name.LocalName == type)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
