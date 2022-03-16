using Newtonsoft.Json;
using System;

namespace Utils.Interfaces
{
    public interface IBaseIdDTO : IBaseDTO
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

    }
}
