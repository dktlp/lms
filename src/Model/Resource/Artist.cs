using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;

namespace LMS.Model.Resource
{
    public class Artist : DomainResource
    {
        /// <summary>
        /// Gets or sets the stage name for the artist.
        /// </summary>
        [JsonProperty(PropertyName = "stageName")]
        [Required, StringLength(32)]
        public string StageName { get; set; }

        /// <summary>
        /// Gets or sets the human name of the artist.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [Required]
        public HumanName Name { get; set; }

        /// <summary>
        /// Gets or sets the legal address for the artist.
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        [Required]
        public Address Address { get; set; }

        /// <summary>
        /// Gets or sets the email address for the artist.
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        [Required, StringLength(128)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the telephone number for the artist.
        /// </summary>
        [JsonProperty(PropertyName = "telecom")]
        [StringLength(32)]
        public string Telecom { get; set; }

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public Artist()
        {
        }
    }
}