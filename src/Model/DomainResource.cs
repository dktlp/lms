using Newtonsoft.Json;
using System;

namespace LMS.Model
{
    /// <summary>
    /// This is an abstract class from which all resources derive. It defines a superset of common properties and methods.
    /// </summary>
    public abstract class DomainResource
    {
        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        [JsonProperty(PropertyName = "resourceType")]
        public string ResourceType
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the resource.
        /// </summary>
        [JsonProperty(PropertyName = "identifier")]
        public int Id { get; set; }

        /// <summary>
        /// Constructor for the abstract class.
        /// </summary>
        public DomainResource()
        {
        }
    }
}