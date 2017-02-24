using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Annotation;

namespace LMS.Model.Composite
{
    /// <summary>
    /// This composite datatype represents an address of a physical location. 
    /// </summary>
    /// <remarks>There is a variety of postal address formats defined around the world. This class defines a superset that is the basis for all addresses around the world.</remarks>
    public class Address : CompositeDataType
    {
        /// <summary>
        /// Gets the text representation of the address.
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text
        {
            get
            {
                string text = "";

                if (Line != null)
                {
                    foreach (string l in Line)
                        text += l + Environment.NewLine;
                }

                if (District != null)
                    text += District + Environment.NewLine;
                if (PostalCode != null && City != null)
                    text += PostalCode + " " + City + Environment.NewLine;
                
                if (State != null)
                    text += State + Environment.NewLine;
                if (Country != null)
                    text += Country;

                return text;
            }
        }

        /// <summary>
        /// Gets or sets the values of address line, fx street name, number, direction, etc.
        /// </summary>
        [JsonProperty(PropertyName = "line")]
        [ArrayStringLength(96)]
        public string[] Line { get; set; }

        /// <summary>
        /// Gets or sets the value for city, fx name of city, town, etc.
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        [StringLength(32)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the value for district (if applicable).
        /// </summary>
        [JsonProperty(PropertyName = "district")]
        [StringLength(32)]
        public string District { get; set; }

        /// <summary>
        /// Gets or sets the value for state, fx sub-unit of country (if applicable). Abreviations can be used.
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        [StringLength(32)]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the value for postal code.
        /// </summary>
        [JsonProperty(PropertyName = "postalCode")]
        [StringLength(16)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the value for country (can be ISO 3166 three letter code).
        /// </summary>
        [JsonProperty(PropertyName = "country")]
        [Required, StringLength(32)]
        public string Country { get; set; }

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public Address()
            : base()
        {
        }
    }
}