using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;
using LMS.Model.Resource.Enums;

namespace LMS.Model.Resource
{
    public class Statement : DomainResource
    {
        /// <summary>
        /// Gets or sets a reference to the artist to which this statement is associated.
        /// </summary>
        [JsonProperty(PropertyName = "artist")]
        [Required]
        public Reference Artist { get; set; }

        /// <summary>
        /// Gets or sets a reference to the label to which this statement is associated.
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        [Required]
        public Reference Label { get; set; }

        /// <summary>
        /// Gets or sets the date/time when this statement was effectively generated.
        /// </summary>
        [JsonProperty(PropertyName = "effectiveTime")]
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// Gets or sets the quarter, fx Q3-2016 for which this statement was generated.
        /// </summary>
        [JsonProperty(PropertyName = "quarter")]
        [Required, StringLength(7)]
        public string Quarter { get; set; }

        /// <summary>
        /// Gets or sets the quarter, fx ARTIST-Q32016 for which this statement was generated.
        /// </summary>
        [JsonProperty(PropertyName = "reference")]
        [Required, StringLength(32)]
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the total amount owed to the artist in this statement.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        /// <summary>
        /// Gets or sets status of the statement.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        [Required]
        public StatementStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the data contained with this statement.
        /// </summary>
        /// <remarks>Data contains Accounts including Transactions which has not previously been added to a Statement. If a Statement is cancelled the association between Transaction and Statement must be broken.</remarks>
        [JsonProperty(PropertyName = "data")]
        public Account[] Data { get; set; }
        
        /// <summary>
        /// Gets or sets a reference to the invoice which may be associated with this statement.
        /// </summary>
        [JsonProperty(PropertyName = "invoice")]
        public Reference Invoice { get; set; }

        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public Statement()
        {
        }
    }
}