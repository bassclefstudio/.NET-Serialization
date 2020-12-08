using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.NET.Serialization.Graphs
{
    /// <summary>
    /// Contains information about how the <see cref="Graph"/> should build and read <see cref="object"/>s.
    /// </summary>
    public class GraphBehaviourInfo
    {
        /// <summary>
        /// The <see cref="TypeGroup"/> defining all applicable object <see cref="Type"/>s.
        /// </summary>
        public TypeGroup ApplicableTypes { get; }

        /// <summary>
        /// <see cref="GraphBehaviour"/> flags indicating the desired changes in serialiaztion/deserialization behaviour.
        /// </summary>
        public GraphBehaviour Behaviours { get; }

        /// <summary>
        /// Creates a new <see cref="GraphBehaviourInfo"/> definition.
        /// </summary>
        /// <param name="types">The <see cref="TypeGroup"/> defining all applicable object <see cref="Type"/>s.</param>
        /// <param name="behaviours"><see cref="GraphBehaviour"/> flags indicating the desired changes in serialiaztion/deserialization behaviour.</param>
        public GraphBehaviourInfo(TypeGroup types, GraphBehaviour behaviours)
        {
            ApplicableTypes = types;
            Behaviours = behaviours;
        }
    }

    /// <summary>
    /// An <see cref="Enum"/> with flags defining behaviours the <see cref="Graph"/> can have when managing certain <see cref="object"/>s.
    /// </summary>
    [Flags]
    public enum GraphBehaviour
    { 
        /// <summary>
        /// No behaviors have been implemented.
        /// </summary>
        None = 0,

        /// <summary>
        /// Serialize all public fields and properties (usual: just public properties).
        /// </summary>
        IncludeFields = 1 << 0,

        /// <summary>
        /// When deserializing, attempt to set field values as well as property values.
        /// </summary>
        SetFields = 1 << 1,

        /// <summary>
        /// Additionally serialize properties (or fields, if set) that are read-only.
        /// </summary>
        ReadOnly = 1 << 2,

        /// <summary>
        /// Attempts to create objects in deserialization of known types using non-default constructors (parameters resolved by name and type).
        /// </summary>
        ParameterConstructor = 1 << 3
    }

    /// <summary>
    /// Extension methods for the <see cref="GraphBehaviourInfo"/> class.
    /// </summary>
    public static class GraphBehaviorExtensions
    {
        /// <summary>
        /// Gets the collection of <see cref="GraphBehaviour"/>s that are applicable to the current <see cref="Type"/> from a collection of <see cref="GraphBehaviourInfo"/>s.
        /// </summary>
        /// <param name="behaviours">The collection of <see cref="GraphBehaviourInfo"/>s.</param>
        /// <param name="type">The desired <see cref="Type"/>.</param>
        /// <returns>A <see cref="GraphBehaviour"/> enum with all applicable flags set.</returns>
        public static GraphBehaviour GetBehaviours(this IEnumerable<GraphBehaviourInfo> behaviours, Type type)
        {
            var allFlags = behaviours.Where(b => b.ApplicableTypes.IsMember(type)).Select(b => b.Behaviours);
            GraphBehaviour flags = GraphBehaviour.None;
            foreach (var f in allFlags)
            {
                flags |= f;
            }
            return flags;
        }
    }
}
