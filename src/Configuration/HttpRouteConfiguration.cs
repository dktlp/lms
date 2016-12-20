using System;
using System.Configuration;

namespace LMS.Configuration
{
    public class HttpRouteConfiguration : ConfigurationElement
    {
        public HttpRouteConfiguration()
            : base()
        {
        }

        [ConfigurationProperty("prefix")]
        public string Prefix
        {
            get
            {
                return (string)this["prefix"];
            }
        }
    }
}