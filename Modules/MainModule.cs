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
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        public static JsonSerializerOptions options = new JsonSerializerOptions
        {
            Converters =
                {
                    new SocketGuildUserJsonConverter(),
                    new ListOfSocketGuildUserJsonConverter(),
                    new IUserJsonConverter()
                },
            
        };
        #region Misc
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }
        [Command("test")]
        public async Task TestAsync()
        {
            List<SocketGuildUser> list = new List<SocketGuildUser>();
            list.Add((SocketGuildUser)Context.Message.Author);
            Plan plan = new Plan("lol", DateTime.UtcNow, DateTime.Now, list, false);
            string jsonString = JsonSerializer.Serialize(plan, options);
            Console.WriteLine(jsonString);
            Plan plan1 = JsonSerializer.Deserialize<Plan>(jsonString, options);
            Console.WriteLine($"{plan1.Participants[0].Id}, {Context.Message.Author.Id}");
        }
        [Command("force")]
        [RequireOwner()]
        public async Task ForceAsync()
        {
            List<Timetable> timetables;
            DirectoryInfo di;
            IMessageChannel channel;
            string text;
            Console.WriteLine("loop");
            timetables = new List<Timetable>();
            di = new DirectoryInfo("D:\\Data\\");
            var files = di.EnumerateFiles();
            foreach (FileInfo f in files)
            {
                timetables.Add(JsonSerializer.Deserialize<Timetable>(File.ReadAllText(f.FullName), options));
            }
            foreach (Timetable t in timetables)
            {
                foreach (Plan p in t.Plans)
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
                            foreach (IUser u in p.Participants)
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
        }
        #endregion
        #region mainCommands
        #region Plan
        [Command("plan")]
        [RequireContext(ContextType.Guild)]
        public async Task GuildPlanAsync(string date1, string time1, string date2, string time2, [Remainder] string name)
        {
            DateTime start = DateTime.Parse(date1 + " " + time1);
            DateTime end = DateTime.Parse(date2 + " " + time2);
            Plan plan = new Plan(name, start, end, new List<SocketGuildUser>() { (SocketGuildUser)Context.Message.Author }, false);
            try
            {
                string text = File.ReadAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json");
                Timetable timetable = JsonSerializer.Deserialize<Timetable>(text, options);
                timetable.AddPlan(plan);
                text = JsonSerializer.Serialize(timetable, options);
                File.WriteAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json", text);
            }
            catch (FileNotFoundException)
            {
                Timetable timetable = new Timetable(new List<Plan>(), (IUser)Context.Message.Author);
                timetable.AddPlan(plan);
                string text = JsonSerializer.Serialize(timetable, options);
                File.WriteAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json", text);
            }
            await ReplyAsync("Событие запланировано");
        }
        [Command("plan")]
        [RequireContext(ContextType.DM)]
        public async Task DMPlanAsync(string date1, string time1, string date2, string time2, [Remainder] string name)
        {
            DateTime start = DateTime.Parse(date1 + " " + time1);
            DateTime end = DateTime.Parse(date2 + " " + time2);
            Plan plan = new Plan(name, start, end, null, true);
            try
            {
                string text = File.ReadAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json");
                Timetable timetable = JsonSerializer.Deserialize<Timetable>(text, options);
                timetable.AddPlan(plan);
                text = JsonSerializer.Serialize(timetable, options);
                File.WriteAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json", text);
            }
            catch (FileNotFoundException)
            {
                Timetable timetable = new Timetable(new List<Plan>(), (IUser)Context.Message.Author);
                timetable.AddPlan(plan);
                string text = JsonSerializer.Serialize(timetable, options);
                File.WriteAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json", text);
            }
            await ReplyAsync("Событие запланировано");
        }
        #endregion
        #region delete
        [Command("delete")]
        public async Task DeleteAsync([Remainder] string name)
        {
            try
            {
                string text = File.ReadAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json");
                Timetable timetable = JsonSerializer.Deserialize<Timetable>(text, options);
                bool isSuccesfull = timetable.DeletePlan(name);
                text = JsonSerializer.Serialize(timetable, options);
                File.WriteAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json", text);
                if (isSuccesfull)
                    await ReplyAsync("Событие удалено");
                else
                    await ReplyAsync("Такого события нет");
            }
            catch (FileNotFoundException)
            {
                await ReplyAsync("У вас еще нет расписания");
            }
        }
        #endregion
        #region schedule
        [Command("schedule")]
        public async Task ScheduleAsync()
        {
            try
            {
                string text = File.ReadAllText($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json");
                Timetable timetable = JsonSerializer.Deserialize<Timetable>(text, options);
                await ReplyAsync(timetable.ToString());
            }
            catch (FileNotFoundException)
            {
                await ReplyAsync("У вас еще нет расписания");
            }
        }
        #endregion
        #region checkAndCheckWT
        [Command("checkwt")]
        [RequireContext(ContextType.Guild)]
        public async Task CheckWTAsync(SocketGuildUser user, [Remainder] string input)
        {
            try
            {
                string text = File.ReadAllText($"D:\\Data\\{((SocketGuildUser)user).Username}{((SocketGuildUser)user).Discriminator}.json");
                Timetable timetable = JsonSerializer.Deserialize<Timetable>(text, options);
                bool isBusy = timetable.Check(DateTime.Parse(input));
                if (isBusy)
                    await ReplyAsync("Пользователь занят");
                else
                    await ReplyAsync("Пользователь свободен");
            }
            catch (FileNotFoundException)
            {
                await ReplyAsync("У пользователя еще нет расписания");
            }
        }
        [Command("Check")]
        [RequireContext(ContextType.Guild)]
        public async Task CheckAsync(SocketGuildUser user)
        {
            try
            {
                string text = File.ReadAllText($"D:\\Data\\{((SocketGuildUser)user).Username}{((SocketGuildUser)user).Discriminator}.json");
                Timetable timetable = JsonSerializer.Deserialize<Timetable>(text, options);
                bool isBusy = timetable.Check(DateTime.Now);
                if (isBusy)
                    await ReplyAsync("Пользователь занят");
                else
                    await ReplyAsync("Пользователь свободен");
            }
            catch (FileNotFoundException)
            {
                await ReplyAsync("У пользователя еще нет расписания");
            }
        }
        #endregion
        #endregion
        #region debug
        [Command("flushCache")]
        public async Task FlushAsync([Remainder] string misc)
        {
            File.Delete($"D:\\Data\\{((SocketGuildUser)Context.Message.Author).Username}{((SocketGuildUser)Context.Message.Author).Discriminator}.json");
        }
        #endregion
    }
}

