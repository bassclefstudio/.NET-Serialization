using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BassClefStudio.Serialization.Graphs
{
    /// <summary>
    /// A link referring to the object contained in a <see cref="Node"/>.
    /// </summary>
    [JsonObject(Title = "Link")]
    public struct NodeLink
    {
        /// <summary>
        /// The <see cref="int"/> ID of the <see cref="Node"/>.
        /// </summary>
        [JsonProperty]
        public int Id { get; set; }

        /// <summary>
        /// Creates a new <see cref="NodeLink"/>.
        /// </summary>
        /// <param name="id">The <see cref="int"/> ID of the <see cref="Node"/>.</param>
        public NodeLink(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Returns the <see cref="NodeLink"/> representing the given <see cref="int"/> ID.
        /// </summary>
        /// <param name="id">The <see cref="int"/> ID of the <see cref="Node"/>.</param>
        public static implicit operator NodeLink(int id)
        {
            return new NodeLink(id);
        }
    }
}
