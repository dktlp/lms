using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace LMS.Service.Security.JWT
{
    public class JsonWebTokenHeader
    {
        [JsonProperty(PropertyName = "alg")]
        public string Algorithm { get; set; }

        [JsonProperty(PropertyName = "typ")]
        public string Type { get; set; }

        public JsonWebTokenHeader()
        {
            Algorithm = "RS256";
            Type = "JWT";
        }
    }
}