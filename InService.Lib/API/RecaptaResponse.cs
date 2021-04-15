using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InService.Lib.API
{
    public class RecaptaResponse
    {
        public bool success { get; set; }

        public DateTime challenge_ts { get; set; }

        public string hostname { get; set; }

        [JsonProperty(PropertyName = "error-codes")]
        public List<string> error_codes { get; set; }
    }
}
