using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Skynet_2._0.JsonConverters
{
    class IUserJsonConverter : JsonConverter<IUser>
    {
        public override IUser Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string jsonReturn = reader.GetString().Replace(" ", "");
            return Program.client.GetUser(Convert.ToUInt64(jsonReturn));
        }
        public override void Write(Utf8JsonWriter writer, IUser value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{Convert.ToString(value.Id)}");
        }
    }
}
