using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.NET.Serialization.Graphs
{
    /// <summary>
    /// Represents a group of <see cref="Assembly"/> and <see cref="Type"/> instances that represent a group of object types.
    /// </summary>
    public class TypeGroup
    {
        /// <summary>
        /// A collection of known <see cref="Assembly"/> references (all types in the assembly are members of the <see cref="TypeGroup"/>).
        /// </summary>
        public List<Assembly> KnownAssemblies { get; }

        /// <summary>
        /// A collection of known <see cref="Type"/> references (all types are members of the <see cref="TypeGroup"/>).
        /// </summary>
        public List<Type> KnownTypes { get; }

        /// <summary>
        /// Creates a new empty <see cref="TypeGroup"/>.
        /// </summary>
        public TypeGroup()
        {
            KnownAssemblies = new List<Assembly>();
            KnownTypes = new List<Type>();
        }

        /// <summary>
        /// Creates a new empty <see cref="TypeGroup"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        public TypeGroup(params Assembly[] knownAssemblies) : this()
        {
            KnownAssemblies.AddRange(knownAssemblies);
        }

        /// <summary>
        /// Creates a new empty <see cref="TypeGroup"/>.
        /// </summary>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public TypeGroup(params Type[] knownTypes) : this()
        {
            KnownTypes.AddRange(knownTypes);
        }

        /// <summary>
        /// Creates a new empty <see cref="TypeGroup"/>.
        /// </summary>
        /// <param name="knownAssemblies">A collection of known <see cref="Assembly"/> references.</param>
        /// <param name="knownTypes">A collection of known <see cref="Type"/> references.</param>
        public TypeGroup(IEnumerable<Assembly> knownAssemblies, IEnumerable<Type> knownTypes) : this()
        {
            KnownAssemblies.AddRange(knownAssemblies);
            KnownTypes.AddRange(knownTypes);
        }


        /// <summary>
        /// Gets and verifies the <see cref="Type"/> represented by the given name.
        /// </summary>
        /// <param name="typeName">The <see cref="string"/> name of the type (see <see cref="Type.AssemblyQualifiedName"/>)</param>
        /// <returns>The member <see cref="Type"/>, if found.</returns>
        /// <exception cref="GraphTypeException">The type was not a member of this <see cref="TypeGroup"/>.</exception>
        public Type GetMember(string typeName)
        {
            var type = Type.GetType(typeName);
            VerifyType(type);
            return type;
        }

        /// <summary>
        /// Gets and verifies the <see cref="Type"/> represented by the given object.
        /// </summary>
        /// <param name="o">The given <see cref="object"/> to check.</param>
        /// <returns>The <see cref="object"/>'s <see cref="Type"/>, if found.</returns>
        /// <exception cref="GraphTypeException">The type was not a member of this <see cref="TypeGroup"/>.</exception>
        public Type GetMember(object o)
        {
            var type = o.GetType();
            VerifyType(type);
            return type;
        }

        /// <summary>
        /// Verifies the given <see cref="Type"/> to check it is a member of this <see cref="TypeGroup"/>.
        /// </summary>
        /// <exception cref="GraphTypeException">The type was not a member of this <see cref="TypeGroup"/>.</exception>
        public void VerifyType(Type type)
        {
            if (!IsMember(type))
            {
                throw new GraphTypeException($"The type {type.FullName} of this object is not trusted.");
            }
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the <see cref="Type"/> is a member of this <see cref="TypeGroup"/>.
        /// </summary>
        /// <param name="type">The given <see cref="Type"/>.</param>
        public bool IsMember(Type type)
        {
            if (KnownAssemblies.Contains(type.Assembly))
            {
                return true;
            }
            else if (KnownTypes.Contains(type))
            {
                return true;
            }

            var trustedTypes = KnownAssemblies.SelectMany(a => a.GetTypes()).Concat(KnownTypes);
            if (type.IsGenericType && trustedTypes.Contains(type.GetGenericTypeDefinition()))
            {
                return true;
            }

            return false;
        }
    }
}
