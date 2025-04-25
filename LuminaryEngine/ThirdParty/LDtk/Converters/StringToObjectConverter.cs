using LuminaryEngine.ThirdParty.LDtk.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LuminaryEngine.ThirdParty.LDtk.Converters;

public class StringToObjectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jToken = JToken.Load(reader);

        if (jToken.Type == JTokenType.String)
        {
            jToken = JToken.Parse((string)jToken);
        }

        return jToken.ToObject(objectType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanWrite => false;
}