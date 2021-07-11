using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.Services.Core
{
    /// <summary>
    /// An <see cref="IGraphConstructor"/> that uses reflection to construct an object using a constructor with parameters.
    /// </summary>
    public class ReflectionConstructor : IGraphConstructor
    {
        /// <inheritdoc/>
        public ITypeMatch SupportedTypes { get; }

        /// <inheritdoc/>
        public GraphPriority Priority { get; }

        /// <summary>
        /// Creates a new <see cref="ReflectionConstructor"/>.
        /// </summary>
        public ReflectionConstructor()
        {
            SupportedTypes = TypeMatch.All();
            Priority = GraphPriority.Reflection;
        }

        /// <inheritdoc/>
        public bool CanHandle(Type desiredType, IDictionary<string, object> subGraph)
        {
            return subGraph.Any()
                && FindConstructor(desiredType, subGraph) != null;
        }

        /// <inheritdoc/>
        public object Construct(Type desiredType, IDictionary<string, object> subGraph, out IEnumerable<string> usedKeys)
        {
            var constructor = FindConstructor(desiredType, subGraph);
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            List<string> keys = new List<string>();
            foreach(var p in constructor.GetParameters())
            {
                parameters.Add(subGraph.First(g =>
                    p.ParameterType.IsAssignableFrom(g.Value.GetType())
                    && p.Name.Equals(g.Key, StringComparison.OrdinalIgnoreCase)));
            }
            usedKeys = parameters.Select(p => p.Key);
            return constructor.Invoke(parameters.Select(p => p.Value).ToArray());
        }

        private ConstructorInfo FindConstructor(Type desiredType, IDictionary<string, object> subGraph)
        {
            var allConstructors = desiredType.GetConstructors();
            var availableConstructors = allConstructors.Where(c =>
                c.GetParameters()
                .All(p => subGraph.Any(g =>
                    p.ParameterType.IsAssignableFrom(g.Value.GetType())
                    && p.Name.Equals(g.Key, StringComparison.OrdinalIgnoreCase))));
            return availableConstructors
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();
        }
    }
}
