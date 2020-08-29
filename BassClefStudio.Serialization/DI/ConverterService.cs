using Autofac;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.Serialization.DI
{
    /// <summary>
    /// Represents a service that can serialize and deserialize objects of type <see cref="TItem"/> using DI with an Autofac <see cref="IContainer"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of items to serialize.</typeparam>
    /// <typeparam name="TInput">The input type for deserialization (<see cref="ReadItem(TInput)"/>).</typeparam>
    /// <typeparam name="TOutput">The output type for serialization (<see cref="WriteItem(TItem)"/>).</typeparam>
    public abstract class ConverterService<TItem, TInput, TOutput> : IDisposable
    {
        /// <summary>
        /// The Autofac container containing all required <see cref="IConverter{TFrom, TTo}"/>s and <see cref="IKeyManager{TItem}"/>s.
        /// </summary>
        public IContainer ConverterContainer { get; }

        /// <summary>
        /// Creates a new <see cref="ConverterService{TItem, TInput, TOutput}"/> using <see cref="Autofac.Module"/>s from the given <see cref="Assembly"/> objects.
        /// </summary>
        /// <param name="assemblies">The assemblies to register types from.</param>
        public ConverterService(params Assembly[] assemblies)
        {
            if (ConverterContainer == null)
            {
                var builder = new ContainerBuilder();
                ConverterModule converterModule = new ConverterModule(assemblies);
                builder.RegisterModule(converterModule);
                ConverterContainer = builder.Build();
            }
        }

        /// <summary>
        /// Serializes the <see cref="TItem"/> into a <see cref="TOutput"/>.
        /// </summary>
        /// <param name="item">The item to serialize.</param>
        public abstract TOutput WriteItem(TItem item);
        
        /// <summary>
        /// Desrializes the <see cref="TInput"/> into a <see cref="TItem"/>.
        /// </summary>
        /// <param name="item">The item to serialize.</param>
        public abstract TItem ReadItem(TInput input);

        /// <inheritdoc/>
        public void Dispose() => ConverterContainer.Dispose();
    }
}
