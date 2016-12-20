using System;
using System.Collections.Generic;
using System.Configuration;

namespace LMS.Configuration
{
    public class ConfigurationSectionHandler : ConfigurationSection
    {
        public ConfigurationSectionHandler()
            : base()
        {
        }

        [ConfigurationProperty("database")]
        public DatabaseConfiguration Database
        {
            get
            {
                return (DatabaseConfiguration)base["database"] ?? new DatabaseConfiguration();
            }
        }

        [ConfigurationProperty("httproute")]
        public HttpRouteConfiguration HttpRoute
        {
            get
            {
                return (HttpRouteConfiguration)base["httproute"] ?? new HttpRouteConfiguration();
            }
        }
    }
}