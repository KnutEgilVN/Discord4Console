using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using Discord4Console.Controllers;
using Discord4Console.Models.JSON;

namespace Discord4Console.Models
{
    class NetClient
    {
        public static bool IsDebugging { get; set; }
        private static Dictionary<string, string> Headers { get; set; }
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/0.0.301 Chrome/56.0.2924.87 Discord/1.6.15 Safari/537.36";
        private const string ContentType = "application/json";

        public NetClient()
        {
            Headers = new Dictionary<string, string>();
        }

        public void AddHeader(string name, string value)
        {
            Headers.Add(name, value);
        }
        
        public async Task<string> Get(string url)
        {
            string response = "";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.UserAgent = UserAgent;
            webRequest.ContentType = ContentType;

            Headers.Keys.ToList().ForEach(k =>
                webRequest.Headers.Add(k, Headers[k]));
            
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    response = await reader.ReadToEndAsync();
                
                return response;
            }
            catch (WebException we)
            {
                DiscordRateLimit rateLimit;

                using (StreamReader sr = new StreamReader(we.Response.GetResponseStream()))
                    rateLimit = Discord.Deserialize<DiscordRateLimit>(await sr.ReadToEndAsync());

                if (IsDebugging)
                    Console.WriteLine($"Wait for {rateLimit.retry_after}ms\n{rateLimit.message}");
                Thread.Sleep(rateLimit.retry_after);

                return await Get(url);
            }
        }
        public async Task<string> Post(string url, string data)
        {
            string response = "";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.UserAgent = UserAgent;
            webRequest.ContentType = ContentType;

            Headers.Keys.ToList().ForEach(k =>
                webRequest.Headers.Add(k, Headers[k]));

            try
            {
                using (StreamWriter writer = new StreamWriter(await webRequest.GetRequestStreamAsync()))
                    writer.WriteLine(data);

                HttpWebResponse webResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                Console.WriteLine(webResponse.StatusCode);

                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    response = await reader.ReadToEndAsync();

                return response;
            }
            catch (WebException we)
            {
                DiscordRateLimit rateLimit;

                using (StreamReader sr = new StreamReader(we.Response.GetResponseStream()))
                    rateLimit = Discord.Deserialize<DiscordRateLimit>(await sr.ReadToEndAsync());

                if (IsDebugging)
                    Console.WriteLine($"Wait for {rateLimit.retry_after}ms\n{rateLimit.message}");
                Thread.Sleep(rateLimit.retry_after);

                return await Post(url, data);
            }
        }
        public async Task<string> Put(string url, string data)
        {
            string response = "";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "PUT";
            webRequest.UserAgent = UserAgent;
            webRequest.ContentType = ContentType;

            Headers.Keys.ToList().ForEach(k =>
                webRequest.Headers.Add(k, Headers[k]));

            try
            {
                using (StreamWriter writer = new StreamWriter(await webRequest.GetRequestStreamAsync()))
                    writer.WriteLine(data);

                HttpWebResponse webResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                Console.WriteLine(webResponse.StatusCode);

                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    response = await reader.ReadToEndAsync();

                return response;
            }
            catch (WebException we)
            {
                DiscordRateLimit rateLimit;

                using (StreamReader sr = new StreamReader(we.Response.GetResponseStream()))
                    rateLimit = Discord.Deserialize<DiscordRateLimit>(await sr.ReadToEndAsync());

                if (IsDebugging)
                    Console.WriteLine($"Wait for {rateLimit.retry_after}ms\n{rateLimit.message}");
                Thread.Sleep(rateLimit.retry_after);

                return await Put(url, data);
            }
        }
    }
}
