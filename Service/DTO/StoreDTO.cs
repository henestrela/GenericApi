using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Interfaces;

namespace Service.DTO
{
    public class StoreDTO: IBaseIdDTO
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }
    }
}
