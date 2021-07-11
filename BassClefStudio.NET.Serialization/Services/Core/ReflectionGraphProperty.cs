using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="IPropertyProvider"/>/<see cref="IPropertyConsumer"/> that gets and sets public property values using reflection.
    /// </summary>
    public class ReflectionGraphProperty : IPropertyProvider, IPropertyConsumer
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; }

        /// <inheritdoc/>
        public GraphPriority Priority { get; } = GraphPriority.BaseReflection;

        /// <summary>
        /// Creates a new <see cref="ReflectionGraphProperty"/>.
        /// </summary>
        public ReflectionGraphProperty()
        {
            SupportedTypes = new AllTypeMatch();
        }

        /// <inheritdoc/>
        public IDictionary<string, object> GetProperties(object value)
        {
            if (value == null)
            {
                throw new ArgumentException($"{nameof(ReflectionGraphProperty)} cannot handle null values.", "value");
            }
            else
            {
                var type = value.GetType();
                return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !p.GetIndexParameters().Any() && !p.CustomAttributes.Any(a => a.AttributeType == typeof(IgnorePropertyAttribute)))
                    .ToDictionary(p => p.Name, p => p.GetValue(value));
            }
        }

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph) => true;

        /// <inheritdoc/>
        public void PopulateObject(object value, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            if (value == null)
            {
                throw new ArgumentException($"{nameof(ReflectionGraphField)} cannot handle null values.", "value");
            }
            else
            {
                var type = value.GetType();
                List<string> keys = new List<string>();
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (subGraph.ContainsKey(prop.Name))
                    {
                        keys.Add(prop.Name);
                        prop.SetValue(value, subGraph[prop.Name]);
                    }
                }
                usedKeys = keys.AsEnumerable();
            }
        }
    }

    /// <summary>
    /// An <see cref="Attribute"/> that specifies to the <see cref="ReflectionGraphProperty"/> service that this property should not be included in serialization dependencies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnorePropertyAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="IgnorePropertyAttribute"/>.
        /// </summary>
        public IgnorePropertyAttribute()
        { }
    }
}
