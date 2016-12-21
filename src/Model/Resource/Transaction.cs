using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;
using LMS.Model.Resource.Enums;

namespace LMS.Model.Resource
{
    public class Transaction : DomainResource
    {
        /// <summary>
        /// Gets or sets the statement which contains the transaction.
        /// </summary>
        /// <remarks>Only set if the transaction has been collected as part of a statement.</remarks>
        [JsonProperty(PropertyName = "statement")]
        public Reference Statement { get; set; }

        /// <summary>
        /// Gets or sets a reference to the account which the transaction occured on.
        /// </summary>
        [JsonProperty(PropertyName = "account")]
        [Required]
        public Reference Account { get; set; }

        /// <summary>
        /// Gets or sets the status of the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        [Required]
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the specific date/time where the transaction was created.
        /// </summary>
        [JsonProperty(PropertyName = "effectiveTime")]
        [Required]
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// Gets or sets the type of the transaction, fx expense, sales, advance or payment.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        [Required]
        public TransactionType Type { get; set; }

        /// <summary>
        /// Gets or sets the amount of the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        [Required]
        public double Amount { get; set; }

        /// <summary>
        /// Gets or sets the amount of the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "quarter")]
        [Required, StringLength(7)]
        public string Quarter { get; set; }

        /// <summary>
        /// Create instance of class.
        /// </summary>
        public Transaction()
        {
        }
    }
}
