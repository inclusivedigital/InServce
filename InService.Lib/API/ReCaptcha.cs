using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InService.Lib.API
{
    public class ReCaptcha
    {
        public static readonly string SiteKey = "6LegPs0ZAAAAAHDKN5W79pZFKzIOcJgDakzyUXIp";

        public static readonly string ScriptURL = $"https://www.google.com/recaptcha/api.js?render={SiteKey}";

        public static readonly string SiteVerifyURL = "https://www.google.com/recaptcha/api/siteverify";

        public string SecretKey { get; set; }

        public ReCaptcha(string secret) => SecretKey = secret;

        public async Task<Response> VerifyToken(string token, string remoteip = null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    FormUrlEncodedContent form = new FormUrlEncodedContent(new Request(SecretKey, token, remoteip).KeyValuePairs);
                    var response = await client.PostAsync(SiteVerifyURL, form);
                    if (response.IsSuccessStatusCode)
                    {
                        var respmsg = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<Response>(respmsg);
                    }
                }
                catch { }
                return null;
            }
        }

        public class Request
        {
            public string secret { get; set; }
            public string response { get; set; }
            public string remoteip { get; set; }

            public Request(string Secret, string Response, string RemoteIP)
            {
                secret = Secret;
                response = Response;
                remoteip = RemoteIP;
            }

            public List<KeyValuePair<string, string>> KeyValuePairs => new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(nameof(secret), secret),
                new KeyValuePair<string, string>(nameof(response), response),
                new KeyValuePair<string, string>(nameof(remoteip), remoteip),
            };
        }

        public class Response
        {
            public bool success { get; set; }

            public DateTime challenge_ts { get; set; }

            public string hostname { get; set; }

            [JsonProperty(PropertyName = "error-codes")]
            public List<string> error_codes { get; set; }
        }
    }

}