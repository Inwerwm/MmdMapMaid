using System.Text.Json;
using System.Text.Json.Serialization;

namespace MmdMapMaid.Helpers;

public class NonStringKeyDictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : notnull
{
    private const string KeyPropertyName = "Key";
    private const string ValuePropertyName = "Value";

    public override Dictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        var value = new Dictionary<TKey, TValue>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return value;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            TKey? key = default;
            TValue? val = default;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case KeyPropertyName:
                        key = JsonSerializer.Deserialize<TKey>(ref reader, options);
                        break;
                    case ValuePropertyName:
                        val = JsonSerializer.Deserialize<TValue>(ref reader, options);
                        break;
                }
            }

            if (key is null)
            {
                continue;
            }

            value.Add(key, val);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var kvp in value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(KeyPropertyName);
            JsonSerializer.Serialize(writer, kvp.Key, options);
            writer.WritePropertyName(ValuePropertyName);
            JsonSerializer.Serialize(writer, kvp.Value, options);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}
