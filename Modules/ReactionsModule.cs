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
    class ReactionsModule
    {
        public static async Task ReactionsHandlingAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            
        }
    }
}
