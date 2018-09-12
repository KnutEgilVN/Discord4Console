using Discord4Console.Controllers;
using Discord4Console.Models.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord4Console.Models
{
    class DiscordClient
    {
        public bool IsLoggedIn { get; set; }
        public Discord Discord { get; set; }

        public DiscordUser User { get; set; }
        public List<DiscordUserGuild> UserGuilds { get; set; }
        public List<DiscordGuild> Guilds { get; set; }

        public DiscordClient(string username, string password)
        {
            Discord = new Discord();
            new Thread(async () => 
            {
                IsLoggedIn = await Discord.Login(username, password);
                Init();
            }).Start();
        }

        public async void Init()
        {
            User = new DiscordUser();
            UserGuilds = new List<DiscordUserGuild>();
            Guilds = new List<DiscordGuild>();

            if (IsLoggedIn)
            {
                User = await Discord.GetUser();
                UserGuilds = await Discord.GetUserGuilds();
                //Guilds = await Discord.GetGuilds();
                DiscordGuild guild = Discord.Deserialize<DiscordGuild>(await Discord.Client.Get(DiscordURL.GetGuildURL(UserGuilds[0])));

                Console.WriteLine(await Discord.Client.Get(DiscordURL.GetGuildURL(UserGuilds[0])));
                Console.WriteLine("FINISHED");
            }
        }
    }
}
