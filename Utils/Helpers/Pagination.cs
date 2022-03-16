using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Utils.Helpers
{

    [DataContract]
    [JsonObject]
    public class Pagination<T>
    {
        [JsonProperty(PropertyName = "count")]
        [DataMember]
        public long? Count { get; set; }

        [JsonProperty(PropertyName = "page")]
        [DataMember]
        public int Page { get; set; }

        [JsonProperty(PropertyName = "perPage")]
        [DataMember]
        public int PerPage { get; set; }

        [JsonProperty(PropertyName = "items")]

        [DataMember]
        public IEnumerable<dynamic> Items { get; set; }

        public Pagination()
        {
            Count = 0;
            Page = 0;
            PerPage = 0;
            Items = null;
        }


    }
}
