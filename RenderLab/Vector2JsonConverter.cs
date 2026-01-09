using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RenderLab
{
    public sealed class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of object for Vector2.");

            float x = 0, y = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return new Vector2(x, y);

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected property name in Vector2 object.");

                string? propName = reader.GetString();
                reader.Read(); // move to value

                switch (propName)
                {
                    case "X":
                        x = reader.GetSingle();
                        break;
                    case "Y":
                        y = reader.GetSingle();
                        break;
                    default:
                        // Skip unknown properties safely
                        reader.Skip();
                        break;
                }
            }

            throw new JsonException("Incomplete Vector2 object.");
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteEndObject();
        }
    }
}







