using Autofac;
using BassClefStudio.Serialization.Json;
using BassClefStudio.Serialization.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.Serialization.DI
{
    /// <summary>
    /// An Autofac module that loads the <see cref="IConverter{TFrom, TTo}"/> and <see cref="IKeyManager{TItem}"/> instances into an Autofac <see cref="IContainer"/> for use in serialization.
    /// </summary>
    public class ConverterModule : Autofac.Module
    {
        public Assembly[] ConverterAssemblies { get; }

        public ConverterModule(IEnumerable<Assembly> assemblies)
        {
            ConverterAssemblies = assemblies
                .Concat(
                new Assembly[] 
                {
                    ThisAssembly 
                })
                .ToArray();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ConverterAssemblies)
                .AsClosedTypesOf(typeof(IConverter<,>))
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterGeneric(typeof(JsonKeyManager<>))
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterGeneric(typeof(XmlKeyManager<>))
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }
    }
}
