using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Utils.Helpers
{
    public sealed class StringTrimConverter : JsonConverter
    {

        private readonly JsonSerializer Serializer;

        public StringTrimConverter(JsonSerializer serializer)
        {
            Serializer = serializer;
        }
  

        public override sealed bool CanRead => true;

        public override sealed bool CanWrite => false; 

        public override sealed bool CanConvert(Type objectType)
        {
            return true;
        }

        public override sealed object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken item = JToken.Load(reader);
            Trim(item);
            return item.ToObject(objectType, Serializer);
        }

        public override sealed void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private void Trim(JToken item)
        {
            if (item.HasValues)
            {
                IJEnumerable<JToken> children = item.Values();

                foreach (JToken child in children)
                {
                    JValue finalLoaf = (child as JValue);

                    if (finalLoaf != null)
                    {
                        object value = finalLoaf.Value;

                        if (value is string)
                        {
                            string stringfiedValue = (string)value;

                            if (!string.IsNullOrEmpty(stringfiedValue))
                            {
                                finalLoaf.Value = stringfiedValue.Trim();
                            }
                        }
                    }
                    else
                    {
                        Trim(child);
                    }
                }
            }
        }
    }
}
