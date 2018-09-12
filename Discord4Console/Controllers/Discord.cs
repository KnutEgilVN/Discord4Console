using Discord4Console.Models;
using Discord4Console.Models.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord4Console.Controllers
{
    class Discord
    {
        public NetClient Client { get; set; }
        private static JsonSerializerSettings JSONSettings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
            }
        }

        private LoginResponse LoginResponse { get; set; }

        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, JSONSettings);
        }

        public Discord()
        {
            Client = new NetClient();
        }
        public async Task<bool> Login(string username, string password)
        {
            string loginData = JsonConvert.SerializeObject(new LoginRequest(username, password));
            LoginResponse = JsonConvert.DeserializeObject<LoginResponse>(await Client.Post(DiscordURL.LoginURL, JsonConvert.SerializeObject(new LoginRequest(username, password))));
            Client.AddHeader("authorization", LoginResponse.token);

            if (LoginResponse.token != "")
                return true;
            else
                return false;
        }

        public async Task<DiscordUser> GetUser()
        {
            return Deserialize<DiscordUser>(await Client.Get(DiscordURL.UserURL));
        }
        public async Task<List<DiscordUserGuild>> GetUserGuilds()
        {
            return Deserialize<List<DiscordUserGuild>>(await Client.Get(DiscordURL.UserGuildsURL));
        }
        public async Task<DiscordGuild> GetGuild(DiscordUserGuild userGuild)
        {
            DiscordGuild guild = Deserialize<DiscordGuild>(await Client.Get(DiscordURL.GetGuildURL(userGuild)));
            return guild;
        }
        public async Task<List<DiscordGuild>> GetGuilds()
        {
            List<DiscordUserGuild> userGuilds = Deserialize<List<DiscordUserGuild>>(await Client.Get(DiscordURL.UserGuildsURL));
            List<DiscordGuild> guilds = new List<DiscordGuild>();

            new Task(() => 
            {
                userGuilds.ForEach(async u =>
                {
                    DiscordGuild guild = Deserialize<DiscordGuild>(await Client.Get(DiscordURL.GetGuildURL(u)));
                    guilds.Add(guild);

                    Console.WriteLine(guild.name);
                });
            }).Wait();
            Console.WriteLine("DONE");
            return guilds;
        }
    }

    class DiscordURL
    {
        public const string BaseURL = "https://discordapp.com/api/v6";

        /// <summary>
        /// A POST request to this URL with a JSON Serialized /JSON/LoginRequest object 
        /// results in a JSON Serialized /JSON/LoginResponse object. (Authentication Token)
        /// </summary>
        public static string LoginURL = $"{BaseURL}/auth/login";

        /// <summary>
        /// A GET request to this URL results in a JSON Serialized /JSON/DiscordUser object
        /// </summary>
        public static string UserURL = $"{BaseURL}/users/@me";

        /// <summary>
        /// A GET request to this URL results in a JSON Serialized List(/JSON/DiscordGuild) object
        /// </summary>
        public static string UserGuildsURL = $"{BaseURL}/users/@me/guilds";
        /// <summary>
        /// A GET requests to this URL results in a JSON Serialized /JSON/DiscordGuild object
        /// </summary>
        /// <param name="userGuild"></param>
        /// <returns>API EndPoint for getting Guilds from ID</returns>
        public static string GetGuildURL(DiscordUserGuild userGuild)
        {
            return $"{BaseURL}/guilds/{userGuild.id}";
        }
        /// <summary>
        /// A GET requests to this URL results in a JSON Serialized List(/JSON/DiscordGuildChannel) object
        /// </summary>
        /// <param name="guild"></param>
        /// <returns>API EndPoint for Guild's Channels</returns>
        public static string GuildChannelsURL(DiscordGuild guild)
        {
            return $"{BaseURL}/guilds/{guild.id}/channels";
        }
        /// <summary>
        /// A GET requests to this URL results in a JSON Serialized List(/JSON/DiscordGuildChannel) object
        /// </summary>
        /// <param name="guild"></param>
        /// <returns>API EndPoint for Guild's Channels</returns>
        public static string GuildChannelsURL(string guildID)
        {
            return $"{BaseURL}/guilds/{guildID}/channels";
        }
    }
}
