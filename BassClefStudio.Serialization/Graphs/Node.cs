using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BassClefStudio.Serialization.Graphs
{
    /// <summary>
    /// Represents an <see cref="object"/> in a data graph.
    /// </summary>
    [JsonObject(Title = "Node")]
    public class Node
    {
        /// <summary>
        /// The <see cref="NodeLink"/> that can be used for referring to this <see cref="Node"/> instance.
        /// </summary>
        [JsonProperty(PropertyName = "Link")]
        public NodeLink MyLink { get; set; }

        /// <summary>
        /// The <see cref="object"/> that this <see cref="Node"/> represents.
        /// </summary>
        [JsonIgnore]
        public object BasedOn { get; set; }

        /// <summary>
        /// The name of the type of this <see cref="Node"/>.
        /// </summary>
        [JsonProperty(PropertyName = "TypeName")]
        public string TypeName { get; set; }

        /// <summary>
        /// A collection of properties with <see cref="string"/> keys - these are either basic (value) objects, or <see cref="NodeLink"/>(s) to other reference <see cref="Node"/>s.
        /// </summary>
        [JsonProperty]
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Creates a new <see cref="Node"/>.
        /// </summary>
        /// <param name="myLink">The <see cref="NodeLink"/> that can be used for referring to this <see cref="Node"/> instance.</param>
        /// <param name="basedOn">The <see cref="object"/> that this <see cref="Node"/> represents.</param>
        public Node(NodeLink myLink, object basedOn)
        {
            MyLink = myLink;
            BasedOn = basedOn;
            Properties = new Dictionary<string, object>();
        }
    }

    public class NodePropertyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
