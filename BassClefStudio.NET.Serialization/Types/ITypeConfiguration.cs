using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Serialization.Types
{
    /// <summary>
    /// Represents the configuration of a given <see cref="ISerializationService"/>'s trusted and native type matching expressions.
    /// </summary>
    public interface ITypeConfiguration
    {
        /// <summary>
        /// An <see cref="ITypeMatch"/> expression that resolves any types that can be included in the serialized output without additional dependency management (e.g. <see cref="string"/>).
        /// </summary>
        ITypeMatch NativeTypes { get; }

        /// <summary>
        /// An <see cref="ITypeMatch"/> for all trusted, non-native data types.
        /// </summary>
        ITypeMatch TrustedTypes { get; }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="ITypeConfiguration"/> interface.
    /// </summary>
    public static class TypeConfigurationExtensions
    {
        /// <summary>
        /// Gets an <see cref="ITypeMatch"/> for both <see cref="ITypeConfiguration.NativeTypes"/> and <see cref="ITypeConfiguration.TrustedTypes"/> of the given configuration.
        /// </summary>
        /// <param name="configuration">The <see cref="ITypeConfiguration"/>s to manage.</param>
        public static ITypeMatch MatchAny(this IEnumerable<ITypeConfiguration> configuration)
        {
            return configuration.SelectMany(c => new ITypeMatch[] { c.NativeTypes, c.TrustedTypes }).Concat();
        }

        /// <summary>
        /// Gets an <see cref="ITypeMatch"/> for both <see cref="ITypeConfiguration.NativeTypes"/> and <see cref="ITypeConfiguration.TrustedTypes"/> of the given configuration.
        /// </summary>
        /// <param name="configuration">The <see cref="ITypeConfiguration"/>s to manage.</param>
        public static ITypeMatch MatchNative(this IEnumerable<ITypeConfiguration> configuration)
        {
            return configuration.Select(c => c.NativeTypes).Concat();
        }

        /// <summary>
        /// Gets an <see cref="ITypeMatch"/> for both <see cref="ITypeConfiguration.NativeTypes"/> and <see cref="ITypeConfiguration.TrustedTypes"/> of the given configuration.
        /// </summary>
        /// <param name="configuration">The <see cref="ITypeConfiguration"/>s to manage.</param>
        public static ITypeMatch MatchTrusted(this IEnumerable<ITypeConfiguration> configuration)
        {
            return configuration.Select(c => c.TrustedTypes).Concat();
        }
    }
}
