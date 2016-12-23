using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;
using LMS.Model.Resource.Enums;

namespace LMS.Model.Resource
{
    public class Invoice : DomainResource
    {
        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        [JsonProperty(PropertyName = "invoiceNumber")]
        [Required, StringLength(16)]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the specific date/time where the transaction was created.
        /// </summary>
        [JsonProperty(PropertyName = "effectiveTime")]
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the invoice.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        [Required]
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the Paypal address to use for payment of the invoice.
        /// </summary>
        [JsonProperty(PropertyName = "paypalAddress")]
        [Required, StringLength(128)]
        public string PaypalAddress { get; set; }

        /// <summary>
        /// Gets or sets the reference to the statement to which the invoice is associated.
        /// </summary>
        [JsonProperty(PropertyName = "statement")]
        public Reference Statement { get; set; }
        
        /// <summary>
        /// Create an instance of the class.
        /// </summary>
        public Invoice()
        {
        }
    }
}