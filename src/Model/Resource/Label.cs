using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;

namespace LMS.Model.Resource
{
    public class Label : DomainResource
    {
        /// <summary>
        /// Gets or sets the name for the label.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [Required, StringLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the legal address for the label.
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        [Required]
        public Address Address { get; set; }

        /// <summary>
        /// Gets or sets the email address for the label.
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        [Required, StringLength(128)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the telephone number for the label.
        /// </summary>
        [JsonProperty(PropertyName = "telecom")]
        [StringLength(32)]
        public string Telecom { get; set; }

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public Label()
        {
        }
    }
}