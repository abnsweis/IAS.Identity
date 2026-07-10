using IAS.Identity.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common.Converters;

public class FlexibleEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var intVal = reader.GetInt32();
            if (Enum.IsDefined(typeof(TEnum), intVal))
                return (TEnum)Enum.ToObject(typeof(TEnum), intVal);
            throw new BadRequestException("Invalid enum value", ErrorCodes.General.IN_VALID_ENUM_VALUE);
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString()!;
            if (Enum.TryParse<TEnum>(str, true, out var parsed))
                return parsed;

            // try parse integer-in-string
            if (int.TryParse(str, out var intVal2) && Enum.IsDefined(typeof(TEnum), intVal2))
                return (TEnum)Enum.ToObject(typeof(TEnum), intVal2);

            throw new BadRequestException($"Invalid string value '{str}' for enum {typeof(TEnum).Name}.", "Invalid_Enum_Valid");
        }

        throw new BadRequestException("Invalid enum value", ErrorCodes.General.IN_VALID_ENUM_VALUE);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        // write as number or string — choose string for readability
        writer.WriteStringValue(value.ToString());
    }
}