using Newtonsoft.Json;
using System;

using LMS.Model;
using LMS.Model.Annotation;

namespace LMS.Model.Resource.Logging
{
    public class Source : CompositeDataType
    {
        /// <summary>
        /// Gets or sets the name (fx application name) of the source for the audit log event.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [Required, StringLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version (fx application version) of the source for the audit log event.
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        [Required, StringLength(16)]
        public string Version { get; set; }

        public Source()
            : base()
        {
        }
    }
}