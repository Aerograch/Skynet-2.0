using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Skynet_2._0.Services;
using Skynet_2._0.Modules;

namespace Skynet_2._0
{
    class Program
    {
        public static DiscordSocketClient client;
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                client.ReactionAdded += ReactionsModule.ReactionsHandlingAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, "OTEyNzU3ODc2ODM2MTQzMTM0.YZ0mBA.DCvnZdViBOXv4LDk72_kmzstROM");
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
