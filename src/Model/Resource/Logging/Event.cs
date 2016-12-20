using Newtonsoft.Json;
using System;

using LMS.Model;
using LMS.Model.Annotation;

namespace LMS.Model.Resource.Logging
{
    public class Event : CompositeDataType
    {
        /// <summary>
        /// Gets or sets the event action.
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        [Required]
        public EventAction Action { get; set; }

        /// <summary>
        /// Gets or sets the specific date/time where the event happened.
        /// </summary>
        [JsonProperty(PropertyName = "effectiveTime")]
        [Required]
        public DateTime EffectiveTime { get; set; }

        public Event()
            : base()
        {
        }
    }
}