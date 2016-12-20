using Newtonsoft.Json;

using System;
using System.Text;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;

namespace LMS.Model.Resource.Logging
{
    public class AuditEvent : DomainResource
    {
        /// <summary>
        /// Gets or sets the target of the audit event.
        /// </summary>
        [JsonProperty(PropertyName = "target")]
        [Required]
        public Target Target { get; set; }

        /// <summary>
        /// Gets or sets the source of the audit event.
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        [Required]
        public Source Source { get; set; }

        /// <summary>
        /// Gets or sets the actual details of the event.
        /// </summary>
        [JsonProperty(PropertyName = "event")]
        [Required]
        public Event Event { get; set; }

        /// <summary>
        /// Gets or sets the user who performed the event.
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        [Required]
        public User User { get; set; }

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public AuditEvent()
        {
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            if (Target != null)
                s.Append(String.Format("Target:[{0};{1}] ", Target.Type ?? "", Target.Resource.Uri ?? ""));
            if (Source != null)
                s.Append(String.Format("Source:[{0};{1}] ", Source.Name ?? "", Source.Version ?? ""));
            if (User != null)
                s.Append(String.Format("User:[{0}] ", User.Username ?? ""));
            if (Event != null)
                s.Append(String.Format("Event:[{0}]", Event.Action.ToString() ?? ""));

            return s.ToString();
        }
    }
}