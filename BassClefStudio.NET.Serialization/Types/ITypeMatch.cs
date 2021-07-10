using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.Types
{
    /// <summary>
    /// Represents an object that can be used to match against one or more <see cref="Type"/>s found in serialized objects.
    /// </summary>
    public interface ITypeMatch
    {
        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the given <see cref="Type"/> is matched by this <see cref="ITypeMatch"/> expression.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to match.</param>
        /// <returns>A <see cref="bool"/> indicating whether the match succeeded.</returns>
        bool Match(Type type);
    }

    /// <summary>
    /// Provides extension methods and unified constructors for <see cref="ITypeMatch"/> expressions.
    /// </summary>
    public static class TypeMatch
    {
        /// <summary>
        /// Creates an <see cref="ITypeMatch"/> that matches objects of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type this expression matches.</typeparam>
        public static ITypeMatch Type<T>() => new BaseTypeMatch(typeof(T));

        /// <summary>
        /// Creates an <see cref="ITypeMatch"/> that matches a collection of <see cref="System.Type"/>s.
        /// </summary>
        /// <param name="types">The <see cref="System.Type"/>s this expression matches.</param>
        public static ITypeMatch Type(params Type[] types) => new BaseTypeMatch(types);

        /// <summary>
        /// (Not for use in trusted type collections!) Creates an <see cref="ITypeMatch"/> that matches any <see cref="System.Type"/>s implementing <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the interface or parent class this expression matches.</typeparam>
        public static ITypeMatch OfType<T>() => new InheritTypeMatch(typeof(T));

        /// <summary>
        /// (Not for use in trusted type collections!) Creates an <see cref="ITypeMatch"/> that matches any <see cref="System.Type"/>s implementing a given <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> of the interface or parent class this expression matches.</param>
        public static ITypeMatch OfType(Type type) => new InheritTypeMatch(type);

        /// <summary>
        /// Creates an <see cref="ITypeMatch"/> that matches a collection of <see cref="System.Reflection.Assembly"/>s.
        /// </summary>
        /// <param name="assemblies">The <see cref="System.Reflection.Assembly"/>s this expression matches.</param>
        public static ITypeMatch Assembly(params Assembly[] assemblies) => new AssemblyMatch(assemblies);

        /// <summary>
        /// Creates an <see cref="ITypeMatch"/> that matches all <see cref="System.Type"/>s.
        /// </summary>
        public static ITypeMatch All() => new AllTypeMatch();

        /// <summary>
        /// Adds two <see cref="ITypeMatch"/> expressions together.
        /// </summary>
        /// <param name="match">The existing <see cref="ITypeMatch"/> expression.</param>
        /// <param name="add">The <see cref="ITypeMatch"/> to add.</param>
        /// <returns>An <see cref="ITypeMatch"/> that matches all <see cref="System.Type"/>s that either of the component expressions match.</returns>
        public static ITypeMatch Concat(this ITypeMatch match, ITypeMatch add) => new ConcatTypeMatch(match, add);
    }
}
