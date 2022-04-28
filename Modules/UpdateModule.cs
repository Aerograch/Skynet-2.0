using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Skynet_2._0.Services;
using Skynet_2._0.Classes;
using Skynet_2._0.JsonConverters;

namespace Skynet_2._0.Modules
{
    class UpdateModule
    {
        public static JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters =
                {
                    new SocketGuildUserJsonConverter(),
                    new ListOfSocketGuildUserJsonConverter(),
                    new IUserJsonConverter()
                }
        };
        public static async Task NotifyLoopAsync()
        {
            await Task.Delay(20000);
            List<Timetable> timetables;
            DirectoryInfo di;
            IMessageChannel channel;
            string text;
            while (true)
            {
                Console.WriteLine("loop");
                timetables = new List<Timetable>();
                di = new DirectoryInfo("D:\\Data\\");
                var files = di.EnumerateFiles();
                foreach(FileInfo f in files)
                {
                    timetables.Add(JsonSerializer.Deserialize<Timetable>(File.ReadAllText(f.FullName), options));
                }
                foreach(Timetable t in timetables)
                {
                    foreach(Plan p in t.Plans)
                    {
                        if ((p.Start - DateTime.Now).TotalMinutes < 10 && (p.Start - DateTime.Now).TotalMinutes > 0 && !p.IsAlreadyNotified)
                        {
                            if (p.IsDM)
                            {
                                channel = await ((IUser)t.Author).GetOrCreateDMChannelAsync();
                                await channel.SendMessageAsync($"У вас запланировнанно мероприятие {p.Name} с {p.Start} по {p.Finish}");
                                p.IsAlreadyNotified = true;
                                text = JsonSerializer.Serialize(t, options);
                                File.WriteAllText($"D:\\Data\\{t.Author.Username}{t.Author.Discriminator}.json", text);
                            }
                            else
                            {
                                foreach(IUser u in p.Participants)
                                {
                                    channel = await u.GetOrCreateDMChannelAsync();
                                    await channel.SendMessageAsync($"У вас запланировнанно мероприятие {p.Name} с {p.Start} по {p.Finish}");
                                    p.IsAlreadyNotified = true;
                                    text = JsonSerializer.Serialize(t, options);
                                    File.WriteAllText($"D:\\Data\\{t.Author.Username}{t.Author.Discriminator}.json", text);
                                }
                            }
                        }
                    }
                }
                await Task.Delay(40000);
            }
        }
    }
}
