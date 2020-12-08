using JsonKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.NET.Serialization.Graphs
{
    /// <summary>
    /// Represents a <see cref="Node"/> for a given <see cref="object"/> that has a custom-generated <see cref="string"/> serialization built for it. Note that <see cref="CustomValueNode"/> does not support native <see cref="Graph"/> features such as reference handling.
    /// </summary>
    [JsonKnownThisType("custom")]
    public class CustomValueNode : Node
    {
        /// <summary>
        /// The value of this <see cref="CustomValueNode"/>'s object, as its <see cref="string"/> representation.
        /// </summary>
        [JsonProperty("Value")]
        public string ValueString { get; set; }

        /// <summary>
        /// Creates a new <see cref="CustomValueNode"/>.
        /// </summary>
        /// <param name="myLink">The <see cref="NodeLink"/> that can be used for referring to this <see cref="Node"/> instance.</param>
        /// <param name="basedOn">The <see cref="object"/> that this <see cref="Node"/> represents.</param>
        public CustomValueNode(NodeLink myLink, object basedOn) : base(myLink, basedOn)
        {
        }
    }
}
