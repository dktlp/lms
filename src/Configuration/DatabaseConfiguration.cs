using System;
using System.Configuration;

namespace LMS.Configuration
{
    public class DatabaseConfiguration : ConfigurationElement
    {
        public DatabaseConfiguration()
            : base()
        {
        }

        public override string ToString()
        {
            return String.Format("server={0};database={1};uid={2};pwd={3}", Server, Database, User, Password);
        }
        
        [ConfigurationProperty("server")]
        public string Server
        {
            get
            {
                return (string)base["server"];
            }
        }

        [ConfigurationProperty("database")]
        public string Database
        {
            get
            {
                return (string)base["database"];
            }
        }

        [ConfigurationProperty("uid")]
        public string User
        {
            get
            {
                return (string)base["uid"];
            }
        }

        [ConfigurationProperty("pwd")]
        public string Password
        {
            get
            {
                return (string)base["pwd"];
            }
        }
    }
}