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
    class SocketGuildUserJsonConverter : JsonConverter<SocketGuildUser>
    {
        public override SocketGuildUser Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string[] jsonReturn = reader.GetString().Replace(" ", "").Split('|');
            return GetGuildAsync(Convert.ToUInt64(jsonReturn[0])).GetAwaiter().GetResult().GetUser(Convert.ToUInt64(jsonReturn[1]));
        }
        public override void Write(Utf8JsonWriter writer, SocketGuildUser value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{Convert.ToString(value.Guild.Id)}|{Convert.ToString(value.Id)}");
        }
        private async Task<SocketGuild> GetGuildAsync(ulong id)
        {
            return Program.client.GetGuild(id);
        }
    }
}
