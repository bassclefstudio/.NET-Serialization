using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.Types
{
    /// <summary>
    /// An <see cref="ITypeMatch"/> expression that matches all non-null <see cref="Type"/>s.
    /// </summary>
    public class AllTypeMatch : ITypeMatch
    {
        /// <inheritdoc/>
        public bool Match(Type type)
        {
            return type != null;
        }
    }

    /// <summary>
    /// An <see cref="ITypeMatch"/> expression that matches no <see cref="Type"/>s. A good default expression value.
    /// </summary>
    public class NoTypeMatch : ITypeMatch
    {
        /// <inheritdoc/>
        public bool Match(Type type)
        {
            return false;
        }
    }

    /// <summary>
    /// A basic <see cref="ITypeMatch"/> expression that matches a set of given <see cref="Type"/>s.
    /// </summary>
    public class BaseTypeMatch : ITypeMatch
    {
        /// <summary>
        /// The collection of <see cref="Type"/> instances or generic declarations to include in the <see cref="ITypeMatch"/> expression.
        /// </summary>
        public Type[] RegisteredTypes { get; }

        /// <summary>
        /// Creates a new <see cref="TypeMatch"/>.
        /// </summary>
        /// <param name="registeredTypes">The collection of <see cref="Type"/> instances or generic declarations to include in the <see cref="ITypeMatch"/> expression.</param>
        public BaseTypeMatch(params Type[] registeredTypes)
        {
            RegisteredTypes = registeredTypes;
        }

        /// <inheritdoc/>
        public bool Match(Type type)
        {
            return RegisteredTypes.Contains(type)
                || (type.IsGenericType && RegisteredTypes.Contains(type.GetGenericTypeDefinition()));
        }
    }

    /// <summary>
    /// An (untrusted) <see cref="ITypeMatch"/> expression for all types that inherit a given interface.
    /// </summary>
    public class InheritTypeMatch : ITypeMatch
    {
        /// <summary>
        /// The <see cref="Type"/> of interface implementation this <see cref="ITypeMatch"/> expression matches.
        /// </summary>
        public Type InheritType { get; }

        /// <summary>
        /// Creates a new <see cref="TypeMatch"/>.
        /// </summary>
        /// <param name="inheritType">The <see cref="Type"/> of interface implementation this <see cref="ITypeMatch"/> expression matches.</param>
        public InheritTypeMatch(Type inheritType)
        {
            InheritType = inheritType;
        }

        /// <inheritdoc/>
        public bool Match(Type type)
        {
            return InheritType.IsAssignableFrom(type);
        }
    }

    /// <summary>
    /// A basic <see cref="ITypeMatch"/> expression that matches all types in given <see cref="Assembly"/>s.
    /// </summary>
    public class AssemblyMatch : ITypeMatch
    {
        /// <summary>
        /// The collection of <see cref="Assembly"/> definitions to include in the <see cref="ITypeMatch"/> expression.
        /// </summary>
        public Assembly[] RegisteredAssemblies { get; }

        /// <summary>
        /// Creates a new <see cref="AssemblyMatch"/>.
        /// </summary>
        /// <param name="registeredAssemblies">The collection of <see cref="Assembly"/> definitions to include in the <see cref="ITypeMatch"/> expression.</param>
        public AssemblyMatch(params Assembly[] registeredAssemblies)
        {
            RegisteredAssemblies = registeredAssemblies;
        }

        /// <inheritdoc/>
        public bool Match(Type type)
        {
            return RegisteredAssemblies.Contains(type.Assembly);
        }
    }

    /// <summary>
    /// An <see cref="ITypeMatch"/> expression that is equivalent to the logical OR of a set of child <see cref="ITypeMatch"/>es.
    /// </summary>
    public class ConcatTypeMatch : ITypeMatch
    {
        /// <summary>
        /// The collection of <see cref="ITypeMatch"/> expressions to concatenate in this <see cref="ITypeMatch"/>.
        /// </summary>
        public ITypeMatch[] Expressions { get; }

        /// <summary>
        /// Creates a new <see cref="TypeMatch"/>.
        /// </summary>
        /// <param name="expressions">The collection of <see cref="ITypeMatch"/> expressions to concatenate in this <see cref="ITypeMatch"/>.</param>
        public ConcatTypeMatch(params ITypeMatch[] expressions)
        {
            Expressions = expressions;
        }

        /// <inheritdoc/>
        public bool Match(Type type)
        {
            return Expressions.Any(e => e.Match(type));
        }
    }
}
