using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using LMS;
using LMS.Model;
using LMS.Model.Annotation;

namespace LMS.Model.Composite
{
    /// <summary>
    /// This composite datatype represents human's name with the ability to identify parts.
    /// </summary>
    public class HumanName : CompositeDataType
    {
        /// <summary>
        /// Gets the text representation of the full name.
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text
        {
            get
            {
                string text = "";

                if (Given != null)
                    text = string.Join(" ", Given) + " ";
                if (Family != null)
                    text += string.Join(" ", Family);

                return text;
            }
        }

        /// <summary>
        /// Gets or sets the values for family name (often called surname).
        /// </summary>
        [JsonProperty(PropertyName = "family")]
        [Required, ArrayStringLength(96)]
        public string[] Family { get; set; }

        /// <summary>
        /// Gets or sets the values for given name (often called first name).
        /// </summary>
        [JsonProperty(PropertyName = "given")]
        [Required, ArrayStringLength(96)]
        public string[] Given { get; set; }

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public HumanName()
            : base()
        {
            Family = new string[] { };
            Given = new string[] { };
        }

        /// <summary>
        /// Add a new value to family name.
        /// </summary>
        /// <param name="value"></param>
        public void AddFamily(string value)
        {
            List<string> list = new List<string>(Family);
            list.Add(value);

            Family = list.ToArray();
        }

        /// <summary>
        /// Add a new value to given name.
        /// </summary>
        /// <param name="value"></param>
        public void AddGiven(string value)
        {
            List<string> list = new List<string>(Given);
            list.Add(value);

            Given = list.ToArray();
        }
    }
}