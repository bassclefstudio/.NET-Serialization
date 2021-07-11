using Autofac;
using BassClefStudio.NET.Serialization.Model;
using BassClefStudio.NET.Serialization.Services;
using BassClefStudio.NET.Serialization.Services.Core;
using BassClefStudio.NET.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization
{
    /// <summary>
    /// Provides extension methods for setting up an <see cref="Autofac.ContainerBuilder"/> with graph serialization capabilities.
    /// </summary>
    public static class GraphInjectionExtensions
    {
        /// <summary>
        /// Registers the <see cref="GraphSerializer"/> as the default serialization service, and additionally pulls in the <see cref="DefaultTypeConfiguration"/> for native type support (required). Combine with either custom or default <see cref="IGraphService"/>s for additional functionality.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> DI container.</param>
        public static void RegisterGraphSerializer(this ContainerBuilder builder)
        {
            builder.RegisterType<GraphSerializer>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<DefaultTypeConfiguration>()
                .SingleInstance()
                .AsImplementedInterfaces();
        }

        /// <summary>
        /// Registers all default <see cref="IGraphService"/> and <see cref="IGraphWriter"/> services to the DI configuration.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> DI container.</param>
        public static void RegisterDefaultGraphServices(this ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(GraphSerializer).Assembly)
                .AssignableTo<IGraphService>()
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(GraphSerializer).Assembly)
                .AssignableTo<IGraphWriter>()
                .AsImplementedInterfaces();
        }

        /// <summary>
        /// Registers the given type of <see cref="IGraphService"/> to the DI configuration.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IGraphService"/> to register.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> DI container.</param>
        public static void RegisterGraphService<T>(this ContainerBuilder builder) where T : IGraphService
        {
            builder.RegisterType<T>()
                .AsImplementedInterfaces();
        }

        /// <summary>
        /// Registers the given type of <see cref="IGraphWriter"/> to the DI configuration.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IGraphWriter"/> to register.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> DI container.</param>
        public static void RegisterGraphWriter<T>(this ContainerBuilder builder) where T : IGraphWriter
        {
            builder.RegisterType<T>()
                .AsImplementedInterfaces();
        }

        /// <summary>
        /// Registers the given type of <see cref="IGraphService"/> to the DI configuration.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> DI container.</param>
        /// <param name="configuration">The <see cref="ITypeConfiguration"/> instance to add to the <see cref="ContainerBuilder"/>.</param>
        public static void RegisterGraphConfiguration(this ContainerBuilder builder, ITypeConfiguration configuration)
        {
            builder.RegisterInstance(configuration)
                .AsImplementedInterfaces();
        }
    }
}
