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
    class ListOfSocketGuildUserJsonConverter : JsonConverter<List<SocketGuildUser>>
    {
        public override List<SocketGuildUser> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string initialStr = reader.GetString();
            int len = initialStr.Replace(" ", "").Split(',').Length;
            string[][] jsonReturn = new string[len][];
            int count1 = 0;
            int count2 = 0;
            foreach (string s in initialStr.Replace(" ", "").Split(','))
            {
                jsonReturn[count1] = new string[2];
                foreach (string c in s.Split('|'))
                {
                    jsonReturn[count1][count2] = c;
                    count2 += 1;
                }
                count1 += 1;
                count2 = 0;
            }
            List<SocketGuildUser> output = new List<SocketGuildUser>();
            SocketGuildUser bufferUser;
            SocketGuild bufferGuild;
            foreach (string[] s in jsonReturn)
            {
                bufferGuild = Program.client.GetGuild(Convert.ToUInt64(s[0]));
                
                bufferUser = bufferGuild.GetUser(Convert.ToUInt64(s[1]));
                output.Add(bufferUser);
            }
            return output;
        }
        public override void Write(Utf8JsonWriter writer, List<SocketGuildUser> value, JsonSerializerOptions options)
        {
            string output = "";
            for (int i = 0; i < value.Count; i++)
            {
                if (i == value.Count - 1)
                {
                    output += $"{value[i].Guild.Id}|{value[i].Id}";
                    continue;
                }
                output += $"{value[i].Guild.Id}|{value[i].Id},";
            }
            writer.WriteStringValue(output);
        }
    }
}
