using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Annotation;

namespace LMS.Model.Composite
{
    /// <summary>
    /// This composite datatype represents a time period defined by a start and end date/time. 
    /// </summary>
    public class Period : CompositeDataType
    {
        /// <summary>
        /// Gets or sets the start date/time.
        /// </summary>
        [JsonProperty(PropertyName = "start")]
        [Required]
        public DateTime? Start { get; set; }

        /// <summary>
        /// Gets or sets the end date/time.
        /// </summary>
        [JsonProperty(PropertyName = "end")]
        [Required]
        public DateTime? End { get; set; }

        /// <summary>
        /// Create instance of class.
        /// </summary>
        public Period()
            : base()
        {
        }
    }
}
