using Newtonsoft.Json;
using System;

using LMS;
using LMS.Model;
using LMS.Model.Composite;
using LMS.Model.Annotation;
using LMS.Model.Resource.Enums;

namespace LMS.Model.Resource
{
    public class Account : DomainResource
    {
        /// <summary>
        /// Gets or sets a reference to the owner of the account.
        /// </summary>
        [JsonProperty(PropertyName = "artist")]
        [Required]
        public Reference Artist { get; set; }

        /// <summary>
        /// Gets or sets a reference to the label to which the account is associated.
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        [Required]
        public Reference Label { get; set; }

        /// <summary>
        /// Gets or sets the name of the account, fx the catalog number of the release.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [Required, StringLength(32)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets status of the account.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        [Required]
        public AccountStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the balance of the account.
        /// </summary>
        /// <remarks>This is calculated by the resource server and cannot be set from the client.</remarks>
        [JsonProperty(PropertyName = "balance")]
        public double Balance { get; set; }

        /// <summary>
        /// Gets or sets the transactions associated with the account.
        /// </summary>
        [JsonProperty(PropertyName = "transactions")]
        public Transaction[] Transactions { get; set; }

        /// <summary>
        /// Create instance of class.
        /// </summary>
        public Account()
        {
        }
    }
}