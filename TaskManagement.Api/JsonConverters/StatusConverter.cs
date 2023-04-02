using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagement.Services.DataContext.Entities;

namespace TaskManagement.Api.JsonConverters
{
    public class StatusConverter : JsonConverter<Status>
    {
        public override Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && Enum.TryParse(reader.GetString(), out Status status))
            {
                return status;
            }
            else
            {
                throw new JsonException($"Invalid value for {nameof(Status)}");
            }
        }

        public override void Write(Utf8JsonWriter writer, Status value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
