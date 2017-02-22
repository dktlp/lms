using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Service.Security.JWT
{
    public class JsonWebToken
    {
        public JsonWebTokenHeader Header { get; set; }
        public JsonWebTokenData Data { get; set; }

        public bool SignatureVerified { get; set; }
        public bool Expired { get; set; }

        public JsonWebToken()
        {
            Header = new JsonWebTokenHeader();
            Data = new JsonWebTokenData();
            SignatureVerified = true;
            Expired = false;
        }
    }
}