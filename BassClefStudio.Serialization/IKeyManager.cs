using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Serialization
{
    /// <summary>
    /// Represents a service which can manage, store, and retrieve <see cref="TItem"/> objects by a <see cref="string"/> key.
    /// </summary>
    /// <typeparam name="TItem">The type of objects stored in the manager.</typeparam>
    public interface IKeyManager<TItem>
    {
        void Add(TItem item);

        TItem this[string id] { get; }
    }

    /// <summary>
    /// Represents a service which can manage, store, and retrieve <see cref="TItem"/> objects by a <see cref="string"/> key, and which can serialize and deserialize the collection to an object of type <see cref="TStore"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of objects stored in the manager.</typeparam>
    /// <typeparam name="TStore">The type of object the collection can be serialized to and deserialized from.</typeparam>
    public interface IKeyManager<TItem, TStore> : IKeyManager<TItem, TStore, TStore>
    { }

    /// <summary>
    /// Represents a service which can manage, store, and retrieve <see cref="TItem"/> objects by a <see cref="string"/> key, and which can serialize the collection to type <see cref="TOutput"/> and deserialize the collection to type <see cref="TInput"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of objects stored in the manager.</typeparam>
    /// <typeparam name="TOutput">The type of object the collection can be serialized to.</typeparam>
    /// <typeparam name="TInput">The type of object the collection can be deserialized from.</typeparam>
    public interface IKeyManager<TItem, TInput, TOutput> : IKeyManager<TItem>
    {
        TOutput SeralizeObjects();

        void DeserializeObjects(TInput input);

        void Clear();
    }

    public static class KeyManagerExtensions
    {
        /// <summary>
        /// Adds an <see cref="IEnumerable{T}"/> of items to a <see cref="IKeyManager{TItem}"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of items to add (and the <see cref="IKeyManager{TItem}"/> being added to)</typeparam>
        /// <param name="keyManager">The <see cref="IKeyManager{TItem}"/> to add the items to.</param>
        /// <param name="items">The objects to add to the <see cref="IKeyManager{TItem}"/>.</param>
        public static void AddRange<TItem>(this IKeyManager<TItem> keyManager, IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                keyManager.Add(item);
            }
        }
    }
}
