using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace LMS.Service.Security.JWT
{
    public class JsonWebTokenData
    {
        [JsonProperty(PropertyName = "iss")]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "exp")]
        public string ExpirationTime { get; set; }

        [JsonProperty(PropertyName = "sub")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "aud")]
        public string Audience { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Username { get; set; }

        public JsonWebTokenData()
        {
        }
    }
}