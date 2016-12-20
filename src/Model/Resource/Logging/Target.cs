using Newtonsoft.Json;
using System;

using LMS.Model;
using LMS.Model.Annotation;
using LMS.Model.Composite;

namespace LMS.Model.Resource.Logging
{
    public class Target : CompositeDataType
    {
        /// <summary>
        /// Gets or sets the resource type which is the target for the audit log event.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        [Required, StringLength(32)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a reference to the resource which is the target for the audit log event.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        [Required]
        public Reference Resource { get; set; }

        public Target()
            : base()
        {
        }
    }
}