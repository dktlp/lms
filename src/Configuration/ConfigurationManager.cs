using System;

namespace LMS.Configuration
{
    public static class ConfigurationManager
    {
        public static DatabaseConfiguration Database
        {
            get
            {
                return (DatabaseConfiguration)((ConfigurationSectionHandler)System.Configuration.ConfigurationManager.GetSection("lms")).Database;
            }
        }

        public static HttpRouteConfiguration HttpRoute
        {
            get
            {
                return (HttpRouteConfiguration)((ConfigurationSectionHandler)System.Configuration.ConfigurationManager.GetSection("lms")).HttpRoute;
            }
        }
    }
}