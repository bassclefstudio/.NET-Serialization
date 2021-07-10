using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="IPropertyProvider"/>/<see cref="IPropertyConsumer"/> that gets and sets field values using reflection.
    /// </summary>
    public class ReflectionGraphField : IPropertyProvider, IPropertyConsumer
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; }

        /// <summary>
        /// Creates a new <see cref="ReflectionGraphField"/>.
        /// </summary>
        public ReflectionGraphField()
        {
            SupportedTypes = new AllTypeMatch();
        }

        /// <inheritdoc/>
        public IDictionary<string, object> GetProperties(object value)
        {
            if (value == null)
            {
                throw new ArgumentException($"{nameof(ReflectionGraphField)} cannot handle null values.", "value");
            }
            else
            {
                var type = value.GetType();
                return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(p => p.Name, p => p.GetValue(value));
            }
        }

        /// <inheritdoc/>
        public void PopulateObject(object value, IDictionary<string, object> subGraph)
        {
            if (value == null)
            {
                throw new ArgumentException($"{nameof(ReflectionGraphField)} cannot handle null values.", "value");
            }
            else
            {
                var type = value.GetType();
                foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if(subGraph.ContainsKey(field.Name))
                    {
                        field.SetValue(value, subGraph[field.Name]);
                    }
                }
            }
        }
    }
}
