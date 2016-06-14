using System;
using Newtonsoft.Json;
using NuGet;

namespace WebView.DataUpdater
{
    /// <summary>
    /// Converts a NuGet.SemanticVersion to and from a string (e.g. "1.2.3-alpha").
    /// </summary>
    public class SemanticVersionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                if (!(value is SemanticVersion))
                {
                    throw new JsonSerializationException("Expected SemanticVersion object value");
                }
                writer.WriteValue(value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonSerializationException(
                    $"Unexpected token or value when parsing version. Token: {reader.TokenType}, Value: {reader.Value}");
            }
            try
            {
                return new SemanticVersion((string)reader.Value);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException($"Error parsing SemanticVersion string: {reader.Value}", ex);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SemanticVersion);
        }
    }
}