using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

public class StronglyTypedIdJsonConverter<T> : JsonConverter<T> where T : StronglyTypedId<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw StronglyTypedIdException.Deserialization();

        var value = reader.GetString();

        if (Guid.TryParse(value, out var guid))
        {
            var idInstance = Activator.CreateInstance(typeof(T), guid) as T;
            if (idInstance is null)
                throw StronglyTypedIdException.NullInstanceCreation();

            return idInstance;
        }

        throw StronglyTypedIdException.Deserialization();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}