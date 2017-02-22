using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace LMS.Service.Security.JWT
{
    public static class JsonWebTokenSerializer
    {
        private const string JWT_TOKEN_PRIVATEKEY = "labeleaze_com_lplabelgroup";

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static Dictionary<string, Func<byte[], byte[], byte[]>> HashAlgorithms { get; set; }

        static JsonWebTokenSerializer()
        {
            HashAlgorithms = new Dictionary<string, Func<byte[], byte[], byte[]>>
            {
                { "RS256", (key, value) => { using (var sha = new HMACSHA256(key)) { return sha.ComputeHash(value); } } },
                { "HS384", (key, value) => { using (var sha = new HMACSHA384(key)) { return sha.ComputeHash(value); } } },
                { "HS512", (key, value) => { using (var sha = new HMACSHA512(key)) { return sha.ComputeHash(value); } } }
            };
        }

        public static string Encode(JsonWebToken token)
        {
            List<string> segments = new List<string>();

            byte[] header = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(token.Header, Formatting.None));
            byte[] payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(token.Data, Formatting.None));

            segments.Add(Base64UrlEncode(header));
            segments.Add(Base64UrlEncode(payload));

            byte[] unsigned = Encoding.UTF8.GetBytes(string.Join(".", segments.ToArray()));
            byte[] signature = HashAlgorithms[token.Header.Algorithm](Encoding.UTF8.GetBytes(JWT_TOKEN_PRIVATEKEY), unsigned);

            segments.Add(Base64UrlEncode(signature));

            return string.Join(".", segments.ToArray());
        }
        
        public static JsonWebToken Decode(string encodedToken, bool verify = true)
        {
            string[] parts = encodedToken.Split('.');
            if (parts == null || parts.Length != 3)
                return null;

            string header = parts[0];
            string payload = parts[1];
            byte[] crypto = Base64UrlDecode(parts[2]);

            JsonWebToken token = new JsonWebToken()
            {
                Header = JsonConvert.DeserializeObject<JsonWebTokenHeader>(Encoding.UTF8.GetString(Base64UrlDecode(header))),
                Data = JsonConvert.DeserializeObject<JsonWebTokenData>(Encoding.UTF8.GetString(Base64UrlDecode(payload)))
            };

            if (verify)
            {
                byte[] unsigned = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                byte[] signature = HashAlgorithms[token.Header.Algorithm](Encoding.UTF8.GetBytes(JWT_TOKEN_PRIVATEKEY), unsigned);

                var decodedCrypto = Convert.ToBase64String(crypto);
                var decodedSignature = Convert.ToBase64String(signature);

                if (decodedCrypto != decodedSignature)
                    token.SignatureVerified = false;

                double expires = Convert.ToDouble(token.Data.ExpirationTime);
                double epoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
                if (epoch >= expires)
                    token.Expired = true;
            }

            return token;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new Exception("Invalid Json Web Token. Unable to perform BASE64 decoding.");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
    }    
}