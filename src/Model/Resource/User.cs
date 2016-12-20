using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;

namespace LMS.Model.Resource
{
    public class User : DomainResource
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        /// <remarks>Unique user identification used for user authenticiation.</remarks>
        [JsonProperty(PropertyName = "uid")]
        [Required, StringLength(32)]
        public string Username { get; set; }

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public User()
        {
        }
    }
}